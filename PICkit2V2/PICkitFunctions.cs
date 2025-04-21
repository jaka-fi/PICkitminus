using System;
using System.Collections.Generic;
using System.Text;
using USB = PICkit2V2.USB;
using KONST = PICkit2V2.Constants;
using UTIL = PICkit2V2.Utilities;
using System.IO;
using File = System.IO.File;
using P32 = PICkit2V2.PIC32MXFunctions;
using PE33 = PICkit2V2.dsPIC33_PE;
using Pk3h = PICkit2V2.PK3Helpers;
using System.Diagnostics;
using System.Threading;
using K32 = PICkit2V2.Kernel32;

namespace PICkit2V2
{
	public class PICkitFunctions
	{
		public static DelegateResetStatusBar ResetStatusBar;
		public static DelegateStepStatusBar StepStatusBar;
		
		public static String FirmwareVersion = "NA";
		public static String DeviceFileVersion = "NA";
		public static DeviceFile DevFile = new DeviceFile();
		public static DeviceData DeviceBuffers;
		public static byte[] Usb_write_array = new byte[KONST.PACKET_SIZE];
		public static byte[] Usb_read_array = new byte[KONST.PACKET_SIZE];
		public static int ActivePart = 0;
		public static uint LastDeviceID = 0;
		public static int LastDeviceRev = 0;
		public static bool LearnMode = false;
		public static byte LastICSPSpeed = 0;
		public static string ToolName = "PICkit 2";
		public static bool isPK2M = false;
		public static bool isPK3 = false;
		public static bool isPKOB = false;
		public static String pkobDeviceString = "PKOB";
		public static bool vddErrorDisabled = false;
		private static IntPtr usbReadHandle = IntPtr.Zero;
		private static IntPtr usbWriteHandle = IntPtr.Zero;
		private static IntPtr hEventObject = IntPtr.Zero;
		private static IntPtr wrhEventObject = IntPtr.Zero;
		private static Kernel32.OVERLAPPED HIDOverlapped;
		private static Kernel32.OVERLAPPED HIDWrOverlapped;
		private static ushort lastPk2number = 0xFF;
		private static int[] familySearchTable; // index is search priority, value is family array index.
		private static bool vddOn = false;
		private static float vddLastSet = 3.3F;  // needed when a family VPP=VDD (PIC18J, PIC24, etc.)
		private static bool targetSelfPowered = false;
		private static bool fastProgramming = true;
		private static bool assertMCLR = false;
		private static bool vppFirstEnabled = false;
		private static bool lvpEnabled = false;
		private static uint scriptBufferChecksum = 0;
		private static int lastFoundPart = 0;
		private static scriptRedirect[] scriptRedirectTable = new scriptRedirect[32]; // up to 32 scripts in FW
        

        private struct scriptRedirect
		{
			public byte redirectToScriptLocation;
			public int deviceFileScriptNumber;
		};
		//private static int USB_BYTE_COUNT = 0;

		public static void TestingMethod()
		{
		}

		public static bool CheckComm()
		{
			byte[] commandArray = new byte[17];
			commandArray[0] = KONST.CLR_DOWNLOAD_BUFFER;
			commandArray[1] = KONST.DOWNLOAD_DATA;
			commandArray[2] = 8;
			commandArray[3] = 0x01;
			commandArray[4] = 0x02;
			commandArray[5] = 0x03;
			commandArray[6] = 0x04;
			commandArray[7] = 0x05;
			commandArray[8] = 0x06;
			commandArray[9] = 0x07;
			commandArray[10] = 0x08;
			commandArray[11] = KONST.COPY_RAM_UPLOAD;   // DL buffer starts at 0x100
			commandArray[12] = 0x00;
			commandArray[13] = 0x01;
			commandArray[14] = KONST.UPLOAD_DATA;
			commandArray[15] = KONST.CLR_DOWNLOAD_BUFFER;
			commandArray[16] = KONST.CLR_UPLOAD_BUFFER;
			if (writeUSB(commandArray))
			{
				if (readUSB())
				{
					if (Usb_read_array[1] == 63)
					{
						for (int i = 1; i < 9; i++)
						{
							if (Usb_read_array[1 + i] != i)
							{
								return false;
							}
						}
						return true;
					}
				}
			}
			return false;
		}

		public static bool EnterLearnMode(byte memsize)
		{
			byte[] commandArray = new byte[5];
			commandArray[0] = KONST.ENTER_LEARN_MODE;
			commandArray[1] = 0x50;
			commandArray[2] = 0x4B;
			commandArray[3] = 0x32;
			commandArray[4] = memsize;    // PICkit 2 EEPROM size 0 = 128K, 1 = 256K
			if (writeUSB(commandArray))
			{
				//Thread.Sleep(5000);     // Wait for FLASH erase

				LearnMode = true;
				// Set VPP voltage by family
				float vpp = DevFile.Families[GetActiveFamily()].Vpp;
				if ((vpp < 1) || (lvpEnabled && (DevFile.PartsList[ActivePart].LVPScript > 0)))
				{ // When nominally zero, use VDD voltage
					//UNLESS it's not an LVP script but a HV script (PIC24F-KA-)
					if (lvpEnabled && (DevFile.PartsList[ActivePart].LVPScript > 0))
					{
						string scriptname = DevFile.Scripts[DevFile.PartsList[ActivePart].LVPScript - 1].ScriptName;
						scriptname = scriptname.Substring(scriptname.Length - 2);
						if (scriptname == "HV")
						{
							// the VPP voltage value is the 2nd script element in 100mV increments.
							vpp = (float)DevFile.Scripts[DevFile.PartsList[ActivePart].LVPScript - 1].Script[1] / 10F;
							SetVppVoltage(vpp, 0.7F);
						}
						else
						{
							SetVppVoltage(vddLastSet, 0.7F);
						}
					}
					else
					{
						SetVppVoltage(vddLastSet, 0.7F);
					}
				}
				else
				{
					SetVppVoltage(vpp, 0.7F);
				}
				downloadPartScripts(GetActiveFamily());
				return true;
			}
			return false;
		}

		public static bool ExitLearnMode()
		{
			LearnMode = false;
			byte[] commandArray = new byte[1];
			commandArray[0] = KONST.EXIT_LEARN_MODE;
			return writeUSB(commandArray);
		}

		public static bool EnablePK2GoMode(byte memsize)
		{
			LearnMode = false;
			byte[] commandArray = new byte[5];
			commandArray[0] = KONST.ENABLE_PK2GO_MODE;
			commandArray[1] = 0x50;
			commandArray[2] = 0x4B;
			commandArray[3] = 0x32;
			commandArray[4] = memsize;    // PICkit 2 EEPROM size 0 = 128K, 1 = 256K

			return writeUSB(commandArray);
		}

		public static bool MetaCmd_CHECK_DEVICE_ID()
		{
			int mask = (int)(DevFile.Families[GetActiveFamily()].DeviceIDMask);
			int deviceID = (int)(DevFile.PartsList[ActivePart].DeviceID);
			// JAKA - Added 5.10.2021 to enable PTG with MSB1st families.
			if (DevFile.Families[GetActiveFamily()].FamilyName == "Midrange/1.8V Min MSB1st")
			{
				deviceID <<= 2;
				deviceID = (int)reverse16Bits((uint)deviceID);
				// deviceID &= mask;
			}
			if (DevFile.Families[GetActiveFamily()].FamilyName == "PIC18/PIC18F MSB1st")
			{
				deviceID = (int)reverse16Bits((uint)deviceID);
			}
			// END JAKA
			if (DevFile.Families[GetActiveFamily()].ProgMemShift != 0)
			{
				mask <<= 1;
				deviceID <<= 1;
			}
			byte[] commandArray = new byte[5];
			commandArray[0] = KONST.MC_CHECK_DEVICE_ID;
			commandArray[1] = (byte)(mask & 0xFF);              // device ID mask
			commandArray[2] = (byte)((mask >> 8) & 0xFF);
			commandArray[3] = (byte)(deviceID & 0xFF);          // device ID value
			commandArray[4] = (byte)((deviceID >> 8) & 0xFF);

			return writeUSB(commandArray);
		}

		public static bool MetaCmd_READ_BANDGAP()
		{
			byte[] commandArray = new byte[1];
			commandArray[0] = KONST.MC_READ_BANDGAP;
			return writeUSB(commandArray);
		}

		public static bool MetaCmd_WRITE_CFG_BANDGAP()
		{
			byte[] commandArray = new byte[1];
			commandArray[0] = KONST.MC_WRITE_CFG_BANDGAP;
			return writeUSB(commandArray);
		}

		public static bool MetaCmd_READ_OSCCAL()
		{
			int OSCCALaddress = (int)(DevFile.PartsList[ActivePart].ProgramMem - 1);
			byte[] commandArray = new byte[3];
			commandArray[0] = KONST.MC_READ_OSCCAL;
			commandArray[1] = (byte)(OSCCALaddress & 0xFF);    // OSCALL address
			commandArray[2] = (byte)((OSCCALaddress >> 8) & 0xFF);

			return writeUSB(commandArray);
		}

		public static bool MetaCmd_WRITE_OSCCAL()
		{
			int OSCCALaddress = (int)(DevFile.PartsList[ActivePart].ProgramMem - 1);
			byte[] commandArray = new byte[3];
			commandArray[0] = KONST.MC_WRITE_OSCCAL;
			commandArray[1] = (byte)(OSCCALaddress & 0xFF);    // OSCALL address
			commandArray[2] = (byte)((OSCCALaddress >> 8) & 0xFF);

			return writeUSB(commandArray);
		}

		public static bool MetaCmd_START_CHECKSUM()
		{
			byte[] commandArray = new byte[3];
			commandArray[0] = KONST.MC_START_CHECKSUM;
			commandArray[1] = (byte)DevFile.Families[GetActiveFamily()].ProgMemShift;    //Format
			commandArray[2] = 0;

			return writeUSB(commandArray);
		}

		public static bool MetaCmd_CHANGE_CHKSM_FRMT(byte format)
		{
			byte[] commandArray = new byte[3];
			commandArray[0] = KONST.MC_CHANGE_CHKSM_FRMT;
			commandArray[1] = format;    //Format
			commandArray[2] = 0;

			return writeUSB(commandArray);
		}

		public static bool MetaCmd_VERIFY_CHECKSUM(uint checksum)
		{
			checksum = ~checksum;

			byte[] commandArray = new byte[3];
			commandArray[0] = KONST.MC_VERIFY_CHECKSUM;
			commandArray[1] = (byte)(checksum & 0xFF);    // OSCALL address
			commandArray[2] = (byte)((checksum >> 8) & 0xFF);

			return writeUSB(commandArray);
		}

		public static void ResetPk2Number()
		{
			lastPk2number = 0xFF;
		}

		public static float MeasurePGDPulse()
		{   // !! ASSUMES target is powered !!
			// sets PGC as output, PGD input
			// Asserts PGC (=1) then measures pulse on PGD
			// Leaves PGC/PGD as inputs.
			//
			// Return is pulse length in ms units.
			byte[] commandArray = new byte[13];
			commandArray[0] = KONST.CLR_UPLOAD_BUFFER;
			commandArray[1] = KONST.EXECUTE_SCRIPT;
			commandArray[2] = 9;
			commandArray[3] = KONST._SET_ICSP_PINS;
			commandArray[4] = 0x02;
			commandArray[5] = KONST._DELAY_LONG;
			commandArray[6] = 20;                 // wait 100ms
			commandArray[7] = KONST._SET_ICSP_PINS;
			commandArray[8] = 0x06;                 // signal ready for pulse
			commandArray[9] = KONST._MEASURE_PULSE;
			commandArray[10] = KONST._SET_ICSP_PINS;
			commandArray[11] = 0x03;
			commandArray[12] = KONST.UPLOAD_DATA;   // get data
			if (writeUSB(commandArray))
			{
				if (readUSB())
				{
					// expect 2 bytes
					if (Usb_read_array[1] == 2)
					{
						float ret = (float)(Usb_read_array[2] + (Usb_read_array[3] * 0x100));
						// ret = 0xFFFF on a timeout.
						return (ret * .021333F);
					}
				}
			}
			return 0F;  // failed
		}

		public static bool EnterUARTMode(uint baudValue)
		{
			byte[] commandArray = new byte[5];
			commandArray[0] = KONST.CLR_DOWNLOAD_BUFFER;
			commandArray[1] = KONST.CLR_UPLOAD_BUFFER;
			commandArray[2] = KONST.ENTER_UART_MODE;
			commandArray[3] = (byte)(baudValue & 0xFF);
			commandArray[4] = (byte)((baudValue >> 8) & 0xFF);
			return writeUSB(commandArray);
		}

		public static bool ExitUARTMode()
		{
			byte[] commandArray = new byte[3];
			commandArray[0] = KONST.EXIT_UART_MODE;
			commandArray[1] = KONST.CLR_DOWNLOAD_BUFFER;
			commandArray[2] = KONST.CLR_UPLOAD_BUFFER;
			return writeUSB(commandArray);
		}

		public static bool ValidateOSSCAL()
		{
			uint value = DeviceBuffers.OSCCAL;
			value &= 0xFF00;
			if ((value != 0) && (value == DevFile.PartsList[ActivePart].ConfigMasks[KONST.OSCCAL_MASK]))
			{
				return true;
			}
			return false;
		}

		public static bool isCalibrated()
		{
			byte[] commandArray = new byte[3];
			commandArray[0] = KONST.RD_INTERNAL_EE;
			commandArray[1] = KONST.ADC_CAL_L;
			commandArray[2] = 4;
			if (writeUSB(commandArray))
			{
				if (readUSB())
				{
					int adcCal = Usb_read_array[1] + (Usb_read_array[2] * 0x100);
					if ((adcCal <= 0x140) && (adcCal >= 0xC0))
					{ // ADC cal within limits.
						if ((Usb_read_array[1] == 0x00) && (Usb_read_array[2] == 0x01)
							&& (Usb_read_array[3] == 0x00) && (Usb_read_array[4] == 0x80))
						{// but not default cals
							return false;
						}
						return true;
					}
				}
			}

			return false;
		}

		public static string UnitIDRead()
		{ // returns a zero-length string if no ID.
			string unitID = "";

			byte[] commandArray = new byte[3];
			commandArray[0] = KONST.RD_INTERNAL_EE;
			commandArray[1] = KONST.UNIT_ID;
			commandArray[2] = 16;
			if (writeUSB(commandArray))
			{
				if (readUSB())
				{
					if (Usb_read_array[1] == 0x23)
					{
						byte[] readBytes;
						int i = 0;
						for (; i < 15; i++)
						{
							if (Usb_read_array[2 + i] == 0)
							{
								break;
							}
						}
						readBytes = new byte[i];
						Array.Copy(Usb_read_array, 2, readBytes, 0, i);
						char[] asciiChars = new char[Encoding.ASCII.GetCharCount(readBytes, 0, readBytes.Length)];
						Encoding.ASCII.GetChars(readBytes, 0, readBytes.Length, asciiChars, 0);
						string newString = new string(asciiChars);
						unitID = newString;
					}

				}
			}

			return unitID;
		}

		public static bool UnitIDWrite(string unitID)
		{
			int length = unitID.Length;
			if (length > 15)
			{
				length = 15;
			}
			byte[] commandArray = new byte[4 + 15];
			commandArray[0] = KONST.WR_INTERNAL_EE;
			commandArray[1] = KONST.UNIT_ID;
			commandArray[2] = 0x10;
			byte[] unicodeBytes = Encoding.Unicode.GetBytes(unitID);
			byte[] asciiBytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, unicodeBytes);
			if (length > 0)
			{
				commandArray[3] = 0x23; // '#' first byte is always ASCII pound to indicate valid UnitID
			}
			else
			{
				commandArray[3] = 0xFF; // clear UnitID
			}

