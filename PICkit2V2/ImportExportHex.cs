using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Pk2 = PICkit2V2.PICkitFunctions;

namespace PICkit2V2
{
    class ImportExportHex
    {
        public static DateTime LastWriteTime = new DateTime();
    
        public static Constants.FileRead ImportHexFile(String filePath, bool progMem, bool eeMem)
        {  // NOTE: The device buffers being read into must all be set to blank value before getting here!
        
            if (filePath.Length > 4)
            {
                if ((filePath.Substring(filePath.Length - 4).ToUpperInvariant() == ".BIN") && Pk2.FamilyIsEEPROM())
                {
                    return ImportBINFile(filePath);
                }   
            }
            
            try
            {
                FileInfo hexFile = new FileInfo(filePath);
                LastWriteTime = hexFile.LastWriteTime;               
                TextReader hexRead = hexFile.OpenText();
                int bytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].ProgMemHexBytes;
                int eeMemBytes = Pk2.DevFile.Families[Pk2.GetActiveFamily()].EEMemHexBytes;
                uint eeAddr = Pk2.DevFile.PartsList[Pk2.ActivePart].EEAddr;
                int progMemSizeBytes = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem * bytesPerWord;
                int segmentAddress = 0;
                bool configRead = false;
                bool lineExceedsFlash = true;
                bool fileExceedsFlash = false;
                int userIDs = Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDWords;
                uint userIDAddr = Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDAddr;
                if (userIDAddr == 0)
                {
                    userIDAddr = 0xFFFFFFFF;
                }
                int userIDMemBytes = Pk2.DevFile.Families[Pk2.GetActiveFamily()].UserIDHexBytes;
                // need to set config words to memory blank.
                int configWords = Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords;
                bool[] configLoaded = new bool[configWords];
                for (int cw = 0; cw < configWords; cw++)
                {
                    if (progMem)   // don't clear config words if importing only EEPROM
                    {
                        Pk2.DeviceBuffers.ConfigWords[cw] = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue;
                    }
                    if (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[cw] == 0)
                        configLoaded[cw] = true;  // if mask is blank (no implemented bits) don't need it in file
                    else if (cw > 8)
                        configLoaded[cw] = true;  // don't warn about configs above config9, since we can't set blank mask for those
                    else
                        configLoaded[cw] = false; // implemented bits, so warn if not in hex file.
                }
                int cfgBytesPerWord = bytesPerWord;
                uint programMemStart = 0;
                uint bootMemStart = 0;
                uint bootMemSize = Pk2.DevFile.PartsList[Pk2.ActivePart].BootFlash;
                if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue > 0xFFFFFF)
                { // PIC32
                    programMemStart = Constants.P32_PROGRAM_FLASH_START_ADDR;
                    bootMemStart = Constants.P32_BOOT_FLASH_START_ADDR;
                    progMemSizeBytes -= (int)bootMemSize * bytesPerWord;
                    progMemSizeBytes += (int)programMemStart;
                    cfgBytesPerWord = 2;
                }
                uint bootMemEnd = bootMemStart + (bootMemSize * (uint)bytesPerWord);   
                int bootArrayStart = (int)(Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem - bootMemSize);

