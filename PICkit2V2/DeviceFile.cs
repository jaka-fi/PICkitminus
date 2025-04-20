using System;
using System.Collections.Generic;
using System.Text;

namespace PICkit2V2
{
    public class DeviceFile
    {
        public DeviceFileParams Info = new DeviceFileParams();
        public DeviceFamilyParams[] Families;
        public DevicePartParams[] PartsList;
        public DeviceScripts[] Scripts;

        public struct DeviceFileParams
            {
            // One instance of this structure is included at the start of the Device File
            public int VersionMajor;    // Device file version number major.minor.dot
            public int VersionMinor;
            public int VersionDot;

            public string VersionNotes; // Max 512 characters

            public int NumberFamilies;  // # number of DeviceFamilyParams sets
            public int NumberParts;     // # number of DevicePartParams sets
            public int NumberScripts;   // # number of DeviceScripts sets
            public byte Compatibility;
            public byte UNUSED1A;
            public ushort UNUSED1B;
            public uint UNUSED2;

            }

        public struct DeviceFamilyParams
            {
            // a single struct instance describes the parameters for an entire part family.
            public ushort FamilyID;             // # essentially, its array index number.
            public ushort FamilyType;           // also used as the display order in the Device Family Menu - lower first
            public ushort SearchPriority;
            public string FamilyName;           // 16 -> 24 chars max (v2.50)
            public ushort ProgEntryScript;
            public ushort ProgExitScript;
            public ushort ReadDevIDScript;
            public uint DeviceIDMask;           // HEX
            public uint BlankValue;             // HEX
            public byte BytesPerLocation;
            public byte AddressIncrement; 
            public bool PartDetect;
            public ushort ProgEntryVPPScript;   // program entry VPP first
            public ushort UNUSED1;    
            public byte EEMemBytesPerWord;
            public byte EEMemAddressIncrement;
            public byte UserIDHexBytes;
            public byte UserIDBytes;
            public byte ProgMemHexBytes;   // added 7-10-06
	        public byte EEMemHexBytes;   // added 7-10-06
            public byte ProgMemShift;   // added 7-10-06
            public uint TestMemoryStart;        // HEX
            public ushort TestMemoryLength;
            public float Vpp;
            }

        public struct DevicePartParams
            {
            // a single struct instance describes parameters for a single silicon part.
            public string PartName;             // 20 chars max
            public ushort Family;               // references FamilyID in DeviceFamilyParams
            public uint DeviceID;
            public uint ProgramMem;
            public ushort EEMem;
            public uint EEAddr;
            public byte ConfigWords;
            public uint ConfigAddr;             // HEX
            public byte UserIDWords;
            public uint UserIDAddr;             // HEX
            public uint BandGapMask;            // HEX
            public ushort[] ConfigMasks;        // HEX 
            public ushort[] ConfigBlank;        // HEX 
            public ushort CPMask;               // HEX
            public byte CPConfig;
            public bool OSSCALSave;
            public uint IgnoreAddress;          // HEX
            public float VddMin;
            public float VddMax;
            public float VddErase;
            public byte CalibrationWords;
            public ushort ChipEraseScript;
            public ushort ProgMemAddrSetScript;
            public byte ProgMemAddrBytes;
            public ushort ProgMemRdScript;
            public ushort ProgMemRdWords;
            public ushort EERdPrepScript;
            public ushort EERdScript;
            public ushort EERdLocations;
            public ushort UserIDRdPrepScript;
            public ushort UserIDRdScript;
            public ushort ConfigRdPrepScript;
            public ushort ConfigRdScript;
            public ushort ProgMemWrPrepScript;
            public ushort ProgMemWrScript;
            public ushort ProgMemWrWords;
            public byte ProgMemPanelBufs;
            public uint ProgMemPanelOffset;
            public ushort EEWrPrepScript;
            public ushort EEWrScript;
            public ushort EEWrLocations;
            public ushort UserIDWrPrepScript;
            public ushort UserIDWrScript;
            public ushort ConfigWrPrepScript;
            public ushort ConfigWrScript;
            public ushort OSCCALRdScript;
            public ushort OSCCALWrScript;
            public ushort DPMask;
            public bool WriteCfgOnErase;
            public bool BlankCheckSkipUsrIDs;
            public ushort IgnoreBytes;            
            public ushort ChipErasePrepScript;
            public uint BootFlash;
            //public uint UNUSED4;
             public ushort Config9Mask;
             public ushort Config9Blank;
            public ushort ProgMemEraseScript; // added 7-10-06
	        public ushort EEMemEraseScript; // added 7-10-06
	        public ushort ConfigMemEraseScript; // added 7-10-06
	        public ushort reserved1EraseScript; // added 7-10-06
	        public ushort reserved2EraseScript; // added 7-10-06
	        public ushort TestMemoryRdScript;
	        public ushort TestMemoryRdWords;
	        public ushort EERowEraseScript;
	        public ushort EERowEraseWords;
	        public bool ExportToMPLAB;
	        public ushort DebugHaltScript;
	        public ushort DebugRunScript;
	        public ushort DebugStatusScript;
	        public ushort DebugReadExecVerScript;
	        public ushort DebugSingleStepScript;
	        public ushort DebugBulkWrDataScript;
	        public ushort DebugBulkRdDataScript;
            public ushort DebugWriteVectorScript;
            public ushort DebugReadVectorScript;
            public ushort DebugRowEraseScript;
            public ushort DebugRowEraseSize;
            public ushort DebugReserved5Script;
            public ushort DebugReserved6Script;
            public ushort DebugReserved7Script;
            public ushort DebugReserved8Script;
            //public ushort DebugReserved9Script;
             public ushort LVPScript;
            }

        public struct DeviceScripts
            {
            public ushort ScriptNumber;         // # Essentially, its array index number - 1 based 0 reserved for no script
            // referred to in the XxxxxxScript fields of DevicePartParams
            public string ScriptName;           // 20 Chars max
            public ushort ScriptVersion;        // increments on each change
            public uint UNUSED1;
            public ushort ScriptLength;
            public ushort[] Script;
            public string Comment;            // 20 max
            }
        
    }
}
