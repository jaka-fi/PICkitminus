using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Pk2 = PICkit2V2.PICkitFunctions;
using KONST = PICkit2V2.Constants;

namespace PICkit2V2
{
    public class PIC24F_PE
    {
        public static DelegateStatusWin UpdateStatusWinText;
        public static DelegateResetStatusBar ResetStatusBar;
        public static DelegateStepStatusBar StepStatusBar;    
        
        private static byte ICSPSpeedRestore = 0;     
        private static bool PEGoodOnWrite = false;                                           
                                                        
        // Trimmed down Program Executive version 0x0026
        // -- fits in 1st page of exec mem, so cal words don't have to be saved & restored.
        // -- No blank check, no CRC
        private const int PIC24_PE_Version = 0x0026;
        private const int PIC24_PE_ID = 0x009B;
        private static uint[] PIC24_PE_Code = new uint[512] {
		0x00040080, 0x00000080, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 0x0080011E, 
		0x00FA0000, 0x00208CEF, 0x00227F80, 0x00880100, 0x00000000, 0x00070002, 0x00FA8000, 0x00060000, 
		0x00FA0000, 0x00EB0000, 0x00883A20, 0x00070150, 0x000700D8, 0x0007000B, 0x0037FFFD, 0x00F80034, 
		0x00781F88, 0x00B3C008, 0x008801A8, 0x0078044F, 0x00FA0000, 0x00FE0000, 0x00FA8000, 0x00F90034, 
		0x00064000, 0x00FA0002, 0x00804000, 0x00DE004C, 0x00FB8000, 0x00780F00, 0x0078001E, 0x00500FE5, 
		0x00320015, 0x0078009E, 0x00508FE5, 0x003C0005, 0x0078009E, 0x00408060, 0x00500FE2, 0x003E0014, 
		0x00370007, 0x0078001E, 0x00500FEA, 0x0032000C, 0x0078009E, 0x00508FEB, 0x0032000B, 0x0037000C, 
		0x00A888C7, 0x00A9A8C7, 0x00A9C8C7, 0x00A9E8C7, 0x0007000E, 0x0037000B, 0x00070040, 0x00370009, 
		0x00070081, 0x00370007, 0x000700A1, 0x00370005, 0x00A888C7, 0x00A8A8C7, 0x00A9C8C7, 0x00A9E8C7, 
		0x00070002, 0x00FA8000, 0x00060000, 0x00FA0000, 0x00804631, 0x002F0000, 0x00608080, 0x00210000, 
		0x00508F80, 0x00320006, 0x00804631, 0x002F0000, 0x00608080, 0x00230000, 0x00508F80, 0x003A0003, 
		0x00EB4000, 0x00B7E8C6, 0x00370002, 0x00B3C010, 0x00B7E8C6, 0x00804000, 0x00DE004C, 0x00FB8000, 
		0x0060006F, 0x00DD0148, 0x00804631, 0x002F0FF0, 0x00608000, 0x00700002, 0x00884630, 0x00804631, 
		0x0020F000, 0x00608080, 0x00202000, 0x00508F80, 0x003A000D, 0x00804010, 0x00D10000, 0x00B90063, 
		0x00E88000, 0x00884640, 0x00804010, 0x00600061, 0x00E00400, 0x00320006, 0x00804640, 0x00E88000, 
		0x00884640, 0x00370002, 0x00200020, 0x00884640, 0x00070082, 0x00FA8000, 0x00060000, 0x00240010, 
		0x00883B00, 0x00208005, 0x00EB0000, 0x00904425, 0x009004A5, 0x00428366, 0x00880198, 0x00780389, 
		0x00200100, 0x00BB0BB6, 0x00BBDBB6, 0x00BBEBB6, 0x00BB1BB6, 0x00BB0BB6, 0x00BBDBB6, 0x00BBEBB6, 
		0x00BB1BB6, 0x00E90000, 0x003AFFF6, 0x00090005, 0x00780B46, 0x00200080, 0x00538380, 0x00BB0BB6, 
		0x00A8C761, 0x00200557, 0x00883B37, 0x00200AA7, 0x00883B37, 0x00A8E761, 0x00000000, 0x00000000, 
		0x00803B00, 0x00A3F000, 0x0031FFFD, 0x00070005, 0x00DD004C, 0x00208C61, 0x00780880, 0x0007FFA3, 
		0x00060000, 0x00208000, 0x00EB0080, 0x00904420, 0x009004A0, 0x002000C7, 0x004001E6, 0x00880198, 
		0x00780289, 0x002001F4, 0x00BA0315, 0x00E13033, 0x003A000B, 0x00BADBB5, 0x00BAD3D5, 0x00E13033, 
		0x003A0007, 0x00BA0335, 0x00E13033, 0x003A0004, 0x00E90204, 0x003BFFF4, 0x00200010, 0x00370001, 
		0x00200020, 0x00060000, 0x00EF2032, 0x002FFFE7, 0x00370000, 0x00EB8200, 0x002FFFE6, 0x00208000, 
		0x00900190, 0x00900120, 0x00B10012, 0x00B18003, 0x0032000E, 0x00BA0057, 0x00E78004, 0x00370009, 
		0x00BAC017, 0x00E78404, 0x00370006, 0x00E13007, 0x003AFFF5, 0x00800190, 0x00E80000, 0x00880190, 
		0x0037FFF1, 0x002000F1, 0x00370001, 0x00200F01, 0x0021A000, 0x00700001, 0x00208C63, 0x00781980, 
		0x00200020, 0x00780980, 0x0007001C, 0x00060000, 0x00200261, 0x0021B000, 0x00700001, 0x00208C61, 
		0x00781880, 0x00200020, 0x00780880, 0x00070013, 0x00060000, 0x00070067, 0x00208007, 0x00781B80, 
		0x0020FFF3, 0x00600183, 0x00E90183, 0x00320004, 0x00070060, 0x00781B80, 0x00E90183, 0x003AFFFC, 
		0x00A9E241, 0x00205000, 0x00881210, 0x00EFA248, 0x00A8E241, 0x00EFA248, 0x00060000, 0x00EF2240, 
		0x0009001D, 0x00000000, 0x00204000, 0x00881210, 0x00A94085, 0x00A8E241, 0x00208C67, 0x00780037, 
		0x00881240, 0x00780037, 0x0007004F, 0x00208C60, 0x00900290, 0x00E98285, 0x00320038, 0x00904010, 
		0x00B240F0, 0x00B3C011, 0x00E10401, 0x003A000A, 0x00208000, 0x009040A0, 0x00FB8081, 0x00880191, 
		0x009003A0, 0x00BA0037, 0x0007003F, 0x00E90285, 0x003AFFFC, 0x00370029, 0x00B3C021, 0x00E10401, 
		0x003A002E, 0x00208000, 0x00900290, 0x00D10305, 0x00B00040, 0x00784090, 0x00780401, 0x007800D0, 
		0x00780381, 0x00E90005, 0x00320015, 0x00200001, 0x00880198, 0x00000000, 0x00BA0897, 0x0007002A, 
		0x00BAD897, 0x00B00027, 0x00B08008, 0x00880198, 0x00000000, 0x00BAD097, 0x00070023, 0x00BA0017, 
		0x00070021, 0x00B00027, 0x00B08008, 0x00880198, 0x00E90306, 0x003AFFEE, 0x00A60005, 0x00370007, 
		0x00880198, 0x00000000, 0x00BA0017, 0x00070016, 0x00BAC017, 0x00FB8000, 0x00070013, 0x00AE4085, 
		0x0037FFFE, 0x00A94085, 0x00A9E241, 0x00A86243, 0x00A8E241, 0x00801241, 0x00060000, 0x00B3C0C1, 
		0x00E10401, 0x003AFFF5, 0x00780037, 0x00070006, 0x0037FFF2, 0x00AE4085, 0x0037FFFE, 0x00A94085, 
		0x00801240, 0x00060000, 0x00781F81, 0x00AE4085, 0x0037FFFE, 0x00AE4085, 0x0037FFFC, 0x00A94085, 
		0x00801241, 0x00881240, 0x007800CF, 0x00060000, 0x00800210, 0x00B30A00, 0x00880210, 0x0020C000, 
		0x00881210, 0x00A94085, 0x00EF20A8, 0x00A800A9, 0x00A84095, 0x00280000, 0x00881200, 0x00EF2762, 
		0x00EF2764, 0x00060000, 0x000008CC, 0x00000002, 0x00000000, 0x00000800, 0x000000CC, 0x00000000, 
		0x00000000, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 
		0x0000009B, 0x00000026, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF, 0x00FFFFFF };
            
