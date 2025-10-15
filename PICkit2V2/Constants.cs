using System;
using System.Collections.Generic;
using System.Text;

namespace PICkit2V2
{
	public class Constants
	{
		// APPLICATION VERSION
		public const string AppVersion = "3.28.03";
		public const byte DevFileCompatLevel = 6;
		public const byte DevFileCompatLevelMin = 0;
		public const string UserGuideFileNamePK2 = "\\PICkit2 User Guide 51553E.pdf";
		public const string UserGuideFileNamePK3 = "\\PICkit 3 Programmer Application User's Guide 50002158a.pdf";

		// min firmware version Pk2
		public const byte FWVerMajorReq = 2;
		public const byte FWVerMinorReq = 32;
		public const byte FWVerDotReq = 2;
		public const string FWFileName = "PK2V023202.hex";

		// min firmware version PK2M
		public const byte FWVerMajorReqPK2M = 2;
		public const byte FWVerMinorReqPK2M = 32;
		public const byte FWVerDotReqPK2M = 2;
		public const string FWFileNamePK2M = "PK2MV023202.hex";

		// min firmware version Pk3 / PKOB
		public const byte FWVerMajorReqPk3 = 2;
		public const byte FWVerMinorReqPk3 = 32;
		public const byte FWVerDotReqPk3 = 2;
		public const string FWFileNamePk3 = "PK3OSV023202.hex";
		public const string BLFileNamePk3 = "PK3BLV011405.hex"; // matches the MPLAB bootlaoder version number
		public const string FWFileNamePkob = "PK3OSV023202.hex";

		public const uint PACKET_SIZE = 65; // 64 + leading 0
		public const uint USB_REPORTLENGTH = 64;
		//
		public const byte BIT_MASK_0 = 0x01;
		public const byte BIT_MASK_1 = 0x02;
		public const byte BIT_MASK_2 = 0x04;
		public const byte BIT_MASK_3 = 0x08;
		public const byte BIT_MASK_4 = 0x10;
		public const byte BIT_MASK_5 = 0x20;
		public const byte BIT_MASK_6 = 0x40;
		public const byte BIT_MASK_7 = 0x80;
		//
		public const ushort MChipVendorID = 0x04D8;
		public const ushort Pk2DeviceID = 0x0033;
		public const ushort Pk3DeviceID = 0x900A;
		public const ushort PkobDeviceID = 0x8107;		// PKOB3
		public const int Pk3MagicKey = 0x336B50;
		public const byte MPLAB_BOOTLOADERTYPE = 0x99;

		//
		public const ushort ConfigRows = 2;//2
		public const ushort ConfigColumns = 4;//4
		public const ushort MaxReadCfgMasks = 8;//8
		public const ushort NumConfigMasks = 9; // was 9, increased to 18 to support Q83/Q84 25.8.2021
												// Changed back to 9 17.9.2025. Masks/blanks > 9 handled separately.
		//
		public enum PICkit2USB
		{
			found,              // implies firmware version is good.
			notFound,
			writeError,
			readError,
			firmwareInvalid,
			bootloader,
			readwriteError,
			pk3mplab,            // a valid mplab ready pk3 that doesn't have scripting firmware
			firmwareOldversion
		};

		public enum PICkit2PWR
		{
			no_response,
			vdd_on,
			vdd_off,
			vdderror,
			vpperror,
			vddvpperrors,
			selfpowered,
			unpowered
		};

		public enum FileRead
		{
			success,
			failed,
			noconfig,
			partialcfg,
			largemem
		};

		public enum StatusColor
		{
			normal,
			green,
			yellow,
			red
		};

		public enum VddTargetSelect
		{
			auto,
			pickit2,
			target
		};

		public enum MPLABerrorcodes
		{
			Unknownerror,
			Bootloadernotfound
		};

		public const float VddThresholdForSelfPoweredTarget = 2.3F;
		public const bool NoMessage = false;
		public const bool ShowMessage = true;
		public const bool UpdateMemoryDisplays = true;
		public const bool DontUpdateMemDisplays = false;
		public const bool EnableMclrCheckBox = true;
		public const bool DontEnableMclrCheckBox = false;
		public const bool UpdateProtections = true;
		public const bool DontUpdateProtections = false;
		public const bool EraseEE = true;
		public const bool WriteEE = false;