			for (int i = 0; i < 15; i++)
			{
				if (i < length)
				{
					commandArray[4 + i] = asciiBytes[i];
				}
				else
				{
					commandArray[4 + i] = 0;
				}
			}

			return writeUSB(commandArray);

		}

		public static bool SetVoltageCals(ushort adcCal, byte vddOffset, byte VddCal)
		{
			byte[] commandArray = new byte[5];
			commandArray[0] = KONST.SET_VOLTAGE_CALS;
			commandArray[1] = (byte)adcCal;
			commandArray[2] = (byte)(adcCal >> 8);
			commandArray[3] = vddOffset;
			commandArray[4] = VddCal;
			return writeUSB(commandArray);
		}

		public static bool HCS360_361_VppSpecial()
		{
			if (DevFile.PartsList[ActivePart].DeviceID != 0xFFFFFF36)
			{ // only HCS360, 361 need this.
				return true;
			}

			byte[] commandArray = new byte[12];
			commandArray[0] = KONST.EXECUTE_SCRIPT;
			commandArray[1] = 10;
			if ((DeviceBuffers.ProgramMemory[0] & 0x1) == 0)
			{ // bit 0 word 0 is 0
				commandArray[2] = KONST._SET_ICSP_PINS; // data goes low with VPP staying low, clock high
				commandArray[3] = 0x04;
				commandArray[4] = KONST._MCLR_GND_ON;
				commandArray[5] = KONST._VPP_OFF;
				commandArray[6] = KONST._DELAY_LONG;
				commandArray[7] = 0x05;
				commandArray[8] = KONST._SET_ICSP_PINS; // data set to bit 0 word 0, clock high
				commandArray[9] = 0x04;
				commandArray[10] = KONST._SET_ICSP_PINS; // clock low, data keeps value
				commandArray[11] = 0x00;
			}
			else
			{ // bit 0 word 0 is 1
				commandArray[2] = KONST._SET_ICSP_PINS; // data goes low with VPP high, clock high
				commandArray[3] = 0x04;
				commandArray[4] = KONST._MCLR_GND_OFF;
				commandArray[5] = KONST._VPP_ON;
				commandArray[6] = KONST._DELAY_LONG;
				commandArray[7] = 0x05;
				commandArray[8] = KONST._SET_ICSP_PINS; // data set to bit 0 word 0, clock high
				commandArray[9] = 0x0C;
				commandArray[10] = KONST._SET_ICSP_PINS; // clock low, data keeps value
				commandArray[11] = 0x08;
			}
			return writeUSB(commandArray);

		}

		public static bool FamilyIsEEPROM()
		{
			int maxLength = DevFile.Families[GetActiveFamily()].FamilyName.Length;
			if (maxLength > 6)
			{
				maxLength = 6;
			}
			return (DevFile.Families[GetActiveFamily()].FamilyName.Substring(0, maxLength) == "EEPROM");
		}

		public static bool FamilyIsKeeloq()
		{
			return (DevFile.Families[GetActiveFamily()].FamilyName == "Others/KEELOQ® HCS");
		}

		public static bool FamilyIsMCP()
		{
			int maxLength = DevFile.Families[GetActiveFamily()].FamilyName.Length;
			if (maxLength > 10)
			{
				maxLength = 10;
			}
			return (DevFile.Families[GetActiveFamily()].FamilyName.Substring(0, maxLength) == "Others/MCP");
		}

		//public static bool PartSupportsBlankSkipping()
		//{
		//	bool supportsSkip = true;
		//	//if (DevFile.PartsList[ActivePart].PartName == "PIC16LF720" || DevFile.PartsList[ActivePart].PartName == "PIC16LF720")
		//	//	supportsSkip = false;
		//	if (FamilyIsMCP())
		//		supportsSkip = false;

		//	return supportsSkip;
		//}


		public static bool NewStyleConfigs()
		{
			if ((DevFile.PartsList[ActivePart].ProgMemPanelBufs & 0xf0) == 160
				|| (DevFile.PartsList[ActivePart].ProgMemPanelBufs & 0xf0) == 176
				|| (DevFile.PartsList[ActivePart].ProgMemPanelBufs & 0xf0) == 192
				|| (DevFile.PartsList[ActivePart].ProgMemPanelBufs & 0xf0) == 224
				|| (DevFile.PartsList[ActivePart].ProgMemPanelBufs & 0xf0) == 240)
				return true;
			else
				return false;
		}

		public static bool PartHasCustomerOTP()
		{
			if ((DevFile.PartsList[ActivePart].ProgMemPanelBufs & 0xf0) == 160
				|| (DevFile.PartsList[ActivePart].ProgMemPanelBufs & 0xf0) == 176
				|| (DevFile.PartsList[ActivePart].ProgMemPanelBufs & 0xf0) == 192
				|| (DevFile.PartsList[ActivePart].ProgMemPanelBufs & 0xf0) == 224
				|| (DevFile.PartsList[ActivePart].ProgMemPanelBufs & 0xf0) == 240)
				return true;
			else
				return false;
		}

		public static bool FamilyIsPIC32()
		{
			int maxLength = DevFile.Families[GetActiveFamily()].FamilyName.Length;
			if (maxLength > 5)
			{
				maxLength = 5;
			}
			return (DevFile.Families[GetActiveFamily()].FamilyName.Substring(0, maxLength) == "PIC32");
		}

		public static bool FamilyIsdsPIC30()
		{
			int maxLength = DevFile.Families[GetActiveFamily()].FamilyName.Length;
			if (maxLength > 14)
			{
				maxLength = 14;
			}
			return (DevFile.Families[GetActiveFamily()].FamilyName.Substring(0, maxLength) == "Others/dsPIC30");
		}

		public static bool FamilyIsdsPIC30SMPS()
		{
			int maxLength = DevFile.Families[GetActiveFamily()].FamilyName.Length;
			if (maxLength > 16)
			{
				maxLength = 16;
			}
			return (DevFile.Families[GetActiveFamily()].FamilyName.Substring(0, maxLength) == "Others/dsPIC30 S");
		}

		public static bool FamilyIsPIC18J()
		{
			int maxLength = DevFile.Families[GetActiveFamily()].FamilyName.Length;
			if (maxLength > 15)	// was 9
			{
				maxLength = 15;	// was 9
			}
			return (DevFile.Families[GetActiveFamily()].FamilyName.Substring(0, maxLength) == "PIC18/PIC18F_J_"); // was PIC18F_J_
		}

		public static bool FamilyIsPIC24FJ()
		{
			int maxLength = DevFile.PartsList[ActivePart].PartName.Length;
			if (maxLength > 7)
			{
				maxLength = 7;
			}
			return (DevFile.PartsList[ActivePart].PartName.Substring(0, maxLength) == "PIC24FJ");
		}

		public static bool FamilyIsPIC24H()
		{
			int maxLength = DevFile.PartsList[ActivePart].PartName.Length;
			if (maxLength > 6)
			{
				maxLength = 6;
			}
			return (DevFile.PartsList[ActivePart].PartName.Substring(0, maxLength) == "PIC24H");
		}

		public static bool FamilyIsPIC24EP()
		{
			int maxLength = DevFile.PartsList[ActivePart].PartName.Length;
			if (maxLength > 7)
			{
				maxLength = 7;
			}
			return (DevFile.PartsList[ActivePart].PartName.Substring(0, maxLength) == "PIC24EP");
		}

		public static bool FamilyIsdsPIC33EP()
		{
			int maxLength = DevFile.PartsList[ActivePart].PartName.Length;
			if (maxLength > 9)
			{
				maxLength = 9;
			}
			return (DevFile.PartsList[ActivePart].PartName.Substring(0, maxLength) == "dsPIC33EP");
		}

		public static bool FamilyIsdsPIC33F()
		{
			int maxLength = DevFile.PartsList[ActivePart].PartName.Length;
			if (maxLength > 8)
			{
				maxLength = 8;
			}
			return (DevFile.PartsList[ActivePart].PartName.Substring(0, maxLength) == "dsPIC33F");
		}

		public static void SetVPPFirstProgramEntry()
		{
			vppFirstEnabled = true;
			scriptBufferChecksum = ~scriptBufferChecksum; // force redownload of scripts
		}

		public static void ClearVppFirstProgramEntry()
		{
			vppFirstEnabled = false;
			scriptBufferChecksum = ~scriptBufferChecksum; // force redownload of scripts
		}

		public static void SetLVPProgramEntry()
		{
			lvpEnabled = true;
			scriptBufferChecksum = ~scriptBufferChecksum; // force redownload of scripts
		}

		public static void ClearLVPProgramEntry()
		{
			lvpEnabled = false;
			scriptBufferChecksum = ~scriptBufferChecksum; // force redownload of scripts
		}

		public static void RowEraseDevice()
		{
			// row erase script automatically increments PC by number of locations erased.
			// --- Erase Program Memory  ---
			int memoryRows = (int)DevFile.PartsList[ActivePart].ProgramMem / DevFile.PartsList[ActivePart].DebugRowEraseSize;
			RunScript(KONST.PROG_ENTRY, 1);
			if (DevFile.PartsList[ActivePart].ProgMemWrPrepScript != 0)
			{ // if prog mem address set script exists for this part
				DownloadAddress3(0);
				RunScript(KONST.PROGMEM_WR_PREP, 1);
			}
			do
			{
				if (memoryRows >= 256)
				{ // erase up to 256 rows at a time               
					RunScript(KONST.ROW_ERASE, 0);  // 0 = 256 times
					memoryRows -= 256;
				}
				else
				{
					RunScript(KONST.ROW_ERASE, memoryRows);
					memoryRows = 0;
				}

			} while (memoryRows > 0 && !FormPICkit2.stopOperation);
			RunScript(KONST.PROG_EXIT, 1);

			// --- Erase EEPROM Data ---
			// only dsPIC30 currently needs this done
			if (DevFile.PartsList[ActivePart].EERowEraseScript > 0)
			{
				int eeRows = (int)DevFile.PartsList[ActivePart].EEMem / DevFile.PartsList[ActivePart].EERowEraseWords;
				RunScript(KONST.PROG_ENTRY, 1);
				if (DevFile.PartsList[ActivePart].EERdPrepScript != 0)
				{ // if ee mem address set script exists for this part
					DownloadAddress3((int)DevFile.PartsList[ActivePart].EEAddr
										/ DevFile.Families[GetActiveFamily()].EEMemBytesPerWord);
					RunScript(KONST.EE_RD_PREP, 1);
				}
				do
				{
					if (eeRows >= 256)
					{ // erase up to 256 rows at a time               
						RunScript(KONST.EEROW_ERASE, 0);  // 0 = 256 times
						eeRows -= 256;
					}
					else
					{
						RunScript(KONST.EEROW_ERASE, eeRows);
						eeRows = 0;
					}

				} while (eeRows > 0);
				RunScript(KONST.PROG_EXIT, 1);

			}

			// --- Erase Config Memory  ---
			if (DevFile.PartsList[ActivePart].ConfigMemEraseScript > 0)
			{
				RunScript(KONST.PROG_ENTRY, 1);
				if (DevFile.PartsList[ActivePart].ProgMemWrPrepScript != 0)
				{ // if prog mem address set script exists for this part
					DownloadAddress3((int)DevFile.PartsList[ActivePart].UserIDAddr);
					RunScript(KONST.PROGMEM_WR_PREP, 1);
				}
				ExecuteScript(DevFile.PartsList[ActivePart].ConfigMemEraseScript);
				RunScript(KONST.PROG_EXIT, 1);
			}
		}

		public static bool ExecuteScript(int scriptArrayIndex)
		{
			// IMPORTANT NOTE: THIS ALWAYS CLEARS THE UPLOAD BUFFER FIRST!

			int scriptLength;
			if (scriptArrayIndex == 0)
				return false;

			scriptLength = DevFile.Scripts[--scriptArrayIndex].ScriptLength;

			//int scriptLength = DevFile.Scripts[--scriptArrayIndex].ScriptLength;

			byte[] commandArray = new byte[3 + scriptLength];
			commandArray[0] = KONST.CLR_UPLOAD_BUFFER;
			commandArray[1] = KONST.EXECUTE_SCRIPT;
			commandArray[2] = (byte)scriptLength;
			for (int n = 0; n < scriptLength; n++)
			{
				commandArray[3 + n] = (byte)DevFile.Scripts[scriptArrayIndex].Script[n];
			}
			return writeUSB(commandArray);
		}


		public static bool GetVDDState()
		{
			return vddOn;
		}

		public static bool SetMCLRTemp(bool nMCLR)
		{
			byte[] releaseMCLRscript = new byte[1];
			if (nMCLR)
			{
				releaseMCLRscript[0] = KONST._MCLR_GND_ON;
			}
			else
			{
				releaseMCLRscript[0] = KONST._MCLR_GND_OFF;
			}
			return SendScript(releaseMCLRscript);
		}

		public static bool HoldMCLR(bool nMCLR)
		{
			assertMCLR = nMCLR;

			byte[] releaseMCLRscript = new byte[1];
			if (nMCLR)
			{
				releaseMCLRscript[0] = KONST._MCLR_GND_ON;
			}
			else
			{
				releaseMCLRscript[0] = KONST._MCLR_GND_OFF;
			}
			return SendScript(releaseMCLRscript);
		}

		public static void SetFastProgramming(bool fast)
		{
			fastProgramming = fast;
			// alter checksum so scripts will reload on next operation.
			scriptBufferChecksum = ~scriptBufferChecksum;
		}

		public static void ForcePICkitPowered()
		{
			targetSelfPowered = false;
		}

		public static void ForceTargetPowered()
		{
			targetSelfPowered = true;
		}

		public static void ReadConfigOutsideProgMem()
		{
			RunScript(KONST.PROG_ENTRY, 1);
			RunScript(KONST.CONFIG_RD, 1);
			UploadData();
			RunScript(KONST.PROG_EXIT, 1);
			int configWords = DevFile.PartsList[ActivePart].ConfigWords;
			int bufferIndex = 2;                    // report starts on index 1, which is #bytes uploaded.
			for (int word = 0; word < configWords; word++)
			{
				uint config = (uint)Usb_read_array[bufferIndex++];
				config |= (uint)Usb_read_array[bufferIndex++] << 8;
				//$$$Bequest333 and JAKA
				if (DevFile.Families[GetActiveFamily()].FamilyName == "Midrange/1.8V Min MSB1st" ||
					DevFile.Families[GetActiveFamily()].FamilyName == "PIC18/PIC18F MSB1st")
				{
					config = reverse16Bits(config);
					if (DevFile.PartsList[ActivePart].IgnoreBytes == 0x000C
						|| (DevFile.PartsList[ActivePart].ProgMemPanelBufs & 0x02) == 0x02)    // Q40,Q41,Q43,Q71,Q83,Q84 configuration is read/written byte at a time
							config = swap2Bytes(config);
					//if (word == (DevFile.PartsList[ActivePart].IgnoreAddress - DevFile.PartsList[ActivePart].ConfigAddr) / DevFile.Families[GetActiveFamily()].BytesPerLocation)
					//	config |= 0xff00;                       // Some compilers put 0xff to non-existing config byte. Use same when reading out, then the exported hex file matches the original.
				}
				//$$$
				if (DevFile.Families[GetActiveFamily()].ProgMemShift > 0)
				{
					config = (config >> 1) & DevFile.Families[GetActiveFamily()].BlankValue;
				}
				DeviceBuffers.ConfigWords[word] = config;
			}
		}

