using System;
using System.Collections.Generic;
using System.Text;

namespace PICkit2V2
{
    public class DeviceData
    {
        public uint[] ProgramMemory;
        public uint[] EEPromMemory;
        public uint[] ConfigWords;
        public uint[] UserIDs;
        public uint OSCCAL;
        public uint BandGap;
       
        public DeviceData(uint progMemSize, ushort eeMemSize, byte numConfigs, ushort numIDs, 
                            uint memBlankVal, int eeBytes, int idBytes, ushort[] configBlank, uint OSCCALInit)
        {   // Overloaded Constructor
            ProgramMemory = new uint[progMemSize];
            EEPromMemory = new uint[eeMemSize];
            ConfigWords = new uint[numConfigs];
            UserIDs = new uint[numIDs];
            
            //init program memory to blank
           ClearProgramMemory(memBlankVal);
            
            //init eeprom to blank
            ClearEEPromMemory(eeBytes, memBlankVal);

            //init configuration to blank
            ClearConfigWords(configBlank);
            
            //init user ids to blank
            ClearUserIDs(idBytes, memBlankVal);
            
            //init OSSCAL & BandGap
            OSCCAL = OSCCALInit | 0xFF;
            BandGap = memBlankVal;
        
        }

        public void ClearProgramMemory(uint memBlankVal)
        {
            if (ProgramMemory.Length > 0)
            {
                for (int i = 0; i < ProgramMemory.Length; i++)
                {
                    ProgramMemory[i] = memBlankVal;
                }
            }
        }


        public void ClearConfigWords(ushort[] configBlank)
        {
            if (ConfigWords.Length > 0)
            {
                //init configuration to blank
                for (int i = 0; i < ConfigWords.Length; i++)
                {
                    ConfigWords[i] = configBlank[i];
                }
            }
        }

        public void ClearUserIDs(int idBytes, uint memBlankVal)
        {
            if (UserIDs.Length > 0)
            {
                //init user ids to blank
                uint idBlank = memBlankVal;
                if (idBytes == 1)
                {
                    idBlank = 0xFF;
                }
                for (int i = 0; i < UserIDs.Length; i++)
                {
                    UserIDs[i] = idBlank;
                }
            }
        }

        public void ClearEEPromMemory(int eeBytes, uint memBlankVal)
        {
            if (EEPromMemory.Length > 0)
            {
                //init eeprom to blank
                uint eeBlankVal = 0xFF;
                if (eeBytes == 2)
                {
                    eeBlankVal = 0xFFFF;
                }
                if (memBlankVal == 0xFFF)
                { // baseline dataflash
                    eeBlankVal = 0xFFF;
                }
                for (int i = 0; i < EEPromMemory.Length; i++)
                {
                    EEPromMemory[i] = eeBlankVal;                  // 8-bit eeprom will just use 8 LSBs
                }
            }
        }        
    }
}