		//
		public const int UploadBufferSize = 128;
		public const int DownLoadBufferSize = 256;
		//
		public const byte READFWFLASH = 1;
		public const byte WRITEFWFLASH = 2;
		public const byte ERASEFWFLASH = 3;
		public const byte READFWEEDATA = 4;
		public const byte WRITEFWEEDATA = 5;
		public const byte RESETFWDEVICE = 0xFF;
		//
		public const byte SWITCH_TO_BL = 0x24;
		public const byte TEST_BOOTLOADER = 0x2A;
		public const byte GETVERSIONS_MPLAB = 0x41;
		public const byte ENTER_BOOTLOADER = 0x42;
		public const byte NO_OPERATION = 0x5A;
		public const byte FIRMWARE_VERSION = 0x76;
		public const byte SETVDD = 0xA0;
		public const byte SETVPP = 0xA1;
		public const byte READ_STATUS = 0xA2;
		public const byte READ_VOLTAGES = 0xA3;
		public const byte DOWNLOAD_SCRIPT = 0xA4;
		public const byte RUN_SCRIPT = 0xA5;
		public const byte EXECUTE_SCRIPT = 0xA6;
		public const byte CLR_DOWNLOAD_BUFFER = 0xA7;
		public const byte DOWNLOAD_DATA = 0xA8;
		public const byte CLR_UPLOAD_BUFFER = 0xA9;
		public const byte UPLOAD_DATA = 0xAA;
		public const byte CLR_SCRIPT_BUFFER = 0xAB;
		public const byte UPLOAD_DATA_NOLEN = 0xAC;
		public const byte END_OF_BUFFER = 0xAD;
		public const byte RESET = 0xAE;
		public const byte SCRIPT_BUFFER_CHKSUM = 0xAF;
		public const byte SET_VOLTAGE_CALS = 0xB0;
		public const byte WR_INTERNAL_EE = 0xB1;
		public const byte RD_INTERNAL_EE = 0xB2;
		public const byte ENTER_UART_MODE = 0xB3;
		public const byte EXIT_UART_MODE = 0xB4;
		public const byte ENTER_LEARN_MODE = 0xB5;
		public const byte EXIT_LEARN_MODE = 0xB6;
		public const byte ENABLE_PK2GO_MODE = 0xB7;
		public const byte LOGIC_ANALYZER_GO = 0xB8;
		public const byte COPY_RAM_UPLOAD = 0xB9;
		// META COMMANDS
		public const byte MC_READ_OSCCAL = 0x80;
		public const byte MC_WRITE_OSCCAL = 0x81;
		public const byte MC_START_CHECKSUM = 0x82;
		public const byte MC_VERIFY_CHECKSUM = 0x83;
		public const byte MC_CHECK_DEVICE_ID = 0x84;
		public const byte MC_READ_BANDGAP = 0x85;
		public const byte MC_WRITE_CFG_BANDGAP = 0x86;
		public const byte MC_CHANGE_CHKSM_FRMT = 0x87;
		//
		public const byte _VDD_ON = 0xFF;
		public const byte _VDD_OFF = 0xFE;
		public const byte _VDD_GND_ON = 0xFD;
		public const byte _VDD_GND_OFF = 0xFC;
		public const byte _VPP_ON = 0xFB;
		public const byte _VPP_OFF = 0xFA;
		public const byte _VPP_PWM_ON = 0xF9;
		public const byte _VPP_PWM_OFF = 0xF8;
		public const byte _MCLR_GND_ON = 0xF7;
		public const byte _MCLR_GND_OFF = 0xF6;
		public const byte _BUSY_LED_ON = 0xF5;
		public const byte _BUSY_LED_OFF = 0xF4;
		public const byte _SET_ICSP_PINS = 0xF3;
		public const byte _WRITE_BYTE_LITERAL = 0xF2;
		public const byte _WRITE_BYTE_BUFFER = 0xF1;
		public const byte _READ_BYTE_BUFFER = 0xF0;
		public const byte _READ_BYTE = 0xEF;
		public const byte _WRITE_BITS_LITERAL = 0xEE;
		public const byte _WRITE_BITS_BUFFER = 0xED;
		public const byte _READ_BITS_BUFFER = 0xEC;
		public const byte _READ_BITS = 0xEB;
		public const byte _SET_ICSP_SPEED = 0xEA;
		public const byte _LOOP = 0xE9;
		public const byte _DELAY_LONG = 0xE8;
		public const byte _DELAY_SHORT = 0xE7;
		public const byte _IF_EQ_GOTO = 0xE6;
		public const byte _IF_GT_GOTO = 0xE5;
		public const byte _GOTO_INDEX = 0xE4;
		public const byte _EXIT_SCRIPT = 0xE3;
		public const byte _PEEK_SFR = 0xE2;
		public const byte _POKE_SFR = 0xE1;