                //bool endOfHexFile = false;
                //string fileLine;
                //endOfHexFile = ((fileLine = hexRead.ReadLine()) == null);
                //while (!endOfHexFile)
                string fileLine = hexRead.ReadLine();
                while (fileLine != null)
                {
                    if (!String.IsNullOrEmpty(fileLine))
                    {
                        if ((fileLine[0] == ':') && (fileLine.Length >= 11))
                        { // skip line if not hex line entry,or not minimum length ":BBAAAATTCC"
                            int byteCount = Int32.Parse(fileLine.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                            int fileAddress = segmentAddress + Int32.Parse(fileLine.Substring(3, 4), System.Globalization.NumberStyles.HexNumber);
                            int recordType = Int32.Parse(fileLine.Substring(7, 2), System.Globalization.NumberStyles.HexNumber);

                            if (recordType == 0)
                            { // Data Record}
                                if (fileLine.Length >= (11 + (2 * byteCount)))
                                { // skip if line isn't long enough for bytecount.                    

                                    for (int lineByte = 0; lineByte < byteCount; lineByte++)
                                    {
                                        int byteAddress = fileAddress + lineByte;
                                        // compute array address from hex file address # bytes per memory location
                                        int arrayAddress = (byteAddress - (int)programMemStart) / bytesPerWord;
                                        // compute byte position withing memory word
                                        int bytePosition = byteAddress % bytesPerWord;
                                        // get the byte value from hex file
                                        uint wordByte = 0xFFFFFF00 | UInt32.Parse(fileLine.Substring((9 + (2 * lineByte)), 2), System.Globalization.NumberStyles.HexNumber);
                                        // shift the byte into its proper position in the word.
                                        for (int shift = 0; shift < bytePosition; shift++)
                                        { // shift byte into proper position
                                            wordByte <<= 8;
                                            wordByte |= 0xFF; // shift in ones.
                                        }

                                        lineExceedsFlash = true; // if not in any memory section, then error

                                        // program memory section --------------------------------------------------
                                        if ((byteAddress >= programMemStart) && (byteAddress < progMemSizeBytes))
                                        {
                                            if (progMem)
                                            { // if importing program memory
                                                Pk2.DeviceBuffers.ProgramMemory[arrayAddress] &= wordByte; // add byte.
                                            }
                                            lineExceedsFlash = false;
                                            //NOTE: program memory locations containing config words may get modified
                                            // by the config section below that applies the config masks.
                                        }

                                        // boot memory section --------------------------------------------------
                                        if ((bootMemSize > 0) && (byteAddress >= bootMemStart) && (byteAddress < bootMemEnd))
                                        {
                                            arrayAddress = (int)(bootArrayStart + ((byteAddress - bootMemStart) / bytesPerWord));
                                            if (progMem)
                                            { // if importing program memory
                                                Pk2.DeviceBuffers.ProgramMemory[arrayAddress] &= wordByte; // add byte.
                                            }
                                            lineExceedsFlash = false;
                                            //NOTE: program memory locations containing config words may get modified
                                            // by the config section below that applies the config masks.
                                        }

                                        // EE data section ---------------------------------------------------------
                                        if ((byteAddress >= eeAddr) && (eeAddr > 0) && (Pk2.DevFile.PartsList[Pk2.ActivePart].EEMem > 0))
                                        {
                                            int eeAddress = (int)(byteAddress - eeAddr) / eeMemBytes;
                                            if (eeAddress < Pk2.DevFile.PartsList[Pk2.ActivePart].EEMem)
                                            {
                                                lineExceedsFlash = false;
                                                if (eeMem)
                                                { // skip if not importing EE Memory
                                                    if (eeMemBytes == bytesPerWord)
                                                    { // same # hex bytes per EE location as ProgMem location
                                                        Pk2.DeviceBuffers.EEPromMemory[eeAddress] &= wordByte; // add byte.    
                                                    }
                                                    else
                                                    {  // PIC18F/J
                                                        int eeshift = (bytePosition / eeMemBytes) * eeMemBytes;
                                                        for (int reshift = 0; reshift < eeshift; reshift++)
                                                        { // shift byte into proper position
                                                            wordByte >>= 8;
                                                        }
                                                        Pk2.DeviceBuffers.EEPromMemory[eeAddress] &= wordByte; // add byte. 
                                                    }
                                                }
                                            }
                                        }
                                        // Some 18F parts without EEPROM have hex files created with blank EEPROM by MPLAB
                                        else if ((byteAddress >= eeAddr) && (eeAddr > 0) && (Pk2.DevFile.PartsList[Pk2.ActivePart].EEMem == 0))
                                        {
                                            lineExceedsFlash = false; // don't give too-large file error.
                                        }
                                        // Config words section ----------------------------------------------------
                                        if ((byteAddress >= Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr)
                                                && (configWords > 0))
                                        {
                                            int configNum;
                                            if (Pk2.NewStyleConfigs())
                                            {
                                                configNum = (byteAddress - ((int)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr)) / cfgBytesPerWord;
                                                if (configNum > 0 && configNum < 8)
                                                    configNum = 255;
                                                else if (configNum % 2 != 0)
                                                    configNum = 255;
                                                else if (configNum > 0)
                                                    configNum = (configNum - 6) / 2;
                                            }
                                            else
                                            {
                                                configNum = (byteAddress - ((int)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr)) / cfgBytesPerWord;
                                            }

                                            if ((cfgBytesPerWord != bytesPerWord) && (bytePosition > 1))
                                            { // PIC32
                                                wordByte = (wordByte >> 16) & Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue;
                                            }
                                            if (configNum < Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords)
                                            {
                                                lineExceedsFlash = false;
                                                configRead = true;
                                                configLoaded[configNum] = true;
                                                if (progMem)
                                                { // if importing program memory
                                                    Pk2.DeviceBuffers.ConfigWords[configNum] &=
                                                           wordByte;
                                                    //    (wordByte & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[configNum]);
                                                    if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue == 0xFFF)
                                                    { // baseline, set OR mask bits
                                                        Pk2.DeviceBuffers.ConfigWords[configNum] |= Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[5];
                                                    }
                                                    if (byteAddress < progMemSizeBytes)
                                                    { // also mask off the word if in program memory.
                                                        uint orMask = 0;
                                                        if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue == 0xFFFF)
                                                        {//PIC18J
                                                            orMask = 0xF000;
                                                        }
                                                        else
                                                        { // PIC24 is currently only other case of config in program mem
                                                            orMask = (uint)(0xFF0000 | (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[configNum]
                                                                & ~Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[configNum]));
                                                        }
                                                        Pk2.DeviceBuffers.ProgramMemory[arrayAddress] &=
                                                            //       wordByte;
                                                            (wordByte & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[configNum]); // add byte.

                                                        Pk2.DeviceBuffers.ProgramMemory[arrayAddress] |= orMask;
                                                    }
                                                }
                                            }

                                        }

                                        // User IDs section ---------------------------------------------------------
                                        if (userIDs > 0)
                                        {
                                            if (byteAddress >= userIDAddr)
                                            {
                                                int uIDAddress = (int)(byteAddress - userIDAddr) / userIDMemBytes;
                                                if (uIDAddress < userIDs)
                                                {
                                                    lineExceedsFlash = false;
                                                    if (progMem)
                                                    { // if importing program memory
                                                        if (userIDMemBytes == bytesPerWord)
                                                        { // same # hex bytes per EE location as ProgMem location
                                                            Pk2.DeviceBuffers.UserIDs[uIDAddress] &= wordByte; // add byte.    
                                                        }
                                                        else
                                                        {  // PIC18F/J, PIC24H/dsPIC33
                                                            int uIDshift = (bytePosition / userIDMemBytes) * userIDMemBytes;
                                                            for (int reshift = 0; reshift < uIDshift; reshift++)
                                                            { // shift byte into proper position
                                                                wordByte >>= 8;
                                                            }
                                                            Pk2.DeviceBuffers.UserIDs[uIDAddress] &= wordByte; // add byte. 
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        // ignore data in hex file 
                                        if (Pk2.DevFile.PartsList[Pk2.ActivePart].IgnoreBytes > 0)
                                        {
                                            if (byteAddress >= Pk2.DevFile.PartsList[Pk2.ActivePart].IgnoreAddress)
                                            {
                                                if (byteAddress < (Pk2.DevFile.PartsList[Pk2.ActivePart].IgnoreAddress
                                                                    + Pk2.DevFile.PartsList[Pk2.ActivePart].IgnoreBytes))
                                                { // if data is in the ignore region, don't do anything with it
                                                  // but don't generate a "hex file larger than device" warning.
                                                    lineExceedsFlash = false;
                                                }
                                            }
                                        }

                                        // test memory section ---------------------------------------------------------
                                        if (FormPICkit2.TestMemoryEnabled && FormPICkit2.TestMemoryOpen)
                                        {
                                            if (FormPICkit2.formTestMem.HexImportExportTM())
                                            {
                                                if ((byteAddress >= Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart)
                                                     && (Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart > 0)
                                                     && (FormPICkit2.TestMemoryWords > 0))
                                                {
                                                    int tmAddress =
                                                        (int)(byteAddress - Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart)
                                                        / bytesPerWord;
                                                    if (tmAddress < FormPICkit2.TestMemoryWords)
                                                    {
                                                        lineExceedsFlash = false;
                                                        FormTestMemory.TestMemory[tmAddress] &= wordByte; // add byte.
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }

                                if (lineExceedsFlash)
                                {
                                    fileExceedsFlash = true;
                                }

                            } // end if (recordType == 0)  

                            if ((recordType == 2) || (recordType == 4))
                            { // Segment address
                                if (fileLine.Length >= (11 + (2 * byteCount)))
                                { // skip if line isn't long enough for bytecount.                                                    
                                    segmentAddress = Int32.Parse(fileLine.Substring(9, 4), System.Globalization.NumberStyles.HexNumber);
                                }
                                if (recordType == 2)
                                {
                                    segmentAddress <<= 4;
                                }
                                else
                                {
                                    segmentAddress <<= 16;
                                }

                            } // end if ((recordType == 2) || (recordType == 4)) 

                            if (recordType == 1)
                            { // end of record
                                break;
                            }

                            if (hexFile.Extension.ToUpperInvariant() == ".NUM")
                            { // Only read first line of SQTP file
                                break;
                            }
                        }
                    }
                    fileLine = hexRead.ReadLine();
                    //endOfHexFile = ((fileLine = hexRead.ReadLine()) == null);

                }
                hexRead.Close();

                if (configWords > 0)
                {
                    if (!configRead)
                    {
                        return Constants.FileRead.noconfig;
                    }
                    for (int cw = 0; cw < configWords; cw++)
                    {
                        if (!configLoaded[cw])
                        {
                            // apply mask to dsPIC33/PIC24HJ config 8 to make sure JTAG bit is cleared
                            if ((Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue == 0xFFFFFF) && (configWords > 7))
                            {
                                Pk2.DeviceBuffers.ConfigWords[7] &= Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[7];
                            }
                            return Constants.FileRead.partialcfg;
                        }
                    }   
                }             
                if (fileExceedsFlash)
                {
                    return Constants.FileRead.largemem;
                }
                return Constants.FileRead.success;
            }
            catch 
            {
                return Constants.FileRead.failed;
            }
        }
        
        public static Constants.FileRead ImportBINFile(string filePath)
        { // for serial EEPROMS only
            try
            {
                FileInfo binDataFile = new FileInfo(filePath);
                LastWriteTime = binDataFile.LastWriteTime; 
                FileStream binFile = File.OpenRead(filePath);
                int bytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].ProgMemHexBytes;
                int memLoc = 0;
                int bytePosition = 0;
                byte[] fileByte = new byte[1];
                while (binFile.Read(fileByte, 0, 1) > 0)
                {
                    if (memLoc >= Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem)
                        return Constants.FileRead.largemem;

                    uint memByte = 0xFFFFFF00 | (uint)fileByte[0];
                    for (int shift = 0; shift < bytePosition; shift++)
                    { // shift byte into proper position
                        memByte <<= 8;
                        memByte |= 0xFF; // shift in ones.
                    }
                    Pk2.DeviceBuffers.ProgramMemory[memLoc] &= memByte;
                    if (++bytePosition >= bytesPerWord)
                    {
                        memLoc++;
                        bytePosition = 0;
                    }
                }

                binFile.Close();
            }
            catch
            {
                return Constants.FileRead.failed;
            }

            return Constants.FileRead.success;
        }
        
        public static bool ExportHexFile(string filePath, bool progMem, bool eeMem)
        {
            if (filePath.Length > 4)
            {
                if ((filePath.Substring(filePath.Length - 4).ToUpperInvariant() == ".BIN") && Pk2.FamilyIsEEPROM())
                {
                    return ExportBINFile(filePath);
                }
            }
        
            StreamWriter hexFile = new StreamWriter(filePath);
            
            // Start with segment zero
            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue > 0xFFFFFF)
            { // PIC32
                hexFile.WriteLine(":020000041D00DD");
            }
            else
            {
                hexFile.WriteLine(":020000040000FA");
            }
            // Program Memory ----------------------------------------------------------------------------
            int fileSegment = 0;
            int fileAddress = 0;  
            int programEnd = Pk2.DeviceBuffers.ProgramMemory.Length;          
            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue > 0xFFFFFF)
            { // PIC32
                fileSegment = (int)(Constants.P32_PROGRAM_FLASH_START_ADDR >> 16);
                fileAddress = (int)(Constants.P32_PROGRAM_FLASH_START_ADDR & 0xFFFF);
                programEnd -= (int)Pk2.DevFile.PartsList[Pk2.ActivePart].BootFlash;
            }
            int arrayIndex = 0;
            int bytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].ProgMemHexBytes;
            int arrayIncrement = 16 / bytesPerWord;     // # array words per hex line.
            if (progMem)
            {
                do
                {
                    string hexLine = string.Format(":10{0:X4}00", fileAddress);
                    for (int i = 0; i < arrayIncrement; i++)
                    {
                        // convert entire array word to hex string of 4 bytes.
                        string hexWord = "00000000";
                        if ((arrayIndex + i) < Pk2.DeviceBuffers.ProgramMemory.Length)
                        {
                            hexWord = string.Format("{0:X8}", Pk2.DeviceBuffers.ProgramMemory[arrayIndex + i]);
                        }
                        for (int j = 0; j < bytesPerWord; j++)
                        {
                            hexLine += hexWord.Substring((6 - 2 * j), 2);
                        }
                    }
                    hexLine += string.Format("{0:X2}", computeChecksum(hexLine));
                    hexFile.WriteLine(hexLine);
                    
                    fileAddress += 16;
                    arrayIndex += arrayIncrement;
                    
                    // check for segment boundary
                    if ((fileAddress > 0xFFFF) && (arrayIndex < Pk2.DeviceBuffers.ProgramMemory.Length))
                    {
                        fileSegment += fileAddress >> 16;
                        fileAddress &= 0xFFFF;
                        string segmentLine = string.Format(":02000004{0:X4}", fileSegment);
                        segmentLine += string.Format("{0:X2}", computeChecksum(segmentLine));
                        hexFile.WriteLine(segmentLine); 
                                          
                    }
                
                } while (arrayIndex < programEnd);
            }
            // Boot Memory ----------------------------------------------------------------------------
            if ((Pk2.DevFile.PartsList[Pk2.ActivePart].BootFlash > 0) && Pk2.FamilyIsPIC32())
            {
                hexFile.WriteLine(":020000041FC01B");
                arrayIndex = programEnd;
                programEnd = Pk2.DeviceBuffers.ProgramMemory.Length;          
                fileSegment = (int)(Constants.P32_BOOT_FLASH_START_ADDR >> 16);
                fileAddress = (int)(Constants.P32_BOOT_FLASH_START_ADDR & 0xFFFF);
                if (progMem)
                {
                    do
                    {
                        string hexLine = string.Format(":10{0:X4}00", fileAddress);
                        for (int i = 0; i < arrayIncrement; i++)
                        {
                            // convert entire array word to hex string of 4 bytes.
                            string hexWord = string.Format("{0:X8}", Pk2.DeviceBuffers.ProgramMemory[arrayIndex + i]);
                            for (int j = 0; j < bytesPerWord; j++)
                            {
                                hexLine += hexWord.Substring((6 - 2 * j), 2);
                            }
                        }
                        hexLine += string.Format("{0:X2}", computeChecksum(hexLine));
                        hexFile.WriteLine(hexLine);
                        
                        fileAddress += 16;
                        arrayIndex += arrayIncrement;
                        
                        // check for segment boundary
                        if ((fileAddress > 0xFFFF) && (arrayIndex < Pk2.DeviceBuffers.ProgramMemory.Length))
                        {
                            fileSegment += fileAddress >> 16;
                            fileAddress &= 0xFFFF;
                            string segmentLine = string.Format(":02000004{0:X4}", fileSegment);
                            segmentLine += string.Format("{0:X2}", computeChecksum(segmentLine));
                            hexFile.WriteLine(segmentLine); 
                                              
                        }

                    } while (arrayIndex < programEnd);
                }   
            }         
            // EEPROM -------------------------------------------------------------------------------------
            if (eeMem)
            {
                int eeSize = Pk2.DevFile.PartsList[Pk2.ActivePart].EEMem;
                arrayIndex = 0;
                if (eeSize > 0)
                {
                    uint eeAddr = Pk2.DevFile.PartsList[Pk2.ActivePart].EEAddr;
                    if ((eeAddr & 0xFFFF0000) > 0)
                    { // need a segment address
                        string segmentLine = string.Format(":02000004{0:X4}", (eeAddr >> 16));
                        segmentLine += string.Format("{0:X2}", computeChecksum(segmentLine));
                        hexFile.WriteLine(segmentLine);
                    }
                    
                    fileAddress = (int)eeAddr & 0xFFFF;
                    int eeBytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].EEMemHexBytes;
                    arrayIncrement = 16 / eeBytesPerWord;     // # array words per hex line.
                    do
                    {
                        string hexLine = string.Format(":10{0:X4}00", fileAddress);
                        for (int i = 0; i < arrayIncrement; i++)
                        {
                            // convert entire array word to hex string of 4 bytes.
                            string hexWord = string.Format("{0:X8}", Pk2.DeviceBuffers.EEPromMemory[arrayIndex + i]);
                            for (int j = 0; j < eeBytesPerWord; j++)
                            {
                                hexLine += hexWord.Substring((6 - 2 * j), 2);
                            }
                        }
                        hexLine += string.Format("{0:X2}", computeChecksum(hexLine));
                        hexFile.WriteLine(hexLine);

                        fileAddress += 16;
                        arrayIndex += arrayIncrement;                
                    }while (arrayIndex < Pk2.DeviceBuffers.EEPromMemory.Length);
                
                }
            }
            // Configuration Words ------------------------------------------------------------------------
            if (progMem)
            {
                int cfgBytesPerWord = bytesPerWord;
                if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue > 0xFFFFFF)
                { // PIC32
                    cfgBytesPerWord = 2;
                }
                int configWords = Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords;
                if ((configWords > 0) && (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr >
                    (Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem * bytesPerWord)))
                { // If there are Config words and they aren't at the end of program flash
                    uint configAddr = Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr;
                    if ((configAddr & 0xFFFF0000) > 0)
                    { // need a segment address
                        string segmentLine = string.Format(":02000004{0:X4}", (configAddr >> 16));
                        segmentLine += string.Format("{0:X2}", computeChecksum(segmentLine));
                        hexFile.WriteLine(segmentLine);
                    }

                    fileAddress = (int)configAddr & 0xFFFF;

                    int cfgsWritten = 0;
                    for (int lines = 0; lines < (((configWords * cfgBytesPerWord - 1)/16) + 1); lines++)
                    {
                        int cfgsLeft = configWords - cfgsWritten;
                        if (cfgsLeft >= (16 / cfgBytesPerWord))
                        {
                            cfgsLeft = (16 / cfgBytesPerWord);
                        }
                        int numConfBytesOnLine = cfgsLeft * cfgBytesPerWord;
                        bool lastConfByteOdd = false;
                        if (((Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemPanelBufs & 0x04) == 0x04)
                            && ((configWords - cfgsWritten) <= (16 / cfgBytesPerWord)))
                        {
                            lastConfByteOdd = true;
                            numConfBytesOnLine--;     // It is last 'missing' config byte of Q83,Q84,Q71 etc.
                        }
                        // string hexLine = string.Format(":{0:X2}{1:X4}00", (cfgsLeft * cfgBytesPerWord), fileAddress);    // orig.
                        string hexLine = string.Format(":{0:X2}{1:X4}00", numConfBytesOnLine, fileAddress);

                        fileAddress += (cfgsLeft * cfgBytesPerWord);
                        for (int i = 0; i < cfgsLeft; i++)
                        {
                            // convert entire array word to hex string of 4 bytes.
                            uint cfgWord = Pk2.DeviceBuffers.ConfigWords[cfgsWritten + i];
                            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue > 0xFFFFFF)
                            {// PIC32
                                cfgWord |= ~(uint)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[cfgsWritten + i];
                                cfgWord &= Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigBlank[cfgsWritten + i];
                            }
                            string hexWord = string.Format("{0:X8}", cfgWord);
                            if (i == (cfgsLeft - 1) && lastConfByteOdd) // Last, partial config word (only one byte) of Q71,83,84
                            {
                                hexLine += hexWord.Substring(6, 2);
                            }
                            else        // 'Normal' write of config word
                            {
                                for (int j = 0; j < cfgBytesPerWord; j++)
                                {
                                    hexLine += hexWord.Substring(8 - ((j + 1) * 2), 2);
                                }
                            }
                        }
                        hexLine += string.Format("{0:X2}", computeChecksum(hexLine));
                        hexFile.WriteLine(hexLine);
                        cfgsWritten += cfgsLeft;
                   }               
                }
            }
            
            // UserIDs ------------------------------------------------------------------------------------
            if (progMem)
            {
                int userIDs = Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDWords;
                arrayIndex = 0;
                if (userIDs > 0)
                {
                    uint uIDAddr = Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDAddr;
                    if ((uIDAddr & 0xFFFF0000) > 0)
                    { // need a segment address
                        string segmentLine = string.Format(":02000004{0:X4}", (uIDAddr >> 16));
                        segmentLine += string.Format("{0:X2}", computeChecksum(segmentLine));
                        hexFile.WriteLine(segmentLine);
                    }

                    fileAddress = (int)uIDAddr & 0xFFFF;
                    int idBytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].UserIDHexBytes;
                    arrayIncrement = 16 / idBytesPerWord;     // # array words per hex line.
                    string hexLine;
                    do
                    {
                        int remainingBytes = (userIDs - arrayIndex) * idBytesPerWord;
                        if (remainingBytes < 16)
                        {
                            hexLine = string.Format(":{0:X2}{1:X4}00", remainingBytes, fileAddress);
                            arrayIncrement = (userIDs - arrayIndex);
                        }
                        else
                        {
                            hexLine = string.Format(":10{0:X4}00", fileAddress);
                        }
                        for (int i = 0; i < arrayIncrement; i++)
                        {
                            // convert entire array word to hex string of 4 bytes.
                            string hexWord = string.Format("{0:X8}", Pk2.DeviceBuffers.UserIDs[arrayIndex + i]);
                            for (int j = 0; j < idBytesPerWord; j++)
                            {
                                hexLine += hexWord.Substring((6 - 2 * j), 2);
                            }
                        }
                        hexLine += string.Format("{0:X2}", computeChecksum(hexLine));
                        hexFile.WriteLine(hexLine);

                        fileAddress += 16;
                        arrayIndex += arrayIncrement;
                    } while (arrayIndex < Pk2.DeviceBuffers.UserIDs.Length);
                }
            }
            // Test Memory --------------------------------------------------------------------------------
            if (FormPICkit2.TestMemoryEnabled && FormPICkit2.TestMemoryOpen)
            {
                if (FormPICkit2.formTestMem.HexImportExportTM())
                {
                    int tmSize = FormPICkit2.TestMemoryWords;
                    arrayIndex = 0;
                    if (tmSize > 0)
                    {
                        uint tmAddr = Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart;
                        if ((tmAddr & 0xFFFF0000) > 0)
                        { // need a segment address
                            string segmentLine = string.Format(":02000004{0:X4}", (tmAddr >> 16));
                            segmentLine += string.Format("{0:X2}", computeChecksum(segmentLine));
                            hexFile.WriteLine(segmentLine);
                        }

                        fileAddress = (int)tmAddr & 0xFFFF;
                        int tmBytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].ProgMemHexBytes;
                        arrayIncrement = 16 / tmBytesPerWord;     // # array words per hex line.
                        do
                        {
                            string hexLine = string.Format(":10{0:X4}00", fileAddress);
                            for (int i = 0; i < arrayIncrement; i++)
                            {
                                // convert entire array word to hex string of 4 bytes.
                                string hexWord = string.Format("{0:X8}", FormTestMemory.TestMemory[arrayIndex + i]);
                                for (int j = 0; j < tmBytesPerWord; j++)
                                {
                                    hexLine += hexWord.Substring((6 - 2 * j), 2);
                                }
                            }
                            hexLine += string.Format("{0:X2}", computeChecksum(hexLine));
                            if ((fileAddress != ((int)tmAddr & 0xFFFF)) || (Pk2.GetActiveFamily() != 3))
                            { // skip User ID line on PIC18F
                                hexFile.WriteLine(hexLine);
                            }

                            fileAddress += 16;
                            arrayIndex += arrayIncrement;
                        } while (arrayIndex < FormPICkit2.TestMemoryWords);
                    }
                 }            
            }
            //end of record line.
            hexFile.WriteLine(":00000001FF");
            hexFile.Close();
            return true;
        }
        
        public static bool ExportBINFile(string filePath)
        { // for serial EEPROMS only
        
            try
            {
                FileStream binFile = File.Open(filePath,System.IO.FileMode.Create);
                int bytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].ProgMemHexBytes;
                for (int memLoc = 0; memLoc < Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem; memLoc++)
                {
                    for (int byteNum = 0; byteNum < bytesPerWord; byteNum++)
                    {
                        byte outputByte = (byte)((Pk2.DeviceBuffers.ProgramMemory[memLoc] >> (8 * byteNum)) & 0xFF);
                        binFile.WriteByte(outputByte);
                    }
                }
                binFile.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
        
        private static byte computeChecksum(string fileLine)
        {
            int byteCount = Int32.Parse(fileLine.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            if (fileLine.Length >= (9 + (2* byteCount)))
            { // skip if line isn't long enough for bytecount.             
                 int checksum = byteCount;
                 for (int i = 0; i < (3 + byteCount); i++)
                 {
                    checksum += Int32.Parse(fileLine.Substring(3 + (2 * i), 2), System.Globalization.NumberStyles.HexNumber);
                 }
                 checksum = 0 - checksum;
                 return (byte)(checksum & 0xFF);
            }
            
            return 0;              
        }
    }
}