        private static byte[] BitReverseTable = new byte[256]
        {
          0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0, 
          0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8, 
          0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4, 
          0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC, 
          0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2, 
          0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA,
          0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6, 
          0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE,
          0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1,
          0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9, 
          0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5,
          0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD,
          0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3, 
          0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB,
          0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7, 
          0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF
        };


        public static bool DownloadPE()
        {
        
            Pk2.RunScript(KONST.PROG_ENTRY, 1);
            // Erase Executive Memory
            Pk2.ExecuteScript(Pk2.DevFile.PartsList[Pk2.ActivePart].DebugWriteVectorScript);
            
            // Set address
            if (Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemWrPrepScript != 0)
            { // if prog mem address set script exists for this part
                Pk2.DownloadAddress3(0x800000); // start of exec memory
                Pk2.RunScript(KONST.PROGMEM_WR_PREP, 1);
            }
            
            int instruction = 0;
            int commOffSet = 0;
            byte[] commandArrayp = new byte[64];
            
            // Program the exec in 8 rows
            for (int row = 0; row < 8; row++)
            {
                // Download a 64-instruction row 
                for (int section = 0; section < 4; section++)
                {
                    commOffSet = 0;
                    if (section == 0)
                    {
                        commandArrayp[commOffSet++] = KONST.CLR_DOWNLOAD_BUFFER;
                    }
                    commandArrayp[commOffSet++] = KONST.DOWNLOAD_DATA;
                    commandArrayp[commOffSet++] = 48; // 16 instructions.
                    for (int word = 0; word < 16; word++)
                    {
                        commandArrayp[commOffSet++] = (byte)(PIC24_PE_Code[instruction] & 0xFF);
                        commandArrayp[commOffSet++] = (byte)((PIC24_PE_Code[instruction] >> 8) & 0xFF);
                        commandArrayp[commOffSet++] = (byte)((PIC24_PE_Code[instruction] >> 16) & 0xFF); 
                        instruction++;
                    }
                    for (; commOffSet < 64; commOffSet++)
                    {
                        commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                    }
                    Pk2.writeUSB(commandArrayp);
                }
                // Program the row
                commOffSet = 0;
                commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
                commandArrayp[commOffSet++] = 0; // fill in later
                commandArrayp[commOffSet++] = KONST._WRITE_BUFWORD_W;
                commandArrayp[commOffSet++] = 0;
                commandArrayp[commOffSet++] = KONST._WRITE_BUFBYTE_W;
                commandArrayp[commOffSet++] = 1;
                commandArrayp[commOffSet++] = KONST._WRITE_BUFWORD_W;
                commandArrayp[commOffSet++] = 3;
                commandArrayp[commOffSet++] = KONST._WRITE_BUFBYTE_W;
                commandArrayp[commOffSet++] = 2;
                commandArrayp[commOffSet++] = KONST._COREINST24; // TBLWTL W0, [W7]
                commandArrayp[commOffSet++] = 0x80;
                commandArrayp[commOffSet++] = 0x0B;
                commandArrayp[commOffSet++] = 0xBB;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._COREINST24; // TBLWTH W1, [W7++]
                commandArrayp[commOffSet++] = 0x81;
                commandArrayp[commOffSet++] = 0x9B;
                commandArrayp[commOffSet++] = 0xBB;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._COREINST24; // TBLWTH W2, [W7]
                commandArrayp[commOffSet++] = 0x82;
                commandArrayp[commOffSet++] = 0x8B;
                commandArrayp[commOffSet++] = 0xBB;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._COREINST24; // TBLWTL W3, [W7++]
                commandArrayp[commOffSet++] = 0x83;
                commandArrayp[commOffSet++] = 0x1B;
                commandArrayp[commOffSet++] = 0xBB;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._LOOP;
                commandArrayp[commOffSet++] = 32;
                commandArrayp[commOffSet++] = 31;
                commandArrayp[commOffSet++] = KONST._COREINST24; // BSET.B 0x0761, #7
                commandArrayp[commOffSet++] = 0x61;
                commandArrayp[commOffSet++] = 0xE7;
                commandArrayp[commOffSet++] = 0xA8;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._COREINST24; // GOTO 0x200
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = 0x02;
                commandArrayp[commOffSet++] = 0x04;
                commandArrayp[commOffSet++] = KONST._NOP24;
                commandArrayp[commOffSet++] = KONST._LOOP;
                commandArrayp[commOffSet++] = 1;
                commandArrayp[commOffSet++] = 3;
                commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
                commandArrayp[commOffSet++] = 72;
                commandArrayp[1] = (byte)(commOffSet - 2);  // script length
                for (; commOffSet < 64; commOffSet++)
                {
                    commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                }
                Pk2.writeUSB(commandArrayp);
            }
            
            // VERIFY PE
            // Set address
            if (Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemWrPrepScript != 0)
            { // if prog mem address set script exists for this part
                Pk2.DownloadAddress3(0x800000); // start of exec memory
                Pk2.RunScript(KONST.PROGMEM_ADDRSET, 1);
            }
            
            // verify the exec in 16 sections
            byte[] upload_buffer = new byte[KONST.UploadBufferSize];
            instruction = 0;
            for (int section = 0; section < 16; section++)
            {
                //Pk2.RunScriptUploadNoLen2(KONST.PROGMEM_RD, 1);
                Pk2.RunScriptUploadNoLen(KONST.PROGMEM_RD, 1);
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, 0, KONST.USB_REPORTLENGTH);
                //Pk2.GetUpload();
                Pk2.UploadDataNoLen();
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
                int uploadIndex = 0;
                for (int word = 0; word < 32; word++)
                {
                    uint memWord = (uint)upload_buffer[uploadIndex++];
                    memWord |= (uint)upload_buffer[uploadIndex++] << 8;
                    memWord |= (uint)upload_buffer[uploadIndex++] << 16;
                    if (memWord != PIC24_PE_Code[instruction++])
                    {
                        Pk2.RunScript(KONST.PROG_EXIT, 1);
                        return false;
                    }
                }
            }
            