		public const byte _ICDSLAVE_RX = 0xE0;
		public const byte _ICDSLAVE_TX_LIT = 0xDF;
		public const byte _ICDSLAVE_TX_BUF = 0xDE;
		public const byte _LOOPBUFFER = 0xDD;
		public const byte _ICSP_STATES_BUFFER = 0xDC;
		public const byte _POP_DOWNLOAD = 0xDB;
		public const byte _COREINST18 = 0xDA;
		public const byte _COREINST24 = 0xD9;
		public const byte _NOP24 = 0xD8;
		public const byte _VISI24 = 0xD7;
		public const byte _RD2_BYTE_BUFFER = 0xD6;
		public const byte _RD2_BITS_BUFFER = 0xD5;
		public const byte _WRITE_BUFWORD_W = 0xD4;
		public const byte _WRITE_BUFBYTE_W = 0xD3;
		public const byte _CONST_WRITE_DL = 0xD2;

		public const byte _WRITE_BITS_LIT_HLD = 0xD1;
		public const byte _WRITE_BITS_BUF_HLD = 0xD0;
		public const byte _SET_AUX = 0xCF;
		public const byte _AUX_STATE_BUFFER = 0xCE;
		public const byte _I2C_START = 0xCD;
		public const byte _I2C_STOP = 0xCC;
		public const byte _I2C_WR_BYTE_LIT = 0xCB;
		public const byte _I2C_WR_BYTE_BUF = 0xCA;
		public const byte _I2C_RD_BYTE_ACK = 0xC9;
		public const byte _I2C_RD_BYTE_NACK = 0xC8;
		public const byte _SPI_WR_BYTE_LIT = 0xC7;
		public const byte _SPI_WR_BYTE_BUF = 0xC6;
		public const byte _SPI_RD_BYTE_BUF = 0xC5;
		public const byte _SPI_RDWR_BYTE_LIT = 0xC4;
		public const byte _SPI_RDWR_BYTE_BUF = 0xC3;
		public const byte _ICDSLAVE_RX_BL = 0xC2;
		public const byte _ICDSLAVE_TX_LIT_BL = 0xC1;
		public const byte _ICDSLAVE_TX_BUF_BL = 0xC0;
		public const byte _MEASURE_PULSE = 0xBF;
		public const byte _UNIO_TX = 0xBE;
		public const byte _UNIO_TX_RX = 0xBD;
		public const byte _JT2_SETMODE = 0xBC;
		public const byte _JT2_SENDCMD = 0xBB;
		public const byte _JT2_XFERDATA8_LIT = 0xBA;
		public const byte _JT2_XFERDATA32_LIT = 0xB9;
		public const byte _JT2_XFRFASTDAT_LIT = 0xB8;
		public const byte _JT2_XFRFASTDAT_BUF = 0xB7;
		public const byte _JT2_XFERINST_BUF = 0xB6;
		public const byte _JT2_GET_PE_RESP = 0xB5;
		public const byte _JT2_WAIT_PE_RESP = 0xB4;
		//
		public const int SEARCH_ALL_FAMILIES = 0xFFFFFF;
		//
		// Script Buffer Reserved Locations
		public const byte PROG_ENTRY = 0;
		public const byte PROG_EXIT = 1;
		public const byte RD_DEVID = 2;
		public const byte PROGMEM_RD = 3;
		public const byte ERASE_CHIP_PREP = 4;
		public const byte PROGMEM_ADDRSET = 5;
		public const byte PROGMEM_WR_PREP = 6;
		public const byte PROGMEM_WR = 7;
		public const byte EE_RD_PREP = 8;
		public const byte EE_RD = 9;
		public const byte EE_WR_PREP = 10;
		public const byte EE_WR = 11;
		public const byte CONFIG_RD_PREP = 12;
		public const byte CONFIG_RD = 13;
		public const byte CONFIG_WR_PREP = 14;
		public const byte CONFIG_WR = 15;
		public const byte USERID_RD_PREP = 16;
		public const byte USERID_RD = 17;
		public const byte USERID_WR_PREP = 18;
		public const byte USERID_WR = 19;
		public const byte OSSCAL_RD = 20;
		public const byte OSSCAL_WR = 21;
		public const byte ERASE_CHIP = 22;
		public const byte ERASE_PROGMEM = 23;
		public const byte ERASE_EE = 24;
		//public const byte ERASE_CONFIG      = 25;
		public const byte ROW_ERASE = 26;
		public const byte TESTMEM_RD = 27;
		public const byte EEROW_ERASE = 28;