		public static void ReadBandGap()
		{
			RunScript(KONST.PROG_ENTRY, 1);
			RunScript(KONST.CONFIG_RD, 1);
			UploadData();
			RunScript(KONST.PROG_EXIT, 1);
			int configWords = DevFile.PartsList[ActivePart].ConfigWords;
			uint config = (uint)Usb_read_array[2];
			config |= (uint)Usb_read_array[3] << 8;
			//$$$Bequest333 and JAKA
			if (DevFile.Families[GetActiveFamily()].FamilyName == "Midrange/1.8V Min MSB1st" ||
				DevFile.Families[GetActiveFamily()].FamilyName == "PIC18/PIC18F MSB1st")
			{
				config = reverse16Bits(config);
			}
			//$$$
			if (DevFile.Families[GetActiveFamily()].ProgMemShift > 0)
			{
				config = (config >> 1) & DevFile.Families[GetActiveFamily()].BlankValue;
			}
			DeviceBuffers.BandGap = config & DevFile.PartsList[ActivePart].BandGapMask;
		}

		public static uint WriteConfigOutsideProgMem(bool codeProtect, bool dataProtect)
		{
			int configWords = DevFile.PartsList[ActivePart].ConfigWords;
			uint checksumPk2Go = 0;
			byte[] configBuffer = new byte[configWords * 2];

			if (DevFile.PartsList[ActivePart].BandGapMask > 0)
			{
				DeviceBuffers.ConfigWords[0] &= ~DevFile.PartsList[ActivePart].BandGapMask;
				if (!LearnMode)
					DeviceBuffers.ConfigWords[0] |= DeviceBuffers.BandGap;
			}
			if (FamilyIsMCP())
			{
				DeviceBuffers.ConfigWords[0] |= 0x3FF8;
			}

			RunScript(KONST.PROG_ENTRY, 1);

			if (DevFile.PartsList[ActivePart].ConfigWrPrepScript > 0)
			{
				DownloadAddress3(0);
				RunScript(KONST.CONFIG_WR_PREP, 1);
			}

			for (int i = 0, j = 0; i < configWords; i++)
			{
				uint configWord;
				if (i<9)
					configWord = DeviceBuffers.ConfigWords[i] & DevFile.PartsList[ActivePart].ConfigMasks[i];
				else
					configWord = DeviceBuffers.ConfigWords[i];		// 27.8.2021 JAKA - Do not use mask if more than 9 config words

				if (i == DevFile.PartsList[ActivePart].CPConfig - 1)
				{
					if (codeProtect)
					{
						configWord &= (uint)~DevFile.PartsList[ActivePart].CPMask;
					}
					if (dataProtect)
					{
						configWord &= (uint)~DevFile.PartsList[ActivePart].DPMask;
					}
				}
				if (DevFile.Families[GetActiveFamily()].ProgMemShift > 0)
				{ // baseline & midrange
					configWord |= (~(uint)DevFile.PartsList[ActivePart].ConfigMasks[i] & ~DevFile.PartsList[ActivePart].BandGapMask);
					if (!FamilyIsMCP())
						configWord &= DevFile.Families[GetActiveFamily()].BlankValue;
					configWord = configWord << 1;
				}
				// JAKA - on 18F MSB1st parts the unimplemented config bits read as 1, not 0
				if ((DevFile.Families[GetActiveFamily()].FamilyName == "PIC18/PIC18F MSB1st") && (i < 9))
				{
					configWord |= (~(uint)DevFile.PartsList[ActivePart].ConfigMasks[i]);
				}
				// END JAKA

				//$$$Bequest333 and JAKA
				if (DevFile.Families[GetActiveFamily()].FamilyName == "Midrange/1.8V Min MSB1st" ||
					DevFile.Families[GetActiveFamily()].FamilyName == "PIC18/PIC18F MSB1st")
				{
					configWord = reverse16Bits(configWord);
					if (DevFile.PartsList[ActivePart].IgnoreBytes == 0x000C
						|| (DevFile.PartsList[ActivePart].ProgMemPanelBufs & 0x02) == 0x02)    // Q40,Q41,Q43,Q71,Q83,Q84 configuration is read/written byte at a time
						configWord = swap2Bytes(configWord);
					if (((DevFile.PartsList[ActivePart].ProgMemPanelBufs & 0x04) == 0x04) && (i == configWords-1))
						configWord &= 0x00ff;						// Some compilers put 0xff to non-existing config byte. Mask it out! Even better would be not to write it to pickit buffer at all
				}
				//$$$
				configBuffer[j++] = (byte)(configWord & 0xFF);
				configBuffer[j++] = (byte)((configWord >> 8) & 0xFF);
				checksumPk2Go += (byte)(configWord & 0xFF);
				checksumPk2Go += (byte)((configWord >> 8) & 0xFF);
			}
			DataClrAndDownload(configBuffer, 0);

			if (LearnMode && (DevFile.PartsList[ActivePart].BandGapMask > 0))
				MetaCmd_WRITE_CFG_BANDGAP();
			else
				RunScript(KONST.CONFIG_WR, 1);
			RunScript(KONST.PROG_EXIT, 1);
			return checksumPk2Go;
		}

		/// <summary>
		/// Special config write function to "unlock" (disable code protect) on the PIC12F529. 
		/// This does not change the device buffers.
		/// </summary>
		/// <param name="codeProtect"></param>
		/// <param name="dataProtect"></param>
		/// <returns></returns>
		public static uint Unlock529Config(bool codeProtect, bool dataProtect)
		{
			uint checksumPk2Go = 0;

			// Save a copy of the data-to-write. We need to replace it with the "blank"
			// config value, as function WriteConfigOutsideProgMem uses those buffers
			// to do its work. Note that we know the 12F529 has only one config word.

			uint backupConfigWords = DeviceBuffers.ConfigWords[0];

			// Use "blank" value as unlock value.
			DeviceBuffers.ConfigWords[0] = DevFile.PartsList[ActivePart].ConfigBlank[0];

			// Write value using normal means.
			checksumPk2Go = WriteConfigOutsideProgMem(codeProtect, dataProtect);

			// Restore original config value.
			DeviceBuffers.ConfigWords[0] = backupConfigWords;

			return checksumPk2Go;
		}