            Pk2.RunScript(KONST.PROG_EXIT, 1);
            
            return true;
        }
        
        public static bool PE_Connect()
        {
            Pk2.RunScript(KONST.PROG_ENTRY, 1);

            if (Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemWrPrepScript != 0)
            { // if prog mem address set script exists for this part
                Pk2.DownloadAddress3(0x8003C0); // last 32 words of exec memory
                Pk2.RunScript(KONST.PROGMEM_ADDRSET, 1);
            }
            byte[] upload_buffer = new byte[KONST.UploadBufferSize];
            //Pk2.RunScriptUploadNoLen2(KONST.PROGMEM_RD, 1);
            Pk2.RunScriptUploadNoLen(KONST.PROGMEM_RD, 1);
            Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, 0, KONST.USB_REPORTLENGTH);
            //Pk2.GetUpload();
            Pk2.UploadDataNoLen();
            Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
            // check ID word
            int memValue = (int)upload_buffer[72]; // addresss 0x8003F0
            memValue |= (int)(upload_buffer[73] << 8);
            if (memValue != PIC24_PE_ID)
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            // check PE version
            memValue = (int)upload_buffer[75]; // addresss 0x8003F2
            memValue |= (int)(upload_buffer[76] << 8);
            if (memValue != PIC24_PE_Version)
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }

            Pk2.RunScript(KONST.PROG_EXIT, 1);
            
            // It looks like there is a PE there.  Try talking to the PE directly:
            int commOffSet = 0;
            byte[] commandArrayp = new byte[64];
            // entering programming mode with PE (4D434850)
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 0; // fill in later
            commandArrayp[commOffSet++] = KONST._VPP_OFF;
            commandArrayp[commOffSet++] = KONST._MCLR_GND_ON;
            commandArrayp[commOffSet++] = KONST._VPP_PWM_ON;
            commandArrayp[commOffSet++] = KONST._BUSY_LED_ON;
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = KONST._DELAY_LONG;
            commandArrayp[commOffSet++] = 20;
            commandArrayp[commOffSet++] = KONST._MCLR_GND_OFF;
            commandArrayp[commOffSet++] = KONST._VPP_ON;
            commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
            commandArrayp[commOffSet++] = 23;
            commandArrayp[commOffSet++] = KONST._VPP_OFF;
            commandArrayp[commOffSet++] = KONST._MCLR_GND_ON;
            commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
            commandArrayp[commOffSet++] = 47;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0xB2;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0xC2;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0x12;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0x0A;
            commandArrayp[commOffSet++] = KONST._MCLR_GND_OFF;
            commandArrayp[commOffSet++] = KONST._VPP_ON;
            commandArrayp[commOffSet++] = KONST._DELAY_LONG;
            commandArrayp[commOffSet++] = 6;    //32ms
            commandArrayp[1] = (byte)(commOffSet - 2);  // script length
            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);
            // Try sanity Check
            commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 12;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // sanity check = 0x00 01
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0x80;                     // PE talks MSB first, script routines are LSB first.
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = 0x02; // PGD is input
            commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
            commandArrayp[commOffSet++] = 5;                        //100+ us
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST.UPLOAD_DATA;
            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);
            if (!Pk2.readUSB())
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            if (Pk2.Usb_read_array[1] != 4) // expect 4 bytes back : 0x10 00 00 02
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            if ((Pk2.Usb_read_array[2] != 0x08) || (Pk2.Usb_read_array[3] != 0x00)
                || (Pk2.Usb_read_array[4] != 0x00) || (Pk2.Usb_read_array[5] != 0x40))
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            
            // Passed sanity check; verify version.
            commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 14;
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = 0x00; // PGD is output
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // QVER = 0xB0 01
            commandArrayp[commOffSet++] = 0x0D;
            commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
            commandArrayp[commOffSet++] = 0x80;                     // PE talks MSB first, script routines are LSB first.
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = 0x02; // PGD is input
            commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
            commandArrayp[commOffSet++] = 5;                        //100+ us
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
            commandArrayp[commOffSet++] = KONST.UPLOAD_DATA;
            for (; commOffSet < 64; commOffSet++)
            {
                commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
            }
            Pk2.writeUSB(commandArrayp);
            if (!Pk2.readUSB())
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            if (Pk2.Usb_read_array[1] != 4) // expect 4 bytes back : 0x1B <Ver> 00 02
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            if ((Pk2.Usb_read_array[2] != 0xD8) || (BitReverseTable[Pk2.Usb_read_array[3]] != (byte)PIC24_PE_Version)
                || (Pk2.Usb_read_array[4] != 0x00) || (Pk2.Usb_read_array[5] != 0x40))
            {
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                return false;
            }
            
            // Do not exit programming mode if we successfully find a PE
            return true;
        }

        public static bool PE_DownloadAndConnect()
        {
            // VDD must already be on!
            // reduce PE comm speed to 500kHz max
            ICSPSpeedRestore = Pk2.LastICSPSpeed;
            if (Pk2.LastICSPSpeed < 2)
                Pk2.SetProgrammingSpeed(2);
            
            // See if the PE already exists
            if (!PE_Connect())
            { // it doesn't, download it    
                UpdateStatusWinText("Downloading Programming Executive...");
                if (!DownloadPE())
                { // download failed
                    UpdateStatusWinText("Downloading Programming Executive...FAILED!");
                    restoreICSPSpeed();
                    return false;
                }
                if (!PE_Connect())
                { // try connecting
                    UpdateStatusWinText("Downloading Programming Executive...FAILED!");
                    restoreICSPSpeed();
                    return false;
                }
            }

            return true;
        }
        
        private static void restoreICSPSpeed()
        {
            if (ICSPSpeedRestore != Pk2.LastICSPSpeed)
                Pk2.SetProgrammingSpeed(ICSPSpeedRestore);
        }
        

        public static bool PE24FBlankCheck(string saveText)
        {
            if (!PE_DownloadAndConnect())
            {
                return false;
            }

            UpdateStatusWinText(saveText);

            // to deal with configs in program mem
            int configsStart = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem - Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords;


            // Check Program Memory ====================================================================================
            byte[] upload_buffer = new byte[KONST.UploadBufferSize];
            int bytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;
            int wordsPerLoop = 32;
            int wordsRead = 0;
            int uploadIndex = 0;
            int commOffSet = 0;
            byte[] commandArrayp = new byte[64];
            ResetStatusBar((int)(Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem / wordsPerLoop));
            uint blankVal = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue;

            do
            {
                commOffSet = 0;
                commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
                commandArrayp[commOffSet++] = 0;
                commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
                commandArrayp[commOffSet++] = 0x00; // PGD is output
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; //0x20 04
                commandArrayp[commOffSet++] = 0x04;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = 0x20;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // Length N
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[wordsPerLoop];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // address MSW
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[((wordsRead >> 15) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // address LSW
                commandArrayp[commOffSet++] = BitReverseTable[((wordsRead >> 7) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[((wordsRead << 1) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
                commandArrayp[commOffSet++] = 0x02; // PGD is input
                commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
                commandArrayp[commOffSet++] = 5;
                commandArrayp[commOffSet++] = KONST._READ_BYTE;         // Read & toss 2 response words
                commandArrayp[commOffSet++] = KONST._READ_BYTE;
                commandArrayp[commOffSet++] = KONST._READ_BYTE;
                commandArrayp[commOffSet++] = KONST._READ_BYTE;
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;  // read 32 3-byte words
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._LOOP;
                commandArrayp[commOffSet++] = 3;
                commandArrayp[commOffSet++] = 31;
                commandArrayp[commOffSet++] = KONST.UPLOAD_DATA_NOLEN;
                commandArrayp[commOffSet++] = KONST.UPLOAD_DATA_NOLEN;
                commandArrayp[1] = (byte)(commOffSet - 4);  // script length
                for (; commOffSet < 64; commOffSet++)
                {
                    commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                }
                Pk2.writeUSB(commandArrayp);

                Pk2.GetUpload();
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, 0, KONST.USB_REPORTLENGTH);
                Pk2.GetUpload();
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
                uploadIndex = 0;
                for (int word = 0; word < wordsPerLoop; word += 2)
                {
                    // two word2 of packed instructions
                    uint memWord1 = (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 8;
                    memWord1 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]];
                    uint memWord2 = (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 16;
                    memWord1 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 16;
                    memWord2 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 8;
                    memWord2 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]];
                    if (wordsRead >= configsStart)
                    {
                        blankVal = (0xFF0000U | (uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[wordsRead - configsStart]);
                    }
                    if (blankVal != memWord1)
                    {
                        string error = "Program Memory is not blank starting at address\n";
                        error += string.Format("0x{0:X6}",
                                (wordsRead * Pk2.DevFile.Families[Pk2.GetActiveFamily()].AddressIncrement));
                        UpdateStatusWinText(error);
                        Pk2.RunScript(KONST.PROG_EXIT, 1);
                        restoreICSPSpeed();
                        return false;
                    }
                    wordsRead++;
                    if (wordsRead >= configsStart)
                    {
                        blankVal = (0xFF0000U | (uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[wordsRead - configsStart]);
                    }
                    if (blankVal != memWord2)
                    {
                        string error = "Program Memory is not blank starting at address\n";
                        error += string.Format("0x{0:X6}", 
                                (wordsRead * Pk2.DevFile.Families[Pk2.GetActiveFamily()].AddressIncrement));
                        UpdateStatusWinText(error);
                        Pk2.RunScript(KONST.PROG_EXIT, 1);
                        restoreICSPSpeed();
                        return false;
                    }
                    wordsRead++;
                    if (wordsRead >= Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem)
                    {
                        break; // for cases where ProgramMemSize%WordsPerLoop != 0
                    }
                }
                StepStatusBar();
            } while (wordsRead < Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem);

            Pk2.RunScript(KONST.PROG_EXIT, 1);
            restoreICSPSpeed();
            return true;
        }
        
        public static bool PE24FRead(string saveText)
        {
            if (!PE_DownloadAndConnect())
            {
                return false;
            }

            UpdateStatusWinText(saveText);

            // Read Program Memory ====================================================================================
            byte[] upload_buffer = new byte[KONST.UploadBufferSize];
            int bytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;
            int wordsPerLoop = 32;
            int wordsRead = 0;
            int uploadIndex = 0;
            int commOffSet = 0;
            byte[] commandArrayp = new byte[64];
            ResetStatusBar((int)(Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem / wordsPerLoop));

            do
            {
                commOffSet = 0;
                commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
                commandArrayp[commOffSet++] = 0;
                commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
                commandArrayp[commOffSet++] = 0x00; // PGD is output
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; //0x20 04
                commandArrayp[commOffSet++] = 0x04;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = 0x20;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // Length N
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[wordsPerLoop];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // address MSW
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[((wordsRead >> 15) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // address LSW
                commandArrayp[commOffSet++] = BitReverseTable[((wordsRead >> 7) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[((wordsRead << 1) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
                commandArrayp[commOffSet++] = 0x02; // PGD is input
                commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
                commandArrayp[commOffSet++] = 5;
                commandArrayp[commOffSet++] = KONST._READ_BYTE;         // Read & toss 2 response words
                commandArrayp[commOffSet++] = KONST._READ_BYTE;
                commandArrayp[commOffSet++] = KONST._READ_BYTE;
                commandArrayp[commOffSet++] = KONST._READ_BYTE;
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;  // read 32 3-byte words
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._LOOP;
                commandArrayp[commOffSet++] = 3;
                commandArrayp[commOffSet++] = 31;
                commandArrayp[commOffSet++] = KONST.UPLOAD_DATA_NOLEN;
                commandArrayp[commOffSet++] = KONST.UPLOAD_DATA_NOLEN;
                commandArrayp[1] = (byte)(commOffSet - 4);  // script length
                for (; commOffSet < 64; commOffSet++)
                {
                    commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                }
                Pk2.writeUSB(commandArrayp);

                Pk2.GetUpload();
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, 0, KONST.USB_REPORTLENGTH);
                Pk2.GetUpload();
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
                uploadIndex = 0;
                for (int word = 0; word < wordsPerLoop; word+= 2)
                {
                    // two word2 of packed instructions
                    uint memWord1 = (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 8;
                    memWord1 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]];
                    uint memWord2 = (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 16;
                    memWord1 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 16;
                    memWord2 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 8;
                    memWord2 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]];
                    Pk2.DeviceBuffers.ProgramMemory[wordsRead++] = memWord1;
                    Pk2.DeviceBuffers.ProgramMemory[wordsRead++] = memWord2;
                    if (wordsRead >= Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem)
                    {
                        break; // for cases where ProgramMemSize%WordsPerLoop != 0
                    }
                }
                StepStatusBar();
            } while (wordsRead < Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem);

            Pk2.RunScript(KONST.PROG_EXIT, 1);
            restoreICSPSpeed();
            return true;
        }
        
        public static bool PE24FWrite(int endOfBuffer, string saveText, bool writeVerify)
        {
            if (!PE_DownloadAndConnect())
            {
                PEGoodOnWrite = false;
                return false;
            }

            PEGoodOnWrite = true;;

            UpdateStatusWinText(saveText);
            
            // Since the PE actually verifies words when it writes, we need the config words
            // filled with valid blank values or the PE PROGP command on them will fail.
            if (endOfBuffer == Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem)
            {// if we'll be writing configs
                for (int cfg = Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords; cfg > 0; cfg--)
                {
                    Pk2.DeviceBuffers.ProgramMemory[endOfBuffer - cfg] &= 
                                (0xFF0000U | (uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords - cfg]);
                }
            }
        
            byte[] downloadBuffer = new byte[KONST.DownLoadBufferSize];
            int wordsPerLoop = 64;
            int wordsWritten = 0;

            ResetStatusBar((int)(endOfBuffer / wordsPerLoop));

            do
            {
                int downloadIndex = 0;
                for (int word = 0; word < wordsPerLoop; word+=2)
                {
                    // Put in packed format for PE  
                    uint memWord = Pk2.DeviceBuffers.ProgramMemory[wordsWritten++];
                    downloadBuffer[downloadIndex + 1] = BitReverseTable[(memWord & 0xFF)];
                    //checksumPk2Go += (byte) (memWord & 0xFF);
                    memWord >>= 8;
                    downloadBuffer[downloadIndex] = BitReverseTable[(memWord & 0xFF)];
                    //checksumPk2Go += (byte)(memWord & 0xFF);
                    memWord >>= 8;
                    downloadBuffer[downloadIndex + 3] = BitReverseTable[(memWord & 0xFF)];
                    //checksumPk2Go += (byte)(memWord & 0xFF);

                    memWord = Pk2.DeviceBuffers.ProgramMemory[wordsWritten++];
                    downloadBuffer[downloadIndex + 5] = BitReverseTable[(memWord & 0xFF)];
                    //checksumPk2Go += (byte) (memWord & 0xFF);
                    memWord >>= 8;
                    downloadBuffer[downloadIndex + 4] = BitReverseTable[(memWord & 0xFF)];
                    //checksumPk2Go += (byte)(memWord & 0xFF);
                    memWord >>= 8;
                    downloadBuffer[downloadIndex + 2] = BitReverseTable[(memWord & 0xFF)];
                    //checksumPk2Go += (byte)(memWord & 0xFF);
                    
                    downloadIndex += 6;
                            
                }
                // download data
                int dataIndex = Pk2.DataClrAndDownload(downloadBuffer, 0);
                while (dataIndex < downloadIndex)
                {
                    dataIndex = Pk2.DataDownload(downloadBuffer, dataIndex, downloadIndex);
                }

                int commOffSet = 0;
                byte[] commandArrayp = new byte[64];
                commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
                commandArrayp[commOffSet++] = 0; // fill in later
                commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
                commandArrayp[commOffSet++] = 0x00; // PGD is output
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // PROGP = 0x50 63
                commandArrayp[commOffSet++] = 0x0A;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = 0xC6;                      // PE talks MSB first, script routines are LSB first.
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // address MSW
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[(((wordsWritten - 64) >> 15) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // address LSW
                commandArrayp[commOffSet++] = BitReverseTable[(((wordsWritten - 64) >> 7) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[(((wordsWritten - 64) << 1) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_BUFFER;  // write 64 3-byte words
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._LOOP;
                commandArrayp[commOffSet++] = 3;
                commandArrayp[commOffSet++] = 63;
                commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
                commandArrayp[commOffSet++] = 0x02; // PGD is input
                commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
                commandArrayp[commOffSet++] = 118;                        //2.5ms
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;  // read response
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST.UPLOAD_DATA;
                commandArrayp[1] = (byte)(commOffSet - 3);
                for (; commOffSet < 64; commOffSet++)
                {
                    commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                }
                Pk2.writeUSB(commandArrayp);
                if (!Pk2.readUSB())
                {
                    UpdateStatusWinText("Programming Executive Error during Write.");
                    Pk2.RunScript(KONST.PROG_EXIT, 1);
                    restoreICSPSpeed();
                    return false;
                }
                if (Pk2.Usb_read_array[1] != 4) // expect 4 bytes back : 0x15 00 00 02
                {
                    UpdateStatusWinText("Programming Executive Error during Write.");
                    Pk2.RunScript(KONST.PROG_EXIT, 1);
                    restoreICSPSpeed();
                    return false;
                }
                if ((BitReverseTable[Pk2.Usb_read_array[2]] != 0x15) || (Pk2.Usb_read_array[3] != 0x00)
                    || (Pk2.Usb_read_array[4] != 0x00) || (BitReverseTable[Pk2.Usb_read_array[5]] != 0x02))
                {
                    UpdateStatusWinText("Programming Executive Error during Write.");
                    Pk2.RunScript(KONST.PROG_EXIT, 1);
                    restoreICSPSpeed();
                    return false;
                }

                StepStatusBar();
            } while (wordsWritten < endOfBuffer);

            if (!writeVerify)
            { // stay in programming mode if we're going to verify to prevent memory modifying code to execute.
                Pk2.RunScript(KONST.PROG_EXIT, 1);
                restoreICSPSpeed();
            }
            return true;
        }

        public static bool PE24FVerify(string saveText, bool writeVerify, int lastLocation)
        {
            if (!writeVerify || !PEGoodOnWrite)
            { // don't reconnect if doing a write verify and PE is already good
                if (!PE_DownloadAndConnect())
                {
                    return false;
                }
            }

            PEGoodOnWrite = false;
            
            if (!writeVerify)
                lastLocation = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem;

            UpdateStatusWinText(saveText);

            // Check Program Memory ====================================================================================
            byte[] upload_buffer = new byte[KONST.UploadBufferSize];
            int bytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;
            int wordsPerLoop = 32;
            int wordsRead = 0;
            int uploadIndex = 0;
            int commOffSet = 0;
            byte[] commandArrayp = new byte[64];
            ResetStatusBar((int)(lastLocation / wordsPerLoop));

            do
            {
                commOffSet = 0;
                commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
                commandArrayp[commOffSet++] = 0;
                commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
                commandArrayp[commOffSet++] = 0x00; // PGD is output
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; //0x20 04
                commandArrayp[commOffSet++] = 0x04;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = 0x20;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // Length N
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[wordsPerLoop];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // address MSW
                commandArrayp[commOffSet++] = 0x00;
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[((wordsRead >> 15) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL; // address LSW
                commandArrayp[commOffSet++] = BitReverseTable[((wordsRead >> 7) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._WRITE_BYTE_LITERAL;
                commandArrayp[commOffSet++] = BitReverseTable[((wordsRead << 1) & 0xFF)];
                commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
                commandArrayp[commOffSet++] = 0x02; // PGD is input
                commandArrayp[commOffSet++] = KONST._DELAY_SHORT;
                commandArrayp[commOffSet++] = 5;
                commandArrayp[commOffSet++] = KONST._READ_BYTE;         // Read & toss 2 response words
                commandArrayp[commOffSet++] = KONST._READ_BYTE;
                commandArrayp[commOffSet++] = KONST._READ_BYTE;
                commandArrayp[commOffSet++] = KONST._READ_BYTE;
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;  // read 32 3-byte words
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._READ_BYTE_BUFFER;
                commandArrayp[commOffSet++] = KONST._LOOP;
                commandArrayp[commOffSet++] = 3;
                commandArrayp[commOffSet++] = 31;
                commandArrayp[commOffSet++] = KONST.UPLOAD_DATA_NOLEN;
                commandArrayp[commOffSet++] = KONST.UPLOAD_DATA_NOLEN;
                commandArrayp[1] = (byte)(commOffSet - 4);  // script length
                for (; commOffSet < 64; commOffSet++)
                {
                    commandArrayp[commOffSet] = KONST.END_OF_BUFFER;
                }
                Pk2.writeUSB(commandArrayp);

                Pk2.GetUpload();
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, 0, KONST.USB_REPORTLENGTH);
                Pk2.GetUpload();
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
                uploadIndex = 0;
                for (int word = 0; word < wordsPerLoop; word += 2)
                {
                    // two word2 of packed instructions
                    uint memWord1 = (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 8;
                    memWord1 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]];
                    uint memWord2 = (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 16;
                    memWord1 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 16;
                    memWord2 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]] << 8;
                    memWord2 |= (uint)BitReverseTable[upload_buffer[uploadIndex++]];
                    if (Pk2.DeviceBuffers.ProgramMemory[wordsRead++] != memWord1)
                    {
                        string error = "";
                        if (!writeVerify)
                        {
                            error = "Verification of Program Memory failed at address\n";
                        }
                        else
                        {
                            error = "Programming failed at Program Memory address\n";
                        }
                        error += string.Format("0x{0:X6}",
                                (--wordsRead * Pk2.DevFile.Families[Pk2.GetActiveFamily()].AddressIncrement));
                        UpdateStatusWinText(error);
                        Pk2.RunScript(KONST.PROG_EXIT, 1);
                        restoreICSPSpeed();
                        return false;
                    }
                    if (Pk2.DeviceBuffers.ProgramMemory[wordsRead++] != memWord2)
                    {
                        string error = "";
                        if (!writeVerify)
                        {
                            error = "Verification of Program Memory failed at address\n";
                        }
                        else
                        {
                            error = "Programming failed at Program Memory address\n";
                        }
                        error += string.Format("0x{0:X6}",
                                (--wordsRead * Pk2.DevFile.Families[Pk2.GetActiveFamily()].AddressIncrement));
                        UpdateStatusWinText(error);
                        Pk2.RunScript(KONST.PROG_EXIT, 1);
                        restoreICSPSpeed();
                        return false;
                    }
                    if (wordsRead >= lastLocation)
                    {
                        break; // for cases where ProgramMemSize%WordsPerLoop != 0
                    }
                }
                StepStatusBar();
            } while (wordsRead < lastLocation);

            Pk2.RunScript(KONST.PROG_EXIT, 1);
            restoreICSPSpeed();
            return true;
        }


    }
}
