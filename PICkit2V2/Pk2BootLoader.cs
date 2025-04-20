using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using Pk2 = PICkit2V2.PICkitFunctions;
using KONST = PICkit2V2.Constants;

namespace PICkit2V2
{
    class Pk2BootLoader
    {
        public static bool ReadHexAndDownload(string fileName, ref ushort pk2num)
        {
            try
            {          
                FileInfo hexFile = new FileInfo(fileName);
                TextReader hexRead = hexFile.OpenText();
                byte[] flashWriteData = new byte[3+32];  // 3 address bytes plus 32 data bytes.
                
                string fileLine = hexRead.ReadLine();
                if (fileLine != null)
                {
                    Pk2.EnterBootloader();
                    Pk2.ResetPk2Number();
                    Thread.Sleep(3000);                   
                    int i;
                    pk2num = 0;
                    for (i = 0; i < 10; i++)
                    {
                        if (Pk2.DetectPICkit2Device(pk2num, true) == Constants.PICkit2USB.bootloader)
                        {                        
                            if (Pk2.VerifyBootloaderMode())
                            {
                                break;
                            }
                        }
                        else
                        {
                            pk2num++;  // look for PK2 with bootloader.
                        }
                        Thread.Sleep(500);                                   
                    }
                    if (i == 10)
                    {
                        hexRead.Close();
                        return false;
                    }
                }
                // erase PICkit 2 firmware flash
                Pk2.BL_EraseFlash();
                
                bool second16 = false;
                while (fileLine != null)
                {
                    if ((fileLine[0] == ':') && (fileLine.Length >= 11))
                    { // skip line if not hex line entry,or not minimum length ":BBAAAATTCC"
                        int byteCount = Int32.Parse(fileLine.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                        int fileAddress = Int32.Parse(fileLine.Substring(3, 4), System.Globalization.NumberStyles.HexNumber);
                        int recordType = Int32.Parse(fileLine.Substring(7, 2), System.Globalization.NumberStyles.HexNumber);

                        if ((second16) && ((fileAddress & 0x00000010) == 0))
                        {// if just moved to new 32-byte boundary.
                            Pk2.BL_WriteFlash(flashWriteData);
                            for (int x = 0; x < flashWriteData.Length; x++)
                            { // clear array for skipped bytes in hex file
                                flashWriteData[x] = 0xFF;
                            }
                        }
                        
                        second16 = ((fileAddress & 0x00000010) == 0x10);
                        
                        if (recordType == 0)
                        { // Data Record}
                            if ((fileAddress >= 0x2000) && (fileAddress < 0x7FE0))
                            { // don't program 5555 key at last address until after verification.
                                if (!second16)
                                {
                                    int rowAddress = fileAddress & 0xFFE0;
                                    flashWriteData[0] = (byte)(rowAddress & 0xFF);
                                    flashWriteData[1] = (byte)((rowAddress >> 8) & 0xFF);
                                    flashWriteData[2] = 0x00;  // address upper
                                }
                            
                                if (fileLine.Length >= (11 + (2 * byteCount)))
                                { // skip if line isn't long enough for bytecount.                    
                                    int startByte = fileAddress & 0x000F;
                                    int endByte = startByte + byteCount;

                                    int offset = 3;
                                    if (second16)
                                    {
                                        offset = 19;
                                    }
                                    for (int rowByte = 0; rowByte < 16; rowByte++)
                                    {
                                        if ((rowByte >= startByte) && (rowByte < endByte))
                                        {
                                            // get the byte value from hex file
                                            uint wordByte = UInt32.Parse(fileLine.Substring((9 + (2 * (rowByte - startByte))), 2), System.Globalization.NumberStyles.HexNumber);                                    
                                            flashWriteData[offset + rowByte] = (byte)(wordByte & 0xFF);
                                        }

                                    }

                                }
                            }
                            
                        } // end if (recordType == 0)  



                        if (recordType == 1)
                        { // end of record
                            break;
                        }
                    }
                    fileLine = hexRead.ReadLine();               

                }
                Pk2.BL_WriteFlash(flashWriteData); // write last row
                hexRead.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }   
            
        public static bool ReadHexAndVerify(string fileName)
        {
            try
            {
                FileInfo hexFile = new FileInfo(fileName);
                TextReader hexRead = hexFile.OpenText();               
                string fileLine = hexRead.ReadLine();
                bool verified = true;       
                int lastAddress = 0;        
                while (fileLine != null)
                {
                    if ((fileLine[0] == ':') && (fileLine.Length >= 11))
                    { // skip line if not hex line entry,or not minimum length ":BBAAAATTCC"
                        int byteCount = Int32.Parse(fileLine.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                        int fileAddress = Int32.Parse(fileLine.Substring(3, 4), System.Globalization.NumberStyles.HexNumber);
                        int recordType = Int32.Parse(fileLine.Substring(7, 2), System.Globalization.NumberStyles.HexNumber);

                        if (recordType == 0)
                        { // Data Record}
                            if ((fileAddress >= 0x2000) && (fileAddress < 0x7FE0))
                            { // don't check bootloader stuff.
                                int startByte = fileAddress & 0x000F; // read 16 bytes at a time.
                                int firstAddress = fileAddress & 0xFFF0;
                                if (lastAddress != firstAddress)
                                { // only read if next line in different 16-byte block
                                    Pk2.BL_ReadFlash16(firstAddress);
                                }
                                if (fileLine.Length >= (11 + (2 * byteCount)))
                                { // skip if line isn't long enough for bytecount.                    
                                    for (int lineByte = 0; lineByte < byteCount; lineByte++)
                                    {
                                        // get the byte value from hex file
                                        uint wordByte = UInt32.Parse(fileLine.Substring((9 + (2 * lineByte)), 2), System.Globalization.NumberStyles.HexNumber);
                                        if (Pk2.Usb_read_array[6 + startByte + lineByte] != (byte)(wordByte & 0xFF))
                                        {
                                            verified  = false;
                                            recordType = 1;
                                            break;   
                                        }
                                    }
                                }
                                lastAddress = firstAddress;
                            }
                        } // end if (recordType == 0)  



                        if (recordType == 1)
                        { // end of record
                            break;
                        }
                    }
                    fileLine = hexRead.ReadLine();               

                }
                hexRead.Close();
                return verified;
            }
            catch
            {
                return false;
            }                
        
        }
    
    
    }
}