		public static bool ReadOSSCAL()
		{
			if (RunScript(KONST.PROG_ENTRY, 1))
			{
				if (DownloadAddress3((int)(DevFile.PartsList[ActivePart].ProgramMem - 1)))
				{
					if (RunScript(KONST.OSSCAL_RD, 1))
					{
						if (UploadData())
						{
							if (RunScript(KONST.PROG_EXIT, 1))
							{
								DeviceBuffers.OSCCAL = (uint)(Usb_read_array[2] + (Usb_read_array[3] * 256));
								//$$$Bequest333 and JAKA
								if (DevFile.Families[GetActiveFamily()].FamilyName == "Midrange/1.8V Min MSB1st" ||
									DevFile.Families[GetActiveFamily()].FamilyName == "PIC18/PIC18F MSB1st")
								{
									DeviceBuffers.OSCCAL = reverse16Bits(DeviceBuffers.OSCCAL);
								}
								//$$$
								if (DevFile.Families[GetActiveFamily()].ProgMemShift > 0)
								{
									DeviceBuffers.OSCCAL >>= 1;
								}
								DeviceBuffers.OSCCAL &= DevFile.Families[GetActiveFamily()].BlankValue;
								//DeviceBuffers.OSCCAL = 0xc00;
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public static bool WriteOSSCAL()
		{
			if (RunScript(KONST.PROG_ENTRY, 1))
			{
				uint calWord = DeviceBuffers.OSCCAL;
				uint calAddress = DevFile.PartsList[ActivePart].ProgramMem - 1;
				if (DevFile.Families[GetActiveFamily()].ProgMemShift > 0)
				{
					calWord <<= 1;
				}
				//$$$Bequest333 and JAKA
				if (DevFile.Families[GetActiveFamily()].FamilyName == "Midrange/1.8V Min MSB1st" ||
					DevFile.Families[GetActiveFamily()].FamilyName == "PIC18/PIC18F MSB1st")
				{
					calWord = reverse16Bits(calWord);
				}
				//$$$
				byte[] addressData = new byte[5];
				addressData[0] = (byte)(calAddress & 0xFF);
				addressData[1] = (byte)((calAddress >> 8) & 0xFF);
				addressData[2] = (byte)((calAddress >> 16) & 0xFF);
				addressData[3] = (byte)(calWord & 0xFF);
				addressData[4] = (byte)((calWord >> 8) & 0xFF);
				DataClrAndDownload(addressData, 0);
				if (RunScript(KONST.OSSCAL_WR, 1))
				{
					if (RunScript(KONST.PROG_EXIT, 1))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static KONST.PICkit2PWR CheckTargetPower(ref float vdd, ref float vpp)
		{
			if (vddOn)  // if VDD on, can't check for self-powered target
			{
				return KONST.PICkit2PWR.vdd_on;
			}

			if (ReadPICkitVoltages(ref vdd, ref vpp))
			{
				if (vdd > KONST.VddThresholdForSelfPoweredTarget)
				{
					targetSelfPowered = true;
					SetVDDVoltage(vdd, 0.85F, true);                     // set VDD to target level, update VPP if necessary
					return KONST.PICkit2PWR.selfpowered;
				}
				targetSelfPowered = false;
				return KONST.PICkit2PWR.unpowered;
			}
			targetSelfPowered = false;
			return KONST.PICkit2PWR.no_response;
		}

		public static int GetActiveFamily()
		{
			return DevFile.PartsList[ActivePart].Family;
		}

		public static void SetActiveFamily(int family)
		{
			ActivePart = 0;
			lastFoundPart = 0;
			DevFile.PartsList[ActivePart].Family = (ushort)family;
			// Set up the buffers for unsupported part (else they remain last selected part)
			ResetBuffers();
		}

		public static bool SetVDDVoltage(float voltage, float threshold, bool updateVPP)
		{
			bool retval;
			if (voltage < 1.7F)
			{
				voltage = 1.7F;  // minimum, as when forcing VDD Target can get set very low (last reading)
				// and too low prevents VPP pump from working.
			}

			vddLastSet = voltage;

			if (!isPK3)
			{
				ushort ccpValue = CalculateVddCPP(voltage);
				byte vFault = (byte)(((threshold * voltage) / 5F) * 255F);
				if (vFault > 210)
				{
					vFault = 210; // ~4.12v maximum.  Because of diode droop, limit threshold on high side.
				}
				if (vddErrorDisabled && vFault > 127)
				{
					vFault = 127;	// always ~2.5V if VDED: Y in .ini file
				}

				byte[] commandArray = new byte[4];
				commandArray[0] = KONST.SETVDD;
				commandArray[1] = (byte)(ccpValue & 0xFF);
				commandArray[2] = (byte)(ccpValue / 256);
				commandArray[3] = vFault;

				retval =  writeUSB(commandArray);
			}
			else
			{
				//TODO change firmware to handle more accurate voltage steps
				ushort vddValue = (ushort)(voltage / 0.125F);

				byte[] commandArray = new byte[3];
				commandArray[0] = KONST.SETVDD;
				commandArray[1] = (byte)(vddValue & 0xFF);
				commandArray[2] = (byte)(vddValue / 256);

				retval =  writeUSB(commandArray);
			}

			if (updateVPP)
            {
				float vpp = DevFile.Families[GetActiveFamily()].Vpp;
				if ((vpp < 1) && (DevFile.Families[GetActiveFamily()].DeviceIDMask == 0)) //&& !(lvpEnabled && (DevFile.PartsList[ActivePart].LVPScript > 0)))
				{
					SetVppVoltage(vddLastSet, 0.7F);	// Adjust VPP for families which don't use device ID checks, such as SPI EEPROM.
				}
			}
			return retval;
		}

		public static ushort CalculateVddCPP(float voltage)
		{
			ushort ccpValue = (ushort)(voltage * 32F + 10.5F);
			ccpValue <<= 6;
			return ccpValue;
		}

		public static bool VddOn()
		{
			byte[] commandArray = new byte[4];
			commandArray[0] = KONST.EXECUTE_SCRIPT;
			commandArray[1] = 2;
			commandArray[2] = KONST._VDD_GND_OFF;
			if (targetSelfPowered)
			{   // don't turn on VDD if self-powered!
				commandArray[3] = KONST._VDD_OFF;
			}
			else
			{
				commandArray[3] = KONST._VDD_ON;
			}
			bool result = writeUSB(commandArray);
			if (result)
			{
				vddOn = true;
				return true;
			}
			return result;
		}

		public static bool VddOff()
		{
			byte[] commandArray = new byte[4];
			commandArray[0] = KONST.EXECUTE_SCRIPT;
			commandArray[1] = 2;
			commandArray[2] = KONST._VDD_OFF;
			if (targetSelfPowered)
			{ // don't ground VDD if self-powered target
				commandArray[3] = KONST._VDD_GND_OFF;
			}
			else
			{
				commandArray[3] = KONST._VDD_GND_ON;
			}
			bool result = writeUSB(commandArray);
			if (result)
			{
				vddOn = false;
				return true;
			}
			return result;
		}

		public static bool SetProgrammingSpeed(byte speed)
		{
			LastICSPSpeed = speed;

			byte[] commandArray = new byte[4];
			commandArray[0] = KONST.EXECUTE_SCRIPT;
			commandArray[1] = 2;
			commandArray[2] = KONST._SET_ICSP_SPEED;
			commandArray[3] = speed;
			return writeUSB(commandArray);
		}

		public static bool ResetPICkit2()
		{
			byte[] commandArray = new byte[1];
			commandArray[0] = KONST.RESET;
			return writeUSB(commandArray);
		}

		public static bool EnterBootloader()
		{
			byte[] commandArray = new byte[1];
			commandArray[0] = KONST.ENTER_BOOTLOADER;
			return writeUSB(commandArray);
		}

		public static bool VerifyBootloaderMode()
		{
			byte[] commandArray = new byte[1];
			commandArray[0] = KONST.FIRMWARE_VERSION;
			if (writeUSB(commandArray))
			{
				if (readUSB())
				{
					if (Usb_read_array[1] == 118) // ASCII 'B'
					{
						return true;
					}
					return false;
				}
				return false;
			}
			return false;
		}

		public static bool BL_EraseFlash()
		{
			byte[] commandArray = new byte[5];
			commandArray[0] = KONST.ERASEFWFLASH;
			commandArray[1] = 0xC0; //# 32-word blocks to erase
			commandArray[2] = 0x00;
			commandArray[3] = 0x20;
			commandArray[4] = 0x00;
			if (writeUSB(commandArray))
			{
				commandArray[3] = 0x50;
				return writeUSB(commandArray);
			}
			return false;
		}

		public static bool BL_WriteFlash(byte[] payload)
		{
			byte[] commandArray = new byte[2 + 35];
			commandArray[0] = KONST.WRITEFWFLASH;
			commandArray[1] = 32;
			for (int i = 0; i < 35; i++)
			{
				commandArray[2 + i] = payload[i];
			}
			return writeUSB(commandArray);

		}

		public static bool BL_WriteFWLoadedKey()
		{
			byte[] flashWriteData = new byte[3 + 32];  // 3 address bytes plus 32 data bytes.

			flashWriteData[0] = 0xE0;
			flashWriteData[1] = 0x7F;
			flashWriteData[2] = 0x00;   // Address = 0x007FE0
			for (int i = 3; i < flashWriteData.Length; i++)
			{
				flashWriteData[i] = 0xFF;
			}
			flashWriteData[flashWriteData.Length - 2] = 0x55;
			flashWriteData[flashWriteData.Length - 1] = 0x55;
			return BL_WriteFlash(flashWriteData);
		}

		public static bool BL_ReadFlash16(int address)
		{
			byte[] commandArray = new byte[5];
			commandArray[0] = KONST.READFWFLASH;
			commandArray[1] = 16;
			commandArray[2] = (byte)(address & 0xFF);
			commandArray[3] = (byte)((address >> 8) & 0xFF);
			commandArray[4] = 0x00;
			if (writeUSB(commandArray))
			{
				return readUSB();
			}
			return false;

		}

		public static bool BL_Reset()
		{
			byte[] commandArray = new byte[1];
			commandArray[0] = KONST.RESETFWDEVICE;

			return writeUSB(commandArray);

		}

		public static bool ButtonPressed()
		{
			ushort status = readPkStatus();
			if ((status & 0x0040) == 0x0040)
			{
				return true;
			}
			return false;
		}
		public static bool EEPROMNotFound()
		{
			ushort status = readPkStatus();
			if ((status & 0x0080) == 0x0080)        // PTG EEPROM missing
            {
				return true;
            }
			return false;
		}
		public static bool BusErrorCheck()
		{
			ushort status = readPkStatus();
			if ((status & 0x0400) == 0x0400)
			{
				return true; //error
			}

			byte[] commandArray = new byte[3];
			commandArray[0] = KONST.EXECUTE_SCRIPT;
			commandArray[1] = 1;
			commandArray[2] = KONST._BUSY_LED_ON;
			writeUSB(commandArray);

			return false; // no error
		}

		public static bool GetVersions_MPLAB()
		{
			byte[] commandArray = new byte[2];
			commandArray[0] = KONST.GETVERSIONS_MPLAB;
			commandArray[1] = 0;
			if (writeUSB_MPLAB(commandArray))
			{
				if (readUSB())
				{
					Pk3h.fwdownloadsuccess = (Usb_read_array[1] == KONST.GETVERSIONS_MPLAB &&
												Usb_read_array[2] == 0 &&
												Usb_read_array[5] == 0 &&
												Usb_read_array[6] == 0) ? true : false;
					if (Pk3h.fwdownloadsuccess)
					{
						Pk3h.os_typ = Usb_read_array[7];
						Pk3h.os_maj = Usb_read_array[8];
						Pk3h.os_min = Usb_read_array[9];
						Pk3h.os_rev = Usb_read_array[10];

						Pk3h.ap_typ = Usb_read_array[11];
						Pk3h.ap_maj = Usb_read_array[12];
						Pk3h.ap_min = Usb_read_array[13];
						Pk3h.ap_rev = Usb_read_array[14];
					}

					Pk3h.MagicKey = Usb_read_array[31] + (Usb_read_array[32] << 8) + (Usb_read_array[33] << 16);

					return true;
				}
				else
				{
					return false;
				}
			}
			else
				return false;
		}

		public static bool SwitchToBootLoader(ref KONST.MPLABerrorcodes errorcode)
		{
			errorcode = KONST.MPLABerrorcodes.Unknownerror;
			byte[] commandArray = new byte[2];
			commandArray[0] = KONST.SWITCH_TO_BL;
			commandArray[1] = 0;
			if (writeUSB(commandArray))
			{
				if (readUSB())
				{
					if ((Usb_read_array[1] == KONST.SWITCH_TO_BL) && (Usb_read_array[2] == 0))
					{
						if (Usb_read_array[3] == 0x99)
						{
							return true;
						}
						else if (Usb_read_array[3] == 0xAA)
						{
							errorcode = KONST.MPLABerrorcodes.Bootloadernotfound;
							return false;
						}
						else
							return false;
					}
					else
						return false;
				}
				else
					return false;
			}
			else
				return false;
		}

		public static bool TestPk3BootloaderExists()
		{
			byte[] commandArray = new byte[2];
			commandArray[0] = KONST.TEST_BOOTLOADER;
			commandArray[1] = 0;
			if (writeUSB(commandArray))
			{
				if (readUSB())
				{
					if ((Usb_read_array[1] == KONST.TEST_BOOTLOADER) && (Usb_read_array[2] == 0))
					{
						if (Usb_read_array[3] == 0x99)
						{
							return true;
						}
						else if (Usb_read_array[3] == 0xAA)
						{
							return false;
						}
						else
							return false;
					}
					else
						return false;
				}
				else
					return false;
			}
			else
				return false;
		}

		public static bool CheckandDownloadFirmware()
		{
			if (!GetVersions_MPLAB())
				return false;

			// TODO: Check for Magic Key here to identify scripting firmware.

			// TODO: Check whether we have an RS or a Bootloader. Assume an RS for now.
			// TODO: routines to find and load proper files
			// TODO: do proper error propagation and checking all the way to the top
			Pk3h.WriteProgrammerOs("bootloader.hex", 0x23);
			Pk3h.WriteProgrammerOs("firmware.hex", 0x22);
			return true;
		}

		public static KONST.PICkit2PWR PowerStatus()
		{
			ushort status = readPkStatus();
			
			if (status == 0xFFFF)
			{
				return KONST.PICkit2PWR.no_response;
			}
			if ((status & 0x0030) == 0x0030)
			{
				vddOn = false;
				return KONST.PICkit2PWR.vddvpperrors;
			}
			if ((status & 0x0020) == 0x0020)
			{
				vddOn = false;
				return KONST.PICkit2PWR.vpperror;
			}
			if ((status & 0x0010) == 0x0010)
			{
				vddOn = false;
				return KONST.PICkit2PWR.vdderror;
			}
			if ((status & 0x0002) == 0x0002)
			{
				vddOn = true;
				return KONST.PICkit2PWR.vdd_on;
			}
			vddOn = false;
			return KONST.PICkit2PWR.vdd_off;

		}

		public static void DisconnectPICkit2Unit()
		{
			if (usbWriteHandle != IntPtr.Zero)
				USB.CloseHandle(usbWriteHandle);
			if (usbReadHandle != IntPtr.Zero)
				USB.CloseHandle(usbReadHandle);
			usbReadHandle = IntPtr.Zero;
			usbWriteHandle = IntPtr.Zero;
		}
		public static string GetSerialUnitID()
		{
			return USB.UnitID;
		}

		public static KONST.PICkit2USB DetectPICkit2Device(ushort pk2ID, bool readFW)
		{
			IntPtr usbRdTemp = IntPtr.Zero;
			IntPtr usbWrTemp = IntPtr.Zero;

			DisconnectPICkit2Unit();

			bool result = USB.Find_This_Device(pk2ID, ref usbRdTemp, ref usbWrTemp);

			// If we use this check and keep the old handles, we'll read whatever packets
			// were read by somebody else looking for pk2 units, messing up communications.
			//if (pk2ID != lastPk2number)
			//{ // new unit selected
			lastPk2number = pk2ID;
			// update handles
			usbReadHandle = usbRdTemp;
			usbWriteHandle = usbWrTemp;
			//}
			/*
			// Test that PICkit is not stuck
			if (result)
			{
				// return KONST.PICkit2USB.writeError;
				try
				{
					Thread tt = new System.Threading.Thread(tryWriteUSB);
					tt.Start();
					//Thread.Sleep(2000);
					//tt.Abort();
					//return KONST.PICkit2USB.writeError;
					//Thread.Sleep(10);
					if (!tt.Join(100))
					{
						// return KONST.PICkit2USB.writeError; 
						tt.Abort();
						//throw new Exception("More than 30 secs.");
						return KONST.PICkit2USB.writeError;
					}
				}
                catch
                {
					return KONST.PICkit2USB.writeError;
				}
			}
			*/

			if (result && !readFW)
				return KONST.PICkit2USB.found;

			if (result)
			{
				if (!isPK3)
				{
					//Read firmware version - this will exit PK2Go mode if needed
					byte[] commandArray = new byte[1];
					commandArray[0] = KONST.FIRMWARE_VERSION;

					FirmwareVersion = string.Format("{0:D1}.{1:D2}.{2:D2}", 2,
										32, 99);
					result = writeUSB(commandArray);
					if (result)
					{
						// read response
						if (readUSB())
						{
							// create a version string
							FirmwareVersion = string.Format("{0:D1}.{1:D2}.{2:D2}", Usb_read_array[1],
										Usb_read_array[2], Usb_read_array[3]);
							// check for minimum supported version
							if (!isPK2M)    // Original PICkit2 (genuine or clone)
							{
								if (Usb_read_array[1] == KONST.FWVerMajorReq)
								{
									if (((Usb_read_array[2] == KONST.FWVerMinorReq)
										&& (Usb_read_array[3] >= KONST.FWVerDotReq))
										|| (Usb_read_array[2] > KONST.FWVerMinorReq))
									{
										return KONST.PICkit2USB.found;
									}
								}
							}
							else
							{
								// PK2M
								if (Usb_read_array[1] == KONST.FWVerMajorReqPK2M)
								{
									if (((Usb_read_array[2] == KONST.FWVerMinorReqPK2M)
										&& (Usb_read_array[3] >= KONST.FWVerDotReqPK2M))
										|| (Usb_read_array[2] > KONST.FWVerMinorReqPK2M))
									{
										return KONST.PICkit2USB.found;
									}
								}
							}
							if (Usb_read_array[1] == 118)
							{
								FirmwareVersion = string.Format("BL {0:D1}.{1:D1}", Usb_read_array[7], Usb_read_array[8]);
								return KONST.PICkit2USB.bootloader;

							}
							return KONST.PICkit2USB.firmwareInvalid;
						}
						return KONST.PICkit2USB.readError;
					}
					return KONST.PICkit2USB.writeError;
				}
				else
				{
					if (GetVersions_MPLAB())
					{
						if (Pk3h.MagicKey != KONST.Pk3MagicKey)
						{
							if (!Pk3h.fwdownloadsuccess)
								return KONST.PICkit2USB.firmwareInvalid;
							else if (Pk3h.ap_typ == KONST.MPLAB_BOOTLOADERTYPE)
								return KONST.PICkit2USB.bootloader;
							else
							{
								// TODO: compare versions
								return KONST.PICkit2USB.pk3mplab;
							}
						}
						else
						{
							// create a version string. skip the magic key
							FirmwareVersion = string.Format("{0:D1}.{1:D2}.{2:D2}", Usb_read_array[34],
										Usb_read_array[35], Usb_read_array[36]);
							// check for minimum supported version
							if (Usb_read_array[34] == KONST.FWVerMajorReqPk3)
							{
								if (((Usb_read_array[35] == KONST.FWVerMinorReqPk3)
									&& (Usb_read_array[36] >= KONST.FWVerDotReqPk3))
									|| (Usb_read_array[35] > KONST.FWVerMinorReqPk3))
								{
									return KONST.PICkit2USB.found;
								}
							}
							return KONST.PICkit2USB.firmwareOldversion;
						}
					}
					return KONST.PICkit2USB.readwriteError;
				}
			}
			return KONST.PICkit2USB.notFound;

		}

		public static bool ReadDeviceFile(string DeviceFileName)
		{
			bool fileExists = File.Exists(DeviceFileName);
			if (fileExists)
			{
				try
				{
					//FileStream fsDevFile = File.Open(DeviceFileName, FileMode.Open);
					FileStream fsDevFile = File.OpenRead(DeviceFileName);
					using (BinaryReader binRead = new BinaryReader(fsDevFile))
					{
						//
						DevFile.Info.VersionMajor = binRead.ReadInt32();
						DevFile.Info.VersionMinor = binRead.ReadInt32();
						DevFile.Info.VersionDot = binRead.ReadInt32();
						DevFile.Info.VersionNotes = binRead.ReadString();
						DevFile.Info.NumberFamilies = binRead.ReadInt32();
						DevFile.Info.NumberParts = binRead.ReadInt32();
						DevFile.Info.NumberScripts = binRead.ReadInt32();
						DevFile.Info.Compatibility = binRead.ReadByte();
						DevFile.Info.UNUSED1A = binRead.ReadByte();
						DevFile.Info.UNUSED1B = binRead.ReadUInt16();
						DevFile.Info.UNUSED2 = binRead.ReadUInt32();
						// create a version string
						DeviceFileVersion = string.Format("{0:D1}.{1:D2}.{2:D2}", DevFile.Info.VersionMajor,
										 DevFile.Info.VersionMinor, DevFile.Info.VersionDot);
						//
						// Declare arrays
						//
						DevFile.Families = new DeviceFile.DeviceFamilyParams[DevFile.Info.NumberFamilies];
						DevFile.PartsList = new DeviceFile.DevicePartParams[DevFile.Info.NumberParts];
						DevFile.Scripts = new DeviceFile.DeviceScripts[DevFile.Info.NumberScripts];
						//
						// now read all families if they are there
						//
						for (int l_x = 0; l_x < DevFile.Info.NumberFamilies; l_x++)
						{
							DevFile.Families[l_x].FamilyID = binRead.ReadUInt16();
							DevFile.Families[l_x].FamilyType = binRead.ReadUInt16();
							DevFile.Families[l_x].SearchPriority = binRead.ReadUInt16();
							DevFile.Families[l_x].FamilyName = binRead.ReadString();
							DevFile.Families[l_x].ProgEntryScript = binRead.ReadUInt16();
							DevFile.Families[l_x].ProgExitScript = binRead.ReadUInt16();
							DevFile.Families[l_x].ReadDevIDScript = binRead.ReadUInt16();
							DevFile.Families[l_x].DeviceIDMask = binRead.ReadUInt32();
							DevFile.Families[l_x].BlankValue = binRead.ReadUInt32();
							DevFile.Families[l_x].BytesPerLocation = binRead.ReadByte();
							DevFile.Families[l_x].AddressIncrement = binRead.ReadByte();
							DevFile.Families[l_x].PartDetect = binRead.ReadBoolean();
							DevFile.Families[l_x].ProgEntryVPPScript = binRead.ReadUInt16();
							DevFile.Families[l_x].UNUSED1 = binRead.ReadUInt16();
							DevFile.Families[l_x].EEMemBytesPerWord = binRead.ReadByte();
							DevFile.Families[l_x].EEMemAddressIncrement = binRead.ReadByte();
							DevFile.Families[l_x].UserIDHexBytes = binRead.ReadByte();
							DevFile.Families[l_x].UserIDBytes = binRead.ReadByte();
							DevFile.Families[l_x].ProgMemHexBytes = binRead.ReadByte();
							DevFile.Families[l_x].EEMemHexBytes = binRead.ReadByte();
							DevFile.Families[l_x].ProgMemShift = binRead.ReadByte();
							DevFile.Families[l_x].TestMemoryStart = binRead.ReadUInt32();
							DevFile.Families[l_x].TestMemoryLength = binRead.ReadUInt16();
							DevFile.Families[l_x].Vpp = binRead.ReadSingle();
						}
						// Create family search table based on priority
						familySearchTable = new int[DevFile.Info.NumberFamilies];
						for (int familyIdx = 0; familyIdx < DevFile.Info.NumberFamilies; familyIdx++)
						{
							familySearchTable[DevFile.Families[familyIdx].SearchPriority] = familyIdx;
						}
						//
						// now read all parts if they are there
						//
						for (int l_x = 0; l_x < DevFile.Info.NumberParts; l_x++)
						{
							DevFile.PartsList[l_x].PartName = binRead.ReadString();
							DevFile.PartsList[l_x].Family = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DeviceID = binRead.ReadUInt32();
							DevFile.PartsList[l_x].ProgramMem = binRead.ReadUInt32();
							DevFile.PartsList[l_x].EEMem = binRead.ReadUInt16();
							DevFile.PartsList[l_x].EEAddr = binRead.ReadUInt32();
							DevFile.PartsList[l_x].ConfigWords = binRead.ReadByte();
							DevFile.PartsList[l_x].ConfigAddr = binRead.ReadUInt32();
							DevFile.PartsList[l_x].UserIDWords = binRead.ReadByte();
							DevFile.PartsList[l_x].UserIDAddr = binRead.ReadUInt32();
							DevFile.PartsList[l_x].BandGapMask = binRead.ReadUInt32();
							// Init config arrays
							DevFile.PartsList[l_x].ConfigMasks = new ushort[KONST.NumConfigMasks];
							DevFile.PartsList[l_x].ConfigBlank = new ushort[KONST.NumConfigMasks];
							for (int l_index = 0; l_index < KONST.MaxReadCfgMasks; l_index++)
							{
								DevFile.PartsList[l_x].ConfigMasks[l_index] = binRead.ReadUInt16();
							}
							for (int l_index = 0; l_index < KONST.MaxReadCfgMasks; l_index++)
							{
								DevFile.PartsList[l_x].ConfigBlank[l_index] = binRead.ReadUInt16();
							}
							DevFile.PartsList[l_x].CPMask = binRead.ReadUInt16();
							DevFile.PartsList[l_x].CPConfig = binRead.ReadByte();
							DevFile.PartsList[l_x].OSSCALSave = binRead.ReadBoolean();
							DevFile.PartsList[l_x].IgnoreAddress = binRead.ReadUInt32();
							DevFile.PartsList[l_x].VddMin = binRead.ReadSingle();
							DevFile.PartsList[l_x].VddMax = binRead.ReadSingle();
							DevFile.PartsList[l_x].VddErase = binRead.ReadSingle();
							DevFile.PartsList[l_x].CalibrationWords = binRead.ReadByte();
							DevFile.PartsList[l_x].ChipEraseScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ProgMemAddrSetScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ProgMemAddrBytes = binRead.ReadByte();
							DevFile.PartsList[l_x].ProgMemRdScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ProgMemRdWords = binRead.ReadUInt16();
							DevFile.PartsList[l_x].EERdPrepScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].EERdScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].EERdLocations = binRead.ReadUInt16();
							DevFile.PartsList[l_x].UserIDRdPrepScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].UserIDRdScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ConfigRdPrepScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ConfigRdScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ProgMemWrPrepScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ProgMemWrScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ProgMemWrWords = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ProgMemPanelBufs = binRead.ReadByte();
							DevFile.PartsList[l_x].ProgMemPanelOffset = binRead.ReadUInt32();
							DevFile.PartsList[l_x].EEWrPrepScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].EEWrScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].EEWrLocations = binRead.ReadUInt16();
							DevFile.PartsList[l_x].UserIDWrPrepScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].UserIDWrScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ConfigWrPrepScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ConfigWrScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].OSCCALRdScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].OSCCALWrScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DPMask = binRead.ReadUInt16();
							DevFile.PartsList[l_x].WriteCfgOnErase = binRead.ReadBoolean();
							DevFile.PartsList[l_x].BlankCheckSkipUsrIDs = binRead.ReadBoolean();
							DevFile.PartsList[l_x].IgnoreBytes = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ChipErasePrepScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].BootFlash = binRead.ReadUInt32();
							//DevFile.PartsList[l_x].UNUSED4 = binRead.ReadUInt32();
							DevFile.PartsList[l_x].Config9Mask = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ConfigMasks[8] = DevFile.PartsList[l_x].Config9Mask;
							DevFile.PartsList[l_x].Config9Blank = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ConfigBlank[8] = DevFile.PartsList[l_x].Config9Blank;
							DevFile.PartsList[l_x].ProgMemEraseScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].EEMemEraseScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ConfigMemEraseScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].reserved1EraseScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].reserved2EraseScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].TestMemoryRdScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].TestMemoryRdWords = binRead.ReadUInt16();
							DevFile.PartsList[l_x].EERowEraseScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].EERowEraseWords = binRead.ReadUInt16();
							DevFile.PartsList[l_x].ExportToMPLAB = binRead.ReadBoolean();
							DevFile.PartsList[l_x].DebugHaltScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugRunScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugStatusScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugReadExecVerScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugSingleStepScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugBulkWrDataScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugBulkRdDataScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugWriteVectorScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugReadVectorScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugRowEraseScript = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugRowEraseSize = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugReserved5Script = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugReserved6Script = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugReserved7Script = binRead.ReadUInt16();
							DevFile.PartsList[l_x].DebugReserved8Script = binRead.ReadUInt16();
							//DevFile.PartsList[l_x].DebugReserved9Script = binRead.ReadUInt16();                                                       
							DevFile.PartsList[l_x].LVPScript = binRead.ReadUInt16();
							// Fill the config masks/blanks for >9 which aren't stored in device file
							// for (int l_index = 9; l_index < KONST.NumConfigMasks; l_index++)
							for (int l_index = 9; l_index < DevFile.PartsList[l_x].ConfigWords; l_index++)
							{
									DevFile.PartsList[l_x].ConfigMasks[l_index] = 0xffff;
							}
							// for (int l_index = 9; l_index < KONST.NumConfigMasks; l_index++)
							for (int l_index = 9; l_index < DevFile.PartsList[l_x].ConfigWords; l_index++)
							{
									DevFile.PartsList[l_x].ConfigBlank[l_index] = 0xffff;
							}

						}
						//
						// now read all scripts if they are there
						//                    
						for (int l_x = 0; l_x < DevFile.Info.NumberScripts; l_x++)
						{
							DevFile.Scripts[l_x].ScriptNumber = binRead.ReadUInt16();
							DevFile.Scripts[l_x].ScriptName = binRead.ReadString();
							DevFile.Scripts[l_x].ScriptVersion = binRead.ReadUInt16();
							DevFile.Scripts[l_x].UNUSED1 = binRead.ReadUInt32();
							DevFile.Scripts[l_x].ScriptLength = binRead.ReadUInt16();
							// init script array
							DevFile.Scripts[l_x].Script = new ushort[DevFile.Scripts[l_x].ScriptLength];
							for (int l_index = 0; l_index < DevFile.Scripts[l_x].ScriptLength; l_index++)
							{
								DevFile.Scripts[l_x].Script[l_index] = binRead.ReadUInt16();
							}
							DevFile.Scripts[l_x].Comment = binRead.ReadString();
						}

						binRead.Close();
					}
					fsDevFile.Close();
				}
				catch
				{
					return false;
				}
				return true;
			}
			else
			{
				return false;
			}

		}
		
		public static bool DetectDevice(int familyIndex, bool resetOnNotFound, bool keepVddOn, FormPICkit2 aObj)
		{
			// Detect a device in the given Family of familyIndex, or all families

			if (familyIndex == KONST.SEARCH_ALL_FAMILIES)
			{
				// Check how many families support part detect so the status bar maximum can be set
				int numDetectableFamilies = 0;
				for (int searchIndex = 0; searchIndex < DevFile.Families.Length; searchIndex++)
				{
					if (DevFile.Families[familySearchTable[searchIndex]].PartDetect)
					{
						numDetectableFamilies++;
					}
				}
				ResetStatusBar(numDetectableFamilies);

				float searchVdd, previousSearchVdd=0.0F;
				for (int searchIndex = 0; searchIndex < DevFile.Families.Length; searchIndex++)
				{
					int famLength = DevFile.Families[familySearchTable[searchIndex]].FamilyName.Length;
					aObj.ExtCallShowText("\nSearching family " + DevFile.Families[familySearchTable[searchIndex]].FamilyName);
					StepStatusBar();
					searchVdd = 3.3F;
					if (famLength >= 3)
					{
						if (DevFile.Families[familySearchTable[searchIndex]].FamilyName.Substring(famLength - 3, 3) == "1v8")
						{
							searchVdd = 1.8F;
						}
					}

					// when searching all families, set Vdd according to family. 3.3v for others than SPIFLASH 1v8 (currently)
					if (!targetSelfPowered && searchVdd != previousSearchVdd)
					{ //but not if self-powered target
						SetVDDVoltage(searchVdd, 0.85F, false);
						Thread.Sleep(50);	// allow voltage to drop, to prevent overvoltage spikes
					}

					previousSearchVdd = searchVdd;

					if (DevFile.Families[familySearchTable[searchIndex]].PartDetect)
					{
						if (searchDevice(familySearchTable[searchIndex], true, keepVddOn))
						// 0 = no supported part found
						{
							ResetStatusBar(0);
							return true;
						}
					}
				}
				ResetStatusBar(0);
				return false; // no supported part found in any family
			}
			else
			{
				// reset VDD 
				SetVDDVoltage(vddLastSet, 0.85F, false);

				if (DevFile.Families[familyIndex].PartDetect)
				{
					if (searchDevice(familyIndex, resetOnNotFound, keepVddOn))
					{
						return true;
					}
					return false;
				}
				else
				{
					return true;    // don't fail unsearchable families like baseline.
				}
			}


		}
		
		public static int FindLastUsedInBuffer(uint[] bufferToSearch, uint blankValue,
												int startIndex)
		{   // go backawards from the start entry to find the last non-blank entry
			if (DevFile.Families[GetActiveFamily()].FamilyName != "KEELOQ® HCS")
			{
				for (int index = startIndex; index > 0; index--)
				{
					if (bufferToSearch[index] != blankValue)
					{
						return index;
					}
				}
			}
			else
			{
				return bufferToSearch.Length - 1;
			}

			return 0;
		}

		public static bool RunScriptUploadNoLen(int script, int repetitions)
		{
			// IMPORTANT NOTE: THIS ALWAYS CLEARS THE UPLOAD BUFFER FIRST!

			byte[] commandArray = new byte[5];
			commandArray[0] = KONST.CLR_UPLOAD_BUFFER;
			commandArray[1] = KONST.RUN_SCRIPT;
			commandArray[2] = scriptRedirectTable[script].redirectToScriptLocation;
			commandArray[3] = (byte)repetitions;
			commandArray[4] = KONST.UPLOAD_DATA_NOLEN;
			bool result = writeUSB(commandArray);
			if (result)
			{
				result = readUSB();
			}
			return result;
		}

		// Deprecated for v2.60.00
		public static bool RunScriptUploadNoLen2(int script, int repetitions)
		{
			// IMPORTANT NOTE: THIS ALWAYS CLEARS THE UPLOAD BUFFER FIRST!
			
			byte[] commandArray = new byte[6];
			commandArray[0] = KONST.CLR_UPLOAD_BUFFER;
			commandArray[1] = KONST.RUN_SCRIPT;
			commandArray[2] = scriptRedirectTable[script].redirectToScriptLocation;
			commandArray[3] = (byte)repetitions;
			commandArray[4] = KONST.UPLOAD_DATA_NOLEN;
			commandArray[5] = KONST.UPLOAD_DATA_NOLEN;
			bool result = writeUSB(commandArray);
			if (result)
			{
				result = readUSB();
			}
			return result;
		}

		public static bool GetUpload()
		{
			return readUSB();
		}

		public static bool UploadData()
		{
			byte[] commandArray = new byte[1];
			commandArray[0] = KONST.UPLOAD_DATA;
			bool result = writeUSB(commandArray);
			if (result)
			{
				result = readUSB();
			}
			return result;
		}

		public static bool UploadDataNoLen()
		{
			byte[] commandArray = new byte[1];
			commandArray[0] = KONST.UPLOAD_DATA_NOLEN;
			bool result = writeUSB(commandArray);
			if (result)
			{
				result = readUSB();
			}
			return result;
		}

		public static bool RunScript(int script, int repetitions)
		{
			// IMPORTANT NOTE: THIS ALWAYS CLEARS THE UPLOAD BUFFER FIRST!

			byte[] commandArray = new byte[4];
			commandArray[0] = KONST.CLR_UPLOAD_BUFFER;
			commandArray[1] = KONST.RUN_SCRIPT;
			commandArray[2] = scriptRedirectTable[script].redirectToScriptLocation;
			commandArray[3] = (byte)repetitions;
			if (writeUSB(commandArray))
			{
				if ((script == KONST.PROG_EXIT) && (!assertMCLR))
				{
					return HoldMCLR(false);
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		public static int DataClrAndDownload(byte[] dataArray, int startIndex)
		// returns index of next byte to be transmitted. 0 = failed
		{
			if (startIndex >= dataArray.Length)
			{
				return 0;
			}
			int length = dataArray.Length - startIndex;
			if (length > 61)
			{
				length = 61;
			}
			byte[] commandArray = new byte[3 + length];
			commandArray[0] = KONST.CLR_DOWNLOAD_BUFFER;
			commandArray[1] = KONST.DOWNLOAD_DATA;
			commandArray[2] = (byte)(length & 0xFF);
			for (int i = 0; i < length; i++)
			{
				commandArray[3 + i] = dataArray[startIndex + i];
			}
			if (writeUSB(commandArray))
			{
				return (startIndex + length);
			}
			else
			{
				return 0;
			}
		}

		public static int DataDownload(byte[] dataArray, int startIndex, int lastIndex)
		// returns index of next byte to be transmitted. 0 = failed
		{
			if (startIndex >= lastIndex)
			{
				return 0;
			}
			int length = lastIndex - startIndex;
			if (length > 62)
			{
				length = 62;
			}
			byte[] commandArray = new byte[2 + length];
			commandArray[0] = KONST.DOWNLOAD_DATA;
			commandArray[1] = (byte)(length & 0xFF);
			for (int i = 0; i < length; i++)
			{
				commandArray[2 + i] = dataArray[startIndex + i];
			}
			if (writeUSB(commandArray))
			{
				return (startIndex + length);
			}
			else
			{
				return 0;
			}
		}

		public static bool DownloadAddress3(int address)
		{
			byte[] commandArray = new byte[6];
			commandArray[0] = KONST.CLR_DOWNLOAD_BUFFER;
			commandArray[1] = KONST.DOWNLOAD_DATA;
			commandArray[2] = 3;
			if (DevFile.Families[GetActiveFamily()].FamilyName == "Midrange/1.8V Min MSB1st" ||
				DevFile.Families[GetActiveFamily()].FamilyName == "PIC18/PIC18F MSB1st")
			{
				address <<= 1;	// add stop bit
				commandArray[3] = (byte)reverse8Bits((uint)(0xFF & (address >> 16)));
				commandArray[4] = (byte)reverse8Bits((uint)(0xFF & (address >> 8)));
				commandArray[5] = (byte)reverse8Bits((uint)(address & 0xFF));
			}
			else
			{
				commandArray[3] = (byte)(address & 0xFF);
				commandArray[4] = (byte)(0xFF & (address >> 8));
				commandArray[5] = (byte)(0xFF & (address >> 16));
			}
			return writeUSB(commandArray);
		}

		public static bool DownloadAddress3MSBFirst(int address)
		{
			byte[] commandArray = new byte[6];
			commandArray[0] = KONST.CLR_DOWNLOAD_BUFFER;
			commandArray[1] = KONST.DOWNLOAD_DATA;
			commandArray[2] = 3;
			commandArray[3] = (byte)(0xFF & (address >> 16));
			commandArray[4] = (byte)(0xFF & (address >> 8));
			commandArray[5] = (byte)(address & 0xFF);
			return writeUSB(commandArray);
		}

		public static bool DownloadAddress3MSBitFirst(int address)
		{
			byte[] commandArray = new byte[6];
			commandArray[0] = KONST.CLR_DOWNLOAD_BUFFER;
			commandArray[1] = KONST.DOWNLOAD_DATA;
			commandArray[2] = 3;
			commandArray[3] = (byte)reverse8Bits((uint)(0xFF & (address >> 16)));
			commandArray[4] = (byte)reverse8Bits((uint)(0xFF & (address >> 8)));
			commandArray[5] = (byte)reverse8Bits((uint)(address & 0xFF));
			//commandArray[3] = 0x00;
			//commandArray[4] = 0x00;
			//commandArray[5] = 0x10;

			return writeUSB(commandArray);
		}

		public static bool Download3Multiples(int downloadBytes, int multiples, int increment)
		{
			byte firstCommand = KONST.CLR_DOWNLOAD_BUFFER;

			do
			{
				int thisWrite = multiples;
				if (multiples > 20) // can only write 20 per USB packet. (20 * 3 = 60 bytes)
				{
					thisWrite = 20;
					multiples -= 20;
				}
				else
				{
					multiples = 0;
				}
				byte[] commandArray = new byte[(3 * thisWrite) + 3];
				commandArray[0] = firstCommand;
				commandArray[1] = KONST.DOWNLOAD_DATA;
				commandArray[2] = (byte)(3 * thisWrite);
				for (int i = 0; i < thisWrite; i++)
				{
					commandArray[3 + (3 * i)] = (byte)(downloadBytes >> 16);
					commandArray[4 + (3 * i)] = (byte)(downloadBytes >> 8);
					commandArray[5 + (3 * i)] = (byte)downloadBytes;

					downloadBytes += increment;
				}

				if (!writeUSB(commandArray))
				{
					return false;
				}

				firstCommand = KONST.NO_OPERATION;
			} while (multiples > 0);

			return true;
		}

		public static bool Download3MultiplesRunScriptUploadNolen(int downloadBytes, int multiples, int increment, bool blockingReadEnabled)
		{
			byte firstCommand = KONST.CLR_DOWNLOAD_BUFFER;
			bool result;
			int repetitions = multiples;

			do
			{
				int thisWrite = multiples;
				int lastWrite = 1;
				if (multiples > 20) // can only write 20 per USB packet. (20 * 3 = 60 bytes)
				{
					thisWrite = 20;
					multiples -= 20;
					lastWrite = 0;
				}
				else if (multiples > 18) // the run script and upload commands still donn't fit (18 * 3 = 54 bytes)
				{
					thisWrite = 18;
					multiples -= 18;
					lastWrite = 0;
				}
				else
				{
					multiples = 0;
				}
				byte[] commandArray = new byte[(3 * thisWrite) + 3 + lastWrite*(5 + Convert.ToByte(blockingReadEnabled))];
				commandArray[0] = firstCommand;
				commandArray[1] = KONST.DOWNLOAD_DATA;
				commandArray[2] = (byte)(3 * thisWrite);
				
				for (int i = 0; i < thisWrite; i++)
				{
					commandArray[3 + (3 * i)] = (byte)(downloadBytes >> 16);
					commandArray[4 + (3 * i)] = (byte)(downloadBytes >> 8);
					commandArray[5 + (3 * i)] = (byte)downloadBytes;

					downloadBytes += increment;
				}

				if (lastWrite == 1)
				{
					commandArray[3 + (3 * thisWrite)] = KONST.CLR_UPLOAD_BUFFER;
					commandArray[4 + (3 * thisWrite)] = KONST.RUN_SCRIPT;
					commandArray[5 + (3 * thisWrite)] = scriptRedirectTable[KONST.PROGMEM_RD].redirectToScriptLocation;
					commandArray[6 + (3 * thisWrite)] = (byte)repetitions;
					commandArray[7 + (3 * thisWrite)] = KONST.UPLOAD_DATA_NOLEN;
					if (blockingReadEnabled)
                    {
						commandArray[8 + (3 * thisWrite)] = KONST.UPLOAD_DATA_NOLEN;
					}
				}

				result = writeUSB(commandArray);
				if (!result)
				{
					return false;
				}

				firstCommand = KONST.NO_OPERATION;
			} while (multiples > 0);

			
			if (result)
			{
				result = readUSB();
			}
			return result;
		}


		public static uint ComputeChecksum(bool codeProtectOn, bool dataProtectOn)
		{
			uint checksum = 0;

			if (DevFile.Families[GetActiveFamily()].BlankValue < 0xFFFF)
			{ // 16F and baseline parts are calculated a word at a time.
				// prog mem first
				int progMemEnd = (int)DevFile.PartsList[ActivePart].ProgramMem;

				if (DevFile.PartsList[ActivePart].OSSCALSave)
				{ // don't include last location for devices with OSSCAL 
					progMemEnd--;
				}

				if (DevFile.PartsList[ActivePart].ConfigWords > 0)
				{
					if (((DevFile.PartsList[ActivePart].CPMask & DeviceBuffers.ConfigWords[DevFile.PartsList[ActivePart].CPConfig - 1])
							!= DevFile.PartsList[ActivePart].CPMask) || codeProtectOn)
					{
						if (DevFile.Families[GetActiveFamily()].BlankValue < 0x3FFF)
						{
							progMemEnd = 0x40; // BASELINE - last location of unprotected mem
						}
						else
						{
							progMemEnd = 0; // no memory included for midrange.
						}
					}
				}

				for (int idx = 0; idx < progMemEnd; idx++)
				{
					checksum += DeviceBuffers.ProgramMemory[idx];
				}

				if (DevFile.PartsList[ActivePart].ConfigWords > 0)
				{
					if (((DevFile.PartsList[ActivePart].CPMask & DeviceBuffers.ConfigWords[DevFile.PartsList[ActivePart].CPConfig - 1])
							!= DevFile.PartsList[ActivePart].CPMask) || codeProtectOn)
					{ // if a code protect bit is set, the checksum is computed differently.
						//checksum = 0; // don't include memory (moved above)
						for (int idx = 0; idx < DevFile.PartsList[ActivePart].UserIDWords; idx++)
						{ // add last nibble of UserIDs in decreasing nibble positions of checksum
							int idPosition = 1;
							for (int factor = 0; factor < idx; factor++)
							{
								idPosition <<= 4;
							}
							checksum += (uint)((0xF & DeviceBuffers.UserIDs[DevFile.PartsList[ActivePart].UserIDWords - idx - 1])
								 * idPosition);
						}
					}

					// config words
					uint tempword = 0;
					for (int idx = 0; idx < DevFile.PartsList[ActivePart].ConfigWords; idx++)
					{
						if (idx == (DevFile.PartsList[ActivePart].CPConfig - 1))
						{
							tempword = (DeviceBuffers.ConfigWords[idx] & DevFile.PartsList[ActivePart].ConfigMasks[idx]);
							if (codeProtectOn)
								tempword &= (uint)~DevFile.PartsList[ActivePart].CPMask;
							if (dataProtectOn)
								tempword &= (uint)~DevFile.PartsList[ActivePart].DPMask;
							checksum += tempword;
						}
						else if (idx < 9)
						{
							checksum += (DeviceBuffers.ConfigWords[idx] & DevFile.PartsList[ActivePart].ConfigMasks[idx]);
						}
						else
                        {
							checksum += DeviceBuffers.ConfigWords[idx];
						}
					}
				}
				return (checksum & 0xFFFF);
			}
			else
			{ //PIC18, PIC24 are computed a byte at a time.
				int progMemEnd = (int)DevFile.PartsList[ActivePart].ConfigAddr / DevFile.Families[GetActiveFamily()].ProgMemHexBytes;
				if (progMemEnd > DevFile.PartsList[ActivePart].ProgramMem)
				{
					progMemEnd = (int)DevFile.PartsList[ActivePart].ProgramMem;
				}

				for (int idx = 0; idx < progMemEnd; idx++)
				{
					uint memWord = DeviceBuffers.ProgramMemory[idx];
					checksum += (memWord & 0x000000FF);
					for (int bite = 1; bite < DevFile.Families[GetActiveFamily()].BytesPerLocation; bite++)
					{
						memWord >>= 8;
						checksum += (memWord & 0x000000FF);
					}
				}
				
				if (DevFile.PartsList[ActivePart].ConfigWords > 0)
				{
					if (((DevFile.PartsList[ActivePart].CPMask & DeviceBuffers.ConfigWords[DevFile.PartsList[ActivePart].CPConfig - 1])
						!= DevFile.PartsList[ActivePart].CPMask) || codeProtectOn)
					{ // if a code protect bit is set, the checksum is computed differently.
						// NOTE: this will only match MPLAB checksum if ALL CP bits are set or ALL CP bits are clear.
						checksum = 0; // don't include memory
						for (int idx = 0; idx < DevFile.PartsList[ActivePart].UserIDWords; idx++)
						{ // add UserIDs to checksum
							uint memWord = DeviceBuffers.UserIDs[idx];
							checksum += (memWord & 0x000000FF);
							checksum += ((memWord >> 8) & 0x000000FF);
						}
					}
					
					// config words
					for (int idx = 0; idx < DevFile.PartsList[ActivePart].ConfigWords; idx++)
					{
						uint memWord;
						if (idx < 9)
                        {
							memWord = (DeviceBuffers.ConfigWords[idx] & DevFile.PartsList[ActivePart].ConfigMasks[idx]);
						}
						else
                        {
							memWord = DeviceBuffers.ConfigWords[idx];
						}
						checksum += (memWord & 0x000000FF);
						checksum += ((memWord >> 8) & 0x000000FF);
					}
				}
				return (checksum & 0xFFFF);
			}

		}

		public static void ResetBuffers()
		{
			DeviceBuffers = new DeviceData(DevFile.PartsList[ActivePart].ProgramMem,
											DevFile.PartsList[ActivePart].EEMem,
											DevFile.PartsList[ActivePart].ConfigWords,
											DevFile.PartsList[ActivePart].UserIDWords,
											DevFile.Families[GetActiveFamily()].BlankValue,
											DevFile.Families[GetActiveFamily()].EEMemAddressIncrement,
											DevFile.Families[GetActiveFamily()].UserIDBytes,
											DevFile.PartsList[ActivePart].ConfigBlank,
											DevFile.PartsList[ActivePart].ConfigMasks[KONST.OSCCAL_MASK]);
		}

		public static DeviceData CloneBuffers(DeviceData copyFrom)
		{
			DeviceData newBuffers = new DeviceData(DevFile.PartsList[ActivePart].ProgramMem,
											DevFile.PartsList[ActivePart].EEMem,
											DevFile.PartsList[ActivePart].ConfigWords,
											DevFile.PartsList[ActivePart].UserIDWords,
											DevFile.Families[GetActiveFamily()].BlankValue,
											DevFile.Families[GetActiveFamily()].EEMemAddressIncrement,
											DevFile.Families[GetActiveFamily()].UserIDBytes,
											DevFile.PartsList[ActivePart].ConfigBlank,
											DevFile.PartsList[ActivePart].ConfigMasks[KONST.OSCCAL_MASK]);

			// clone all the data
			for (int i = 0; i < copyFrom.ProgramMemory.Length; i++)
			{
				newBuffers.ProgramMemory[i] = copyFrom.ProgramMemory[i];
			}
			for (int i = 0; i < copyFrom.EEPromMemory.Length; i++)
			{
				newBuffers.EEPromMemory[i] = copyFrom.EEPromMemory[i];
			}
			for (int i = 0; i < copyFrom.ConfigWords.Length; i++)
			{
				newBuffers.ConfigWords[i] = copyFrom.ConfigWords[i];
			}
			for (int i = 0; i < copyFrom.UserIDs.Length; i++)
			{
				newBuffers.UserIDs[i] = copyFrom.UserIDs[i];
			}
			newBuffers.OSCCAL = copyFrom.OSCCAL;
			newBuffers.OSCCAL = copyFrom.BandGap;

			return newBuffers;
		}

		public static void PrepNewPart(bool resetBuffers)
		{
			if (resetBuffers)
				ResetBuffers();
			// Set VPP voltage by family
			float vpp = DevFile.Families[GetActiveFamily()].Vpp;
			if ((vpp < 1) || (lvpEnabled && (DevFile.PartsList[ActivePart].LVPScript > 0)))
			{ // When nominally zero, use VDD voltage
				//UNLESS it's not an LVP script but a HV script (PIC24F-KA-)
				if (lvpEnabled && (DevFile.PartsList[ActivePart].LVPScript > 0))
				{
					string scriptname = DevFile.Scripts[DevFile.PartsList[ActivePart].LVPScript - 1].ScriptName;
					scriptname = scriptname.Substring(scriptname.Length - 2);
					if (scriptname == "HV")
					{
						// the VPP voltage value is the 2nd script element in 100mV increments.
						vpp = (float)DevFile.Scripts[DevFile.PartsList[ActivePart].LVPScript - 1].Script[1] / 10F;
						SetVppVoltage(vpp, 0.7F);
					}
					else
					{
						SetVppVoltage(vddLastSet, 0.7F);
					}
				}
				else
				{
					SetVppVoltage(vddLastSet, 0.7F);
				}
			}
			else
			{
				SetVppVoltage(vpp, 0.7F);
			}
			downloadPartScripts(GetActiveFamily());
		}

		public static uint ReadDebugVector()
		{
			RunScript(KONST.PROG_ENTRY, 1);
			ExecuteScript(DevFile.PartsList[ActivePart].DebugReadVectorScript);
			UploadData();
			RunScript(KONST.PROG_EXIT, 1);
			int configWords = 2;
			int bufferIndex = 2;                    // report starts on index 1, which is #bytes uploaded.
			uint returnWords = 0;
			for (int word = 0; word < configWords; word++)
			{
				uint config = (uint)Usb_read_array[bufferIndex++];
				config |= (uint)Usb_read_array[bufferIndex++] << 8;
				//$$$Bequest333 and JAKA
				if (DevFile.Families[GetActiveFamily()].FamilyName == "Midrange/1.8V Min MSB1st" ||
					DevFile.Families[GetActiveFamily()].FamilyName == "PIC18/PIC18F MSB1st")
				{
					config = reverse16Bits(config);
				}
				//$$$
				if (DevFile.Families[GetActiveFamily()].ProgMemShift > 0)
				{
					config = (config >> 1) & DevFile.Families[GetActiveFamily()].BlankValue;
				}
				if (word == 0)
					returnWords = config;
				else
					returnWords += (config << 16);
			}

			return returnWords;
		}

		public static void WriteDebugVector(uint debugWords)
		{
			int configWords = 2;
			byte[] configBuffer = new byte[4];

			RunScript(KONST.PROG_ENTRY, 1);

			for (int i = 0, j = 0; i < configWords; i++)
			{
				uint configWord = 0;
				if (i == 0)
					configWord = (debugWords & 0xFFFF);
				else
					configWord = (debugWords >> 16);
				if (DevFile.Families[GetActiveFamily()].ProgMemShift > 0)
				{
					configWord = configWord << 1;
				}
				//$$$Bequest333 and JAKA
				if (DevFile.Families[GetActiveFamily()].FamilyName == "Midrange/1.8V Min MSB1st" ||
					DevFile.Families[GetActiveFamily()].FamilyName == "PIC18/PIC18F MSB1st")
				{
					configWord = reverse16Bits(configWord);
				}
				//$$$
				configBuffer[j++] = (byte)(configWord & 0xFF);
				configBuffer[j++] = (byte)((configWord >> 8) & 0xFF);
			}
			DataClrAndDownload(configBuffer, 0);
			ExecuteScript(DevFile.PartsList[ActivePart].DebugWriteVectorScript);
			RunScript(KONST.PROG_EXIT, 1);

		}

		public static bool ReadPICkitVoltages(ref float vdd, ref float vpp)
		{
			byte[] commandArray = new byte[1];
			commandArray[0] = KONST.READ_VOLTAGES;
			if (writeUSB(commandArray))
			{
				if (readUSB())
				{
					if (!isPK3)
					{
						float valueADC = (float)((Usb_read_array[2] * 256) + Usb_read_array[1]);
						vdd = (valueADC / 65536) * 5.0F;
						valueADC = (float)((Usb_read_array[4] * 256) + Usb_read_array[3]);
						vpp = (valueADC / 65536) * 13.7F;
					}
					else
					{
						//TODO if firmware is changed to handle more accurate voltage steps, change this too
						vpp = (float)((Usb_read_array[0] * 256) + Usb_read_array[1]) * .125F;
						vdd = (float)((Usb_read_array[2] * 256) + Usb_read_array[3]) * .125F;
					}
					return true;
				}
			}
			return false;
		}

		public static bool SetVppVoltage(float voltage, float threshold)
		{
			if (!isPK3)
			{
				byte ccpValue = 0x40;
				byte vppADC = (byte)(voltage * 18.61F);
				byte vFault = (byte)(threshold * voltage * 18.61F);

				byte[] commandArray = new byte[4];
				commandArray[0] = KONST.SETVPP;
				commandArray[1] = ccpValue;
				commandArray[2] = vppADC;
				commandArray[3] = vFault;

				return writeUSB(commandArray);
			}
			else
			{
				//TODO if firmware is changed to handle more accurate voltage steps, change this too
				ushort vppValue = (ushort)(voltage / 0.125F);

				byte[] commandArray = new byte[3];
				commandArray[0] = KONST.SETVPP;
				commandArray[1] = (byte)(vppValue & 0xFF);
				commandArray[2] = (byte)(vppValue / 256);

				return writeUSB(commandArray);
			}
		}


		public static bool SendScript(byte[] script)
		{
			int scriptLength = script.Length;

			byte[] commandArray = new byte[2 + scriptLength];
			commandArray[0] = KONST.EXECUTE_SCRIPT;
			commandArray[1] = (byte)scriptLength;
			for (int n = 0; n < scriptLength; n++)
			{
				commandArray[2 + n] = script[n];
			}
			return writeUSB(commandArray);
		}


		// ================================== PRIVATE METHODS ========================================
		//$$$Bequest333
		private static uint reverse16Bits(uint v)
		{
			v = ((v & 0x0000ff00) >> 8) | ((v & 0x000000ff) << 8);
			v = ((v & 0x0000f0f0) >> 4) | ((v & 0x00000f0f) << 4);
			v = ((v & 0x0000cccc) >> 2) | ((v & 0x00003333) << 2);
			v = ((v & 0x0000aaaa) >> 1) | ((v & 0x00005555) << 1);
			return v;
		}
		//$$$

		// JAKA:
		private static uint swap2Bytes(uint v)
		{
			return ((v << 8) & 0xff00) | ((v >> 8) & 0x00ff);
		}

		private static uint reverse8Bits(uint b)
		{
			b = ((b & 0xF0) >> 4) | ((b & 0x0F) << 4);
			b = ((b & 0xCC) >> 2) | ((b & 0x33) << 2);
			b = ((b & 0xAA) >> 1) | ((b & 0x55) << 1);
			return b;
		}

		// end JAKA


		private static ushort readPkStatus()
		{
			byte[] commandArray = new byte[1];
			commandArray[0] = KONST.READ_STATUS;
			if (writeUSB(commandArray))
			{
				if (readUSB())
				{
					//if (vddErrorDisabled)
					//	return (ushort)((Usb_read_array[2] * 256 + Usb_read_array[1]) & 0xFFEF);	// mask out vdd error bit
					//else
						return (ushort)(Usb_read_array[2] * 256 + Usb_read_array[1]);
				}
				return 0xFFFF;
			}

			return 0xFFFF;
		}

		public static bool writeUSB(byte[] commandList)
		{
			if (wrhEventObject == IntPtr.Zero)
			{
				wrhEventObject = USB.CreateEvent
					(IntPtr.Zero,
					true,
					true,
					"");

				//Set the members of the overlapped structure.
				HIDWrOverlapped.hEvent = wrhEventObject;
				HIDWrOverlapped.Offset = 0;
				HIDWrOverlapped.OffsetHigh = 0;
			}
			uint Result;
			bool retVal = false;

			int bytesWritten = 0;

			Usb_write_array[0] = 0;                         // first byte must always be zero.        
			for (int index = 1; index < Usb_write_array.Length; index++)
			{
				Usb_write_array[index] = KONST.END_OF_BUFFER;              // init array to all END_OF_BUFFER cmds.
			}
			Array.Copy(commandList, 0, Usb_write_array, 1, commandList.Length);
			Result = USB.WriteFile(usbWriteHandle, Usb_write_array, Usb_write_array.Length, ref bytesWritten, ref HIDWrOverlapped);

			Result = USB.WaitForSingleObject(wrhEventObject, 1000);

			switch (Result)
			{
				case KONST.WAIT_OBJECT_0:
					{
						retVal = true;
						break;
					}
				case KONST.WAIT_TIMEOUT:
					{
						//Cancel the Read operation.
						//API call: CancelIo
						//Cancels the ReadFile
						//		Requires the device handle.
						//		Returns non-zero on success.
						

						Result = USB.CancelIo(usbWriteHandle);
						retVal = false;
						//A timeout may mean that the device has been removed.
						//Close the device handles and set DeviceDetected = False
						//so the next access attempt will search for the device.
						break;
					}
				default:
					{
						break;
					}
			}
			
			USB.ResetEvent(wrhEventObject);
			return retVal;
		}

		// Simiar to writeUSB but uses the format expected from an MPLAB host with the length at the end of the USB buffer
		public static bool writeUSB_MPLAB(byte[] commandList)
		{
			int bytesWritten = 0;
			int commandLength = commandList.Length;
			uint Result;
			bool retVal = false;

			if (wrhEventObject == IntPtr.Zero)
			{
				wrhEventObject = USB.CreateEvent
					(IntPtr.Zero,
					true,
					true,
					"");

				//Set the members of the overlapped structure.
				HIDWrOverlapped.hEvent = wrhEventObject;
				HIDWrOverlapped.Offset = 0;
				HIDWrOverlapped.OffsetHigh = 0;
			}

			Usb_write_array[0] = 0;                         // first byte must always be zero.        
			for (int index = 1; index < Usb_write_array.Length; index++)
			{
				Usb_write_array[index] = KONST.END_OF_BUFFER;              // init array to all END_OF_BUFFER cmds.
			}
			Array.Copy(commandList, 0, Usb_write_array, 1, commandList.Length);

			// Emded the length as expected from an MPLAB host
			Usb_write_array[61] = (byte)(commandLength & 0xFF);
			Usb_write_array[62] = (byte)((commandLength >> 8) & 0xFF);
			Usb_write_array[63] = (byte)((commandLength >> 16) & 0xFF);
			Usb_write_array[64] = (byte)((commandLength >> 24) & 0xFF);

			Result = USB.WriteFile(usbWriteHandle, Usb_write_array, Usb_write_array.Length, ref bytesWritten, ref HIDWrOverlapped);

			Result = USB.WaitForSingleObject(wrhEventObject, 1000);

			switch (Result)
			{
				case KONST.WAIT_OBJECT_0:
					{
						retVal = true;
						break;
					}
				case KONST.WAIT_TIMEOUT:
					{
						//Cancel the Read operation.
						//API call: CancelIo
						//Cancels the ReadFile
						//		Requires the device handle.
						//		Returns non-zero on success.


						Result = USB.CancelIo(usbWriteHandle);
						retVal = false;
						break;
					}
				default:
					{
						break;
					}
			}

			USB.ResetEvent(wrhEventObject);
			return retVal;
		}


		public static bool readUSB()
		{
			int bytesRead = 0;
			uint Result;
			bool retVal = false;

			if (hEventObject == IntPtr.Zero)
			{
				hEventObject = USB.CreateEvent
					(IntPtr.Zero,
					true,
					true,
					"");

				//Set the members of the overlapped structure.
				HIDOverlapped.hEvent = hEventObject;
				HIDOverlapped.Offset = 0;
				HIDOverlapped.OffsetHigh = 0;
			}

			if (LearnMode)
				return true;

			Result = USB.ReadFile(usbReadHandle, Usb_read_array, Usb_read_array.Length, ref bytesRead, ref HIDOverlapped);

			Result = USB.WaitForSingleObject(hEventObject, 1000);
			
			switch (Result)
			{
				case KONST.WAIT_OBJECT_0:
					{
						retVal = true;
						break;
					}
				case KONST.WAIT_TIMEOUT:
					{
						//Cancel the Read operation.
						//API call: CancelIo
						//Cancels the ReadFile
						//		Requires the device handle.
						//		Returns non-zero on success.
						

						Result = USB.CancelIo(usbReadHandle);
						retVal = false;
						break;
					}
				default:
					{
						break;
					}
			}
			
			USB.ResetEvent(hEventObject);
			return retVal;
		}

		public static bool VerifyDeviceID(bool resetOnNoDevice, bool keepVddOn)
		{
			// NOTE: the interface portion should ensure that self-powered targets
			// are detected before calling this function.

			// Set VPP voltage by family
			float vpp = DevFile.Families[GetActiveFamily()].Vpp;
			if ((vpp < 1) || (lvpEnabled && (DevFile.PartsList[ActivePart].LVPScript > 0)))
			{ // When nominally zero, use VDD voltage
				//UNLESS it's not an LVP script but a HV script (PIC24F-KA-)
				if (lvpEnabled && (DevFile.PartsList[ActivePart].LVPScript > 0))
				{
					string scriptname = DevFile.Scripts[DevFile.PartsList[ActivePart].LVPScript - 1].ScriptName;
					scriptname = scriptname.Substring(scriptname.Length - 2);
					if (scriptname == "HV")
					{
						// the VPP voltage value is the 2nd script element in 100mV increments.
						vpp = (float)DevFile.Scripts[DevFile.PartsList[ActivePart].LVPScript - 1].Script[1] / 10F;
						SetVppVoltage(vpp, 0.7F);
					}
					else
					{
						SetVppVoltage(vddLastSet, 0.7F);
					}
				}
				else
				{
					SetVppVoltage(vddLastSet, 0.7F);
				}
			}
			else
			{
				SetVppVoltage(vpp, 0.7F);
			}

			// Turn on Vdd (if self-powered, just turns off ground resistor)
			SetMCLRTemp(true);     // assert /MCLR to prevent code execution before programming mode entered.
			VddOn();

			// use direct execute scripts when checking for a part
			if (lvpEnabled && (DevFile.PartsList[ActivePart].LVPScript > 0))
			{
				ExecuteScript(DevFile.PartsList[ActivePart].LVPScript);
			}
			else if (vppFirstEnabled && (DevFile.Families[GetActiveFamily()].ProgEntryVPPScript > 0))
			{
				ExecuteScript(DevFile.Families[GetActiveFamily()].ProgEntryVPPScript);
			}
			else
			{
				ExecuteScript(DevFile.Families[GetActiveFamily()].ProgEntryScript);
			}
			ExecuteScript(DevFile.Families[GetActiveFamily()].ReadDevIDScript);
			UploadData();
			ExecuteScript(DevFile.Families[GetActiveFamily()].ProgExitScript);

			// Turn off Vdd (if PICkit-powered, turns on ground resistor)
			if (!keepVddOn)
			{ // don't want it off when user wants PICkit 2 VDD "ON"
				VddOff();
			}

			if (!assertMCLR)
			{
				HoldMCLR(false);
			}

			// NOTE: parts that only return 2 bytes for DevID will have junk in upper word.  This is OK - it gets masked off
			uint deviceID = (uint)(Usb_read_array[5] * 0x1000000 + Usb_read_array[4] * 0x10000 + Usb_read_array[3] * 256 + Usb_read_array[2]);
			for (int shift = 0; shift < DevFile.Families[GetActiveFamily()].ProgMemShift; shift++)
			{
				deviceID >>= 1;         // midrange/baseline part results must be shifted by 1
			}
			if (Usb_read_array[1] == 4) // 16-bit/32-bit parts have Rev in separate word
			{
				LastDeviceRev = (int)(Usb_read_array[5] * 256 + Usb_read_array[4]);
				if (DevFile.Families[GetActiveFamily()].BlankValue == 0xFFFFFFFF) // PIC32
					LastDeviceRev >>= 12;    // 6.6.2022 JAKA / Was right shift by 4
			}		
			else
				LastDeviceRev = (int)(deviceID & ~DevFile.Families[GetActiveFamily()].DeviceIDMask);
			LastDeviceRev &= 0xFFFF; // make sure to clear upper word.
			LastDeviceRev &= (int)DevFile.Families[GetActiveFamily()].BlankValue;
			deviceID &= DevFile.Families[GetActiveFamily()].DeviceIDMask; // mask off version bits.

			if (FamilyIsEEPROM()
				&& (DevFile.Families[GetActiveFamily()].DeviceIDMask == 0xffffff))  // Reverse SPI FLASH JEDEC DEVID and MFG ID byte order for better readability
			//	&& (DevFile.PartsList[ActivePart].ConfigMasks[KONST.PROTOCOL_CFG] == KONST.SPI_FLASH_BUS))  // Reverse SPI FLASH JEDEC DEVID and MFG ID byte order for better readability
			{
				deviceID = ((deviceID << 16) & 0x00ff0000) | (deviceID & 0x0000ff00) | ((deviceID >> 16) & 0x000000ff);
			}


			// JAKA
			if (DevFile.Families[GetActiveFamily()].FamilyName == "Midrange/1.8V Min MSB1st")
			{
				deviceID = reverse16Bits(deviceID);
				deviceID >>= 2;
				LastDeviceRev = (int)reverse16Bits((uint)(LastDeviceRev));
				LastDeviceRev >>= 2;
			}
			if (DevFile.Families[GetActiveFamily()].FamilyName == "PIC18/PIC18F MSB1st")
			{
				deviceID = reverse16Bits(deviceID);
				LastDeviceRev = (int)reverse16Bits((uint)(LastDeviceRev));
			}
			// END JAKA

			LastDeviceID = deviceID;

			if (deviceID != DevFile.PartsList[ActivePart].DeviceID)
			{
				return false;
			}

			// Get OSCCAL if exists
			if (DevFile.PartsList[ActivePart].OSSCALSave)
			{
				VddOn();
				ReadOSSCAL();
			}
			if (DevFile.PartsList[ActivePart].BandGapMask > 0)
			{
				VddOn();
				ReadBandGap();
			}
			if (!keepVddOn)
			{
				VddOff();
			}

			return true;
		}


		private static bool searchDevice(int familyIndex, bool resetOnNoDevice, bool keepVddOn)
		{
			int lastPart = ActivePart;  // remember the current part
			if (ActivePart != 0)
			{
				lastFoundPart = ActivePart;
			}

			// NOTE: the interface portion should ensure that self-powered targets
			// are detected before calling this function.

			// Set VPP voltage by family
			float vpp = DevFile.Families[familyIndex].Vpp;
			if ((vpp < 1) || (lvpEnabled && (DevFile.PartsList[ActivePart].LVPScript > 0)))
			{ // When nominally zero, use VDD voltage
				//UNLESS it's not an LVP script but a HV script (PIC24F-KA-)
				if (lvpEnabled && (DevFile.PartsList[ActivePart].LVPScript > 0))
				{
					string scriptname = DevFile.Scripts[DevFile.PartsList[ActivePart].LVPScript - 1].ScriptName;
					scriptname = scriptname.Substring(scriptname.Length - 2);
					if (scriptname == "HV")
					{
						// the VPP voltage value is the 2nd script element in 100mV increments.
						vpp = (float)DevFile.Scripts[DevFile.PartsList[ActivePart].LVPScript - 1].Script[1] / 10F;
						SetVppVoltage(vpp, 0.7F);
					}
					else
					{
						SetVppVoltage(vddLastSet, 0.7F);
					}
				}
				else
				{
					SetVppVoltage(vddLastSet, 0.7F);
				}
			}
			else
			{
				SetVppVoltage(vpp, 0.7F);
			}

			// Turn on Vdd (if self-powered, just turns off ground resistor)
			SetMCLRTemp(true);     // assert /MCLR to prevent code execution before programming mode entered.
			VddOn();

			// use direct execute scripts when checking for a part
			if (lvpEnabled && (DevFile.PartsList[ActivePart].LVPScript > 0))
			{
				ExecuteScript(DevFile.PartsList[ActivePart].LVPScript);
			}
			else if (vppFirstEnabled && (DevFile.Families[familyIndex].ProgEntryVPPScript > 0))
			{
				ExecuteScript(DevFile.Families[familyIndex].ProgEntryVPPScript);
			}
			else
			{
				ExecuteScript(DevFile.Families[familyIndex].ProgEntryScript);
			}
			ExecuteScript(DevFile.Families[familyIndex].ReadDevIDScript);
			UploadData();
			ExecuteScript(DevFile.Families[familyIndex].ProgExitScript);

			// Turn off Vdd (if PICkit-powered, turns on ground resistor)
			if (!keepVddOn)
			{ // don't want it off when user wants PICkit 2 VDD "ON"
				VddOff();
			}

			if (!assertMCLR)
			{
				HoldMCLR(false);
			}

			// NOTE: parts that only return 2 bytes for DevID will have junk in upper word.  This is OK - it gets masked off
			uint deviceID = (uint)(Usb_read_array[5] * 0x1000000 + Usb_read_array[4] * 0x10000 + Usb_read_array[3] * 256 + Usb_read_array[2]);
			for (int shift = 0; shift < DevFile.Families[familyIndex].ProgMemShift; shift++)
			{
				deviceID >>= 1;         // midrange/baseline part results must be shifted by 1
			}
			if (Usb_read_array[1] == 4) // 16-bit/32-bit parts have Rev in separate word (also newer 8-bit devices - JAKA)
			{
				LastDeviceRev = (int)(Usb_read_array[5] * 256 + Usb_read_array[4]);
				//LastDeviceRev = 0xffff;
				if (DevFile.Families[familyIndex].BlankValue == 0xFFFFFFFF) // PIC32
					LastDeviceRev >>= 12;    // 6.6.2022 JAKA / Was right shift by 4
			}
			else
				LastDeviceRev = (int)(deviceID & ~DevFile.Families[familyIndex].DeviceIDMask);
			LastDeviceRev &= 0xFFFF; // make sure to clear upper word.
			LastDeviceRev &= (int)DevFile.Families[familyIndex].BlankValue;
			deviceID &= DevFile.Families[familyIndex].DeviceIDMask; // mask off version bits.

			int maxLength = DevFile.Families[familyIndex].FamilyName.Length;
			if (maxLength > 6)
			{
				maxLength = 6;
			}
			if ((DevFile.Families[familyIndex].FamilyName.Substring(0, maxLength) == "EEPROM")
				&& (DevFile.Families[familyIndex].DeviceIDMask == 0xffffff))  // Reverse SPI FLASH JEDEC DEVID and MFG ID byte order for better readability
			{
				deviceID = ((deviceID << 16) & 0x00ff0000) | (deviceID & 0x0000ff00) | ((deviceID >> 16) & 0x000000ff);
			}

			// JAKA
			if (DevFile.Families[familyIndex].FamilyName == "Midrange/1.8V Min MSB1st")
			{
				deviceID = reverse16Bits(deviceID);
				deviceID >>= 2;
				LastDeviceRev = (int)reverse16Bits((uint)(LastDeviceRev));
				LastDeviceRev >>= 1; // Why this has to be >>1 to get correct rev??? is there something else wrong?
			}
			if (DevFile.Families[familyIndex].FamilyName == "PIC18/PIC18F MSB1st")
			{
				deviceID = reverse16Bits(deviceID);
				LastDeviceRev = (int)reverse16Bits((uint)(LastDeviceRev));

			}
			// END JAKA
			LastDeviceID = deviceID;


			// Search through the device file to see if we find the part
			ActivePart = 0; // no device is default.
			for (int partEntry = 0; partEntry < DevFile.PartsList.Length; partEntry++)
			{
				if (DevFile.PartsList[partEntry].Family == familyIndex)
				{ // don't check device ID if in a different family
					if (DevFile.PartsList[partEntry].DeviceID == deviceID)
					{
						ActivePart = partEntry;
						break;  // found a match - get out of the loop.
					}
				}
			}

			if (ActivePart == 0) // not a known part
			{   // still need a buffer object in existance.
				if (lastPart != 0)
				{
					DevFile.PartsList[ActivePart] = DevFile.PartsList[lastPart];
					DevFile.PartsList[ActivePart].DeviceID = 0;
					DevFile.PartsList[ActivePart].PartName = "Unsupported Part";
				}
				if (resetOnNoDevice)
				{
					ResetBuffers();
				}
				return false;   // we're done
			}

			//if ((ActivePart == lastPart) && (scriptBufferChecksum == getScriptBufferChecksum())) 
			//{// same part we have been using (ensure scipt buffer hasn't been corrupted.)
			//    return true;    // don't need to download scripts as they should already be there.
			//}

			if ((ActivePart == lastFoundPart) && (scriptBufferChecksum != 0)
						&& (scriptBufferChecksum == getScriptBufferChecksum()))
			{// same as the last part we were connected to.
				return true;    // don't need to download scripts as they should already be there.
			}

			// Getting here means we've found a part, but it's a new one so we need to download scripts
			downloadPartScripts(familyIndex);

			// create a new set of device buffers
			// If only need to redownload scripts, don't clear buffer.
			if (ActivePart != lastFoundPart)
			{
				ResetBuffers();
			}

			// Get OSCCAL if exists
			if (DevFile.PartsList[ActivePart].OSSCALSave)
			{
				VddOn();
				ReadOSSCAL();
			}
			if (DevFile.PartsList[ActivePart].BandGapMask > 0)
			{
				VddOn();
				ReadBandGap();
			}
			if (!keepVddOn)
			{
				VddOff();
			}

			return true;

		}


		private static void downloadPartScripts(int familyIndex)
		{
			byte[] commandArray = new byte[1];
			commandArray[0] = KONST.CLR_SCRIPT_BUFFER;      // clear script buffer- we're loading new scripts
			bool result = writeUSB(commandArray);

			// clear the script redirect table
			for (int i = 0; i < scriptRedirectTable.Length; i++)
			{
				scriptRedirectTable[i].redirectToScriptLocation = 0;
				scriptRedirectTable[i].deviceFileScriptNumber = 0;
			}

			// program entry
			if (DevFile.Families[familyIndex].ProgEntryScript != 0) // don't download non-existant scripts
			{
				if (lvpEnabled && (DevFile.PartsList[ActivePart].LVPScript > 0))
				{
					downloadScript(KONST.PROG_ENTRY, DevFile.PartsList[ActivePart].LVPScript);
				}
				else if (vppFirstEnabled && (DevFile.Families[familyIndex].ProgEntryVPPScript != 0))
				{ // download VPP first program mode entry
					downloadScript(KONST.PROG_ENTRY, DevFile.Families[familyIndex].ProgEntryVPPScript);
				}
				else
				{ // standard program entry
					downloadScript(KONST.PROG_ENTRY, DevFile.Families[familyIndex].ProgEntryScript);
				}
			}
			// program exit
			if (DevFile.Families[familyIndex].ProgExitScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.PROG_EXIT, DevFile.Families[familyIndex].ProgExitScript);
			}
			// read device id
			if (DevFile.Families[familyIndex].ReadDevIDScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.RD_DEVID, DevFile.Families[familyIndex].ReadDevIDScript);
			}
			// read program memory
			if (DevFile.PartsList[ActivePart].ProgMemRdScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.PROGMEM_RD, DevFile.PartsList[ActivePart].ProgMemRdScript);
			}
			// chip erase prep
			if (DevFile.PartsList[ActivePart].ChipErasePrepScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.ERASE_CHIP_PREP, DevFile.PartsList[ActivePart].ChipErasePrepScript);
			}
			// set program memory address
			if (DevFile.PartsList[ActivePart].ProgMemAddrSetScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.PROGMEM_ADDRSET, DevFile.PartsList[ActivePart].ProgMemAddrSetScript);
			}
			// prepare for program memory write
			if (DevFile.PartsList[ActivePart].ProgMemWrPrepScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.PROGMEM_WR_PREP, DevFile.PartsList[ActivePart].ProgMemWrPrepScript);
			}
			// program memory write                 
			if (DevFile.PartsList[ActivePart].ProgMemWrScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.PROGMEM_WR, DevFile.PartsList[ActivePart].ProgMemWrScript);
			}
			// prep for ee read               
			if (DevFile.PartsList[ActivePart].EERdPrepScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.EE_RD_PREP, DevFile.PartsList[ActivePart].EERdPrepScript);
			}
			// ee read               
			if (DevFile.PartsList[ActivePart].EERdScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.EE_RD, DevFile.PartsList[ActivePart].EERdScript);
			}
			// prep for ee write               
			if (DevFile.PartsList[ActivePart].EEWrPrepScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.EE_WR_PREP, DevFile.PartsList[ActivePart].EEWrPrepScript);
			}
			// ee write               
			if (DevFile.PartsList[ActivePart].EEWrScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.EE_WR, DevFile.PartsList[ActivePart].EEWrScript);
			}
			// prep for config read       
			if (DevFile.PartsList[ActivePart].ConfigRdPrepScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.CONFIG_RD_PREP, DevFile.PartsList[ActivePart].ConfigRdPrepScript);
			}
			// config read       
			if (DevFile.PartsList[ActivePart].ConfigRdScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.CONFIG_RD, DevFile.PartsList[ActivePart].ConfigRdScript);
			}
			// prep for config write       
			if (DevFile.PartsList[ActivePart].ConfigWrPrepScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.CONFIG_WR_PREP, DevFile.PartsList[ActivePart].ConfigWrPrepScript);
			}
			// config write       
			if (DevFile.PartsList[ActivePart].ConfigWrScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.CONFIG_WR, DevFile.PartsList[ActivePart].ConfigWrScript);
			}
			// prep for user id read      
			if (DevFile.PartsList[ActivePart].UserIDRdPrepScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.USERID_RD_PREP, DevFile.PartsList[ActivePart].UserIDRdPrepScript);
			}
			// user id read      
			if (DevFile.PartsList[ActivePart].UserIDRdScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.USERID_RD, DevFile.PartsList[ActivePart].UserIDRdScript);
			}
			// prep for user id write      
			if (DevFile.PartsList[ActivePart].UserIDWrPrepScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.USERID_WR_PREP, DevFile.PartsList[ActivePart].UserIDWrPrepScript);
			}
			// user id write      
			if (DevFile.PartsList[ActivePart].UserIDWrScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.USERID_WR, DevFile.PartsList[ActivePart].UserIDWrScript);
			}
			// read osscal      
			if (DevFile.PartsList[ActivePart].OSCCALRdScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.OSSCAL_RD, DevFile.PartsList[ActivePart].OSCCALRdScript);
			}
			// write osscal      
			if (DevFile.PartsList[ActivePart].OSCCALWrScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.OSSCAL_WR, DevFile.PartsList[ActivePart].OSCCALWrScript);
			}
			// chip erase      
			if (DevFile.PartsList[ActivePart].ChipEraseScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.ERASE_CHIP, DevFile.PartsList[ActivePart].ChipEraseScript);
			}
			// program memory erase 
			if (DevFile.PartsList[ActivePart].ProgMemEraseScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.ERASE_PROGMEM, DevFile.PartsList[ActivePart].ProgMemEraseScript);
			}
			// ee erase 
			if (DevFile.PartsList[ActivePart].EEMemEraseScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.ERASE_EE, DevFile.PartsList[ActivePart].EEMemEraseScript);
			}
			// row erase
			if (DevFile.PartsList[ActivePart].DebugRowEraseScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.ROW_ERASE, DevFile.PartsList[ActivePart].DebugRowEraseScript);
			}
			// Test Memory Read
			if (DevFile.PartsList[ActivePart].TestMemoryRdScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.TESTMEM_RD, DevFile.PartsList[ActivePart].TestMemoryRdScript);
			}
			// EE Row Erase
			if (DevFile.PartsList[ActivePart].EERowEraseScript != 0) // don't download non-existant scripts
			{
				downloadScript(KONST.EEROW_ERASE, DevFile.PartsList[ActivePart].EERowEraseScript);
			}

			// get script buffer checksum
			scriptBufferChecksum = getScriptBufferChecksum();
		}

		private static uint getScriptBufferChecksum()
		{
			if (LearnMode)
				return 0;

			byte[] commandArray = new byte[1];
			commandArray[0] = KONST.SCRIPT_BUFFER_CHKSUM;
			if (writeUSB(commandArray))
			{
				if (readUSB())
				{
					uint checksum = (uint)Usb_read_array[4];
					checksum += (uint)(Usb_read_array[3] << 8);
					checksum += (uint)(Usb_read_array[2] << 16);
					checksum += (uint)(Usb_read_array[1] << 24);

					return checksum;
				}
				return 0;
			}
			return 0;
		}

		private static bool downloadScript(byte scriptBufferLocation, int scriptArrayIndex)
		{
			// see if we've already downloaded the script.  Some devices use the same script
			// for different functions.  Not downloading it several times saves space in the script buffer
			byte redirectTo = scriptBufferLocation;  // default doesn't redirect; calls itself
			for (byte i = 0; i < scriptRedirectTable.Length; i++)
			{
				if (scriptArrayIndex == scriptRedirectTable[i].deviceFileScriptNumber)
				{
					redirectTo = i; // redirect to this buffer location
					break;
				}
			}
			scriptRedirectTable[scriptBufferLocation].redirectToScriptLocation = redirectTo; // set redirection
			scriptRedirectTable[scriptBufferLocation].deviceFileScriptNumber = scriptArrayIndex;
			// note: since the FOR loop above always finds the first instance of a script, we don't have to
			// worry about redirecting to a redirect.
			if (scriptBufferLocation != redirectTo)
			{  // we're redirecting
				return true;  // we're all done
			}

			int scriptLength = DevFile.Scripts[--scriptArrayIndex].ScriptLength;

			byte[] commandArray = new byte[3 + scriptLength];
			commandArray[0] = KONST.DOWNLOAD_SCRIPT;
			commandArray[1] = scriptBufferLocation;
			commandArray[2] = (byte)scriptLength;
			for (int n = 0; n < scriptLength; n++)
			{
				ushort scriptEntry = DevFile.Scripts[scriptArrayIndex].Script[n];
				if (fastProgramming)
				{
					commandArray[3 + n] = (byte)scriptEntry;
				}
				else
				{
					if (scriptEntry == 0xAAE7)
					{ // delay short
						ushort nextEntry = (ushort)(DevFile.Scripts[scriptArrayIndex].Script[n + 1] & 0xFF);
						if ((nextEntry < 170) && (nextEntry != 0))
						{
							commandArray[3 + n++] = (byte)scriptEntry;
							byte delay = (byte)DevFile.Scripts[scriptArrayIndex].Script[n];
							commandArray[3 + n] = (byte)(delay + (delay / 2)); //1.5x delay   
							//commandArray[3 + n] = (byte)(2 * DevFile.Scripts[scriptArrayIndex].Script[n]);
						}
						else
						{
							commandArray[3 + n++] = KONST._DELAY_LONG;
							commandArray[3 + n] = 2;
						}
					}
					else if (scriptEntry == 0xAAE8)
					{ // delay long
						ushort nextEntry = (ushort)(DevFile.Scripts[scriptArrayIndex].Script[n + 1] & 0xFF);
						if ((nextEntry < 171) && (nextEntry != 0))
						{
							commandArray[3 + n++] = (byte)scriptEntry;
							byte delay = (byte)DevFile.Scripts[scriptArrayIndex].Script[n];
							commandArray[3 + n] = (byte)(delay + (delay / 2)); //1.5x delay
						}
						else
						{
							commandArray[3 + n++] = KONST._DELAY_LONG;
							commandArray[3 + n] = 0; // max out
						}
					}
					else
					{
						commandArray[3 + n] = (byte)scriptEntry;
					}
				}
			}
			return writeUSB(commandArray);
		}
	}
}