		// OSCCAL valid mask in config masks
		public const int OSCCAL_MASK = 7;

		// EEPROM config words
		public const int PROTOCOL_CFG = 0;
		public const int ADR_MASK_CFG = 1;
		public const int ADR_BITS_CFG = 2;
		public const int CS_PINS_CFG = 3;
		public const int BS_BELOW_CS_CFG = 4;		// indicates if block select bit is below chip select bits in I2C control byte
		public const int TYP_ERASE_TIME_CFG = 4;
		public const int MAX_ERASE_TIME_CFG = 5;
		// EEPROM Protocols
		public const int I2C_BUS = 1;
		public const int SPI_BUS = 2;
		public const int MICROWIRE_BUS = 3;
		public const int UNIO_BUS = 4;
		public const int SPI_FLASH_BUS = 5;
		public const bool READ_BIT = true;
		public const bool WRITE_BIT = false;

		// for user32.dll window flashing
		//Stop flashing. The system restores the window to its original state. 
		public const UInt32 FLASHW_STOP = 0;
		//Flash the window caption. 
		public const UInt32 FLASHW_CAPTION = 1;
		//Flash the taskbar button. 
		public const UInt32 FLASHW_TRAY = 2;
		//Flash both the window caption and taskbar button.
		//This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags. 
		public const UInt32 FLASHW_ALL = 3;
		//Flash continuously, until the FLASHW_STOP flag is set. 
		public const UInt32 FLASHW_TIMER = 4;
		//Flash continuously until the window comes to the foreground. 
		public const UInt32 FLASHW_TIMERNOFG = 12;

		// PICkit 2 internal EEPROM Locations
		public const byte ADC_CAL_L = 0x00;
		public const byte ADC_CAL_H = 0x01;
		public const byte CPP_OFFSET = 0x02;
		public const byte CPP_CAL = 0x03;
		public const byte UNIT_ID = 0xF0;  //through 0xFF

		public const UInt32 WAIT_OBJECT_0 = 0x00000000;
		public const UInt32 WAIT_TIMEOUT = 0x00000102;

		/*

		public struct OVERLAPPED
		{
			public int Internal;
			public int InternalHigh;
			public int Offset;
			public int OffsetHigh;
			public int hEvent;
		};
		 */



		// PIC32 related
		public const uint P32_PROGRAM_FLASH_START_ADDR = 0x1D000000;
		public const uint P32_BOOT_FLASH_START_ADDR = 0x1FC00000;

		// dsPIC33AK related
		public const uint P33AK_PROGRAM_FLASH_START_ADDR = 0x800000;

		// OSCCAL regeration
		public static uint[] BASELINE_CAL = new uint[41]{
            0x0C00, 0x0025, 0x0067, 0x0068, 0x0069, 0x0066, 0x0CFE, 0x0006, 
            0x0626, 0x0A08, 0x0726, 0x0A0A, 0x0070, 0x0C82, 0x0031, 0x02F0, 
            0x0A0F, 0x02F1, 0x0A0F, 0x0CF9, 0x0030, 0x0CC8, 0x0031, 0x0506, 
            0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x02F0, 0x0A18, 0x0000, 
            0x0CF9, 0x0030, 0x0000, 0x0000, 0x0000, 0x02F1, 0x0A18, 0x0406, 
            0x0A08 };

		public static uint[] MR16F676FAM_CAL = new uint[48]{
            0x3000, 0x2805, 0x0000, 0x0000, 0x0009, 0x1683, 0x0090, 0x0191, 
            0x019F, 0x30FE, 0x0085, 0x1283, 0x3007, 0x0099, 0x0185, 0x1885, 
            0x280F, 0x1C85, 0x2811, 0x01A0, 0x3082, 0x00A1, 0x0BA0, 0x2816, 
            0x0BA1, 0x2816, 0x30F9, 0x00A0, 0x30C8, 0x00A1, 0x1405, 0x0000, 
            0x0000, 0x0000, 0x0000, 0x0000, 0x0BA0, 0x281F, 0x0000, 0x30F9, 
            0x00A0, 0x0000, 0x0000, 0x0000, 0x0BA1, 0x281F, 0x1005, 0x280F};
	}
}
