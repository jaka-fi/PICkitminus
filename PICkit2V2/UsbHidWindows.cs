using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using KONST = PICkit2V2.Constants;

namespace PICkit2V2
{
    public class UsbHidWindows : IUsbHidTransport
    {
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        private const int INVALID_HANDLE_VALUE = -1;
        private const short OPEN_EXISTING = 3;
        // from setupapi.h
        private const short DIGCF_PRESENT = 0x00000002;
        private const short DIGCF_DEVICEINTERFACE = 0x00000010;
        //
        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public System.Guid InterfaceClassGuid;
            public int Flags;
            public int Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            public string DevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public System.Guid ClassGuid;
            public int DevInst;
            public int Reserved;
        }

        //
        public struct HIDD_ATTRIBUTES
        {
            public int Size;
            public ushort VendorID;
            public ushort ProductID;
            public ushort VersionNumber;
        }
        //
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public int lpSecurityDescriptor;
            public int bInheritHandle;
        }
        //
        [StructLayout(LayoutKind.Sequential)]
        public struct HIDP_CAPS
        {
            public short Usage;
            public short UsagePage;
            public short InputReportByteLength;
            public short OutputReportByteLength;
            public short FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public short[] Reserved;
            public short NumberLinkCollectionNodes;
            public short NumberInputButtonCaps;
            public short NumberInputValueCaps;
            public short NumberInputDataIndices;
            public short NumberOutputButtonCaps;
            public short NumberOutputValueCaps;
            public short NumberOutputDataIndices;
            public short NumberFeatureButtonCaps;
            public short NumberFeatureValueCaps;
            public short NumberFeatureDataIndices;

        }
        //
        // DLL imnports
        //
        [DllImport("hid.dll")]
        static public extern void HidD_GetHidGuid(ref System.Guid HidGuid);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(ref System.Guid ClassGuid, string Enumerator, int hwndParent, int Flags);

        [DllImport("setupapi.dll")]
        static public extern int SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, int DeviceInfoData, ref System.Guid InterfaceClassGuid, int MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static public extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, IntPtr DeviceInfoData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static public extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, ref SECURITY_ATTRIBUTES lpSecurityAttributes, int dwCreationDisposition, uint dwFlagsAndAttributes, int hTemplateFile);

        [DllImport("hid.dll")]
        static public extern int HidD_GetAttributes(IntPtr HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);

        [DllImport("hid.dll")]
        static public extern bool HidD_GetPreparsedData(IntPtr HidDeviceObject, ref IntPtr PreparsedData);

        [DllImport("hid.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static public extern bool HidD_GetSerialNumberString(IntPtr HidDeviceObject, IntPtr Buffer, uint BufferLength);

        [DllImport("hid.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static public extern bool HidD_GetProductString(IntPtr HidDeviceObject, IntPtr Buffer, uint BufferLength);

        [DllImport("hid.dll")]
        static public extern int HidP_GetCaps(IntPtr PreparsedData, ref HIDP_CAPS Capabilities);

        [DllImport("setupapi.dll")]
        public static extern int SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("hid.dll")]
        static public extern bool HidD_FreePreparsedData(ref IntPtr PreparsedData);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe UInt32 WriteFile(     
            IntPtr hFile,                       // handle to file
            byte[] Buffer,                      // data buffer
            int numBytesToWrite,                // num of bytes to write
            ref int numBytesWritten,            // number of bytes actually written
            ref Kernel32.OVERLAPPED Overlapped  // overlapped buffer
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe UInt32 ReadFile(
              IntPtr hFile,                       // handle to file
              byte[] Buffer,                      // data buffer
              int NumberOfBytesToRead,            // number of bytes to read
              ref int pNumberOfBytesRead,         // number of bytes read
              ref Kernel32.OVERLAPPED Overlapped  // overlapped buffer
              );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 CancelIo(IntPtr hFile);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 ResetEvent(IntPtr hHandle);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        public Pk2DeviceInfo FindDevice(ushort p_index,
                                        out IntPtr p_ReadHandle,
                                        out IntPtr p_WriteHandle)
        {
            // Zero based p_index is used to identify which PICkit 2 we wish to talk to
            Pk2DeviceInfo info = new Pk2DeviceInfo();
            info.UnitId = "";
            info.ToolName = "PICkit 2";
            info.PkobModel = "PKOB";
            p_ReadHandle = IntPtr.Zero;
            p_WriteHandle = IntPtr.Zero;
            IntPtr DeviceInfoSet = IntPtr.Zero;
            IntPtr PreparsedDataPointer = IntPtr.Zero;
            HIDP_CAPS Capabilities = new HIDP_CAPS();
            System.Guid HidGuid;
            int Result;
            bool l_found_device;
            ushort l_num_found_devices = 0;
            IntPtr l_temp_handle = IntPtr.Zero;
            int BufferSize = 0;
            SP_DEVICE_INTERFACE_DATA MyDeviceInterfaceData;
            SP_DEVICE_INTERFACE_DETAIL_DATA MyDeviceInterfaceDetailData;
            string SingledevicePathName;
            SECURITY_ATTRIBUTES Security = new SECURITY_ATTRIBUTES();
            HIDD_ATTRIBUTES DeviceAttributes;
            IntPtr InvalidHandle = new IntPtr(-1);
            string unitIDSerial;
            string productName;
            //
            // initialize all
            //
            Security.lpSecurityDescriptor = 0;
            Security.bInheritHandle = System.Convert.ToInt32(true);
            Security.nLength = Marshal.SizeOf(Security);
            //
            HidGuid = Guid.Empty;
            //
            MyDeviceInterfaceData.cbSize = 0;
            MyDeviceInterfaceData.Flags = 0;
            MyDeviceInterfaceData.InterfaceClassGuid = Guid.Empty;
            MyDeviceInterfaceData.Reserved = 0;
            //
            MyDeviceInterfaceDetailData.cbSize = 0;
            MyDeviceInterfaceDetailData.DevicePath = "";
            //
            DeviceAttributes.ProductID = 0;
            DeviceAttributes.Size = 0;
            DeviceAttributes.VendorID = 0;
            DeviceAttributes.VersionNumber = 0;
            //
            l_found_device = false;
            Security.lpSecurityDescriptor = 0;
            Security.bInheritHandle = System.Convert.ToInt32(true);
            Security.nLength = Marshal.SizeOf(Security);

            HidD_GetHidGuid(ref HidGuid);
            DeviceInfoSet = SetupDiGetClassDevs(
                    ref HidGuid,
                    null,
                    0,
                    DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

            MyDeviceInterfaceData.cbSize = Marshal.SizeOf(MyDeviceInterfaceData);
            for (int l_loop = 0; l_loop < 64; l_loop++)             // JAKA: Increased loop count from 20 to 64. Fixed 'device not found' if computer has too many USB devices
            {
                Result = SetupDiEnumDeviceInterfaces(
                         DeviceInfoSet,
                         0,
                         ref HidGuid,
                         l_loop,
                         ref MyDeviceInterfaceData);
                if (Result != 0)
                {
                    SetupDiGetDeviceInterfaceDetail(DeviceInfoSet, ref MyDeviceInterfaceData, IntPtr.Zero, 0, ref BufferSize, IntPtr.Zero);
                    // Store the structure's size.
                    MyDeviceInterfaceDetailData.cbSize = Marshal.SizeOf(MyDeviceInterfaceDetailData);
                    // Allocate memory for the MyDeviceInterfaceDetailData Structure using the returned buffer size.
                    IntPtr DetailDataBuffer = Marshal.AllocHGlobal(BufferSize);
                    // Store cbSize in the first 4 bytes of the array
                    Marshal.WriteInt32(DetailDataBuffer, 4 + Marshal.SystemDefaultCharSize);
                    //Call SetupDiGetDeviceInterfaceDetail again.  
                    // This time, pass a pointer to DetailDataBuffer and the returned required buffer size.
                    SetupDiGetDeviceInterfaceDetail(DeviceInfoSet, ref MyDeviceInterfaceData, DetailDataBuffer, BufferSize, ref BufferSize, IntPtr.Zero);
                    // Skip over cbsize (4 bytes) to get the address of the devicePathName.
                    IntPtr pdevicePathName = new IntPtr(DetailDataBuffer.ToInt32() + 4);
                    // Get the String containing the devicePathName.
                    SingledevicePathName = Marshal.PtrToStringAuto(pdevicePathName);
                    l_temp_handle = CreateFile(
                                        SingledevicePathName,
                                        GENERIC_READ | GENERIC_WRITE,
                                        FILE_SHARE_READ | FILE_SHARE_WRITE,
                                        ref Security,
                                        OPEN_EXISTING,
                                        //0,
                                        FILE_FLAG_OVERLAPPED,
                                        0);
                    if (l_temp_handle != InvalidHandle)
                    {
                        // tried to use System.Threading.WaitHandle.InvalidHandle, but had access problems since it's protected
                        // The returned handle is valid, so find out if this is the device we're looking for.
                        // Set the Size property of DeviceAttributes to the number of bytes in the structure.
                        DeviceAttributes.Size = Marshal.SizeOf(DeviceAttributes);
                        Result = HidD_GetAttributes(l_temp_handle, ref DeviceAttributes);
                        if (Result != 0)
                        {
                            if (DeviceAttributes.VendorID == KONST.MChipVendorID &&
                                (DeviceAttributes.ProductID == KONST.Pk2DeviceID || 
                                DeviceAttributes.ProductID == KONST.Pk3DeviceID ||
                                DeviceAttributes.ProductID == KONST.PkobDeviceID))
                            {
                                if (DeviceAttributes.ProductID == KONST.Pk2DeviceID)
                                {
                                    info.IsPk3 = false;
                                    info.IsPkob = false;
                                
                                    // Detect if PICkit2 is PK2M
                                    IntPtr ptrBuffer = Marshal.AllocHGlobal(126);

                                    // get Product name string
                                    HidD_GetProductString(l_temp_handle, ptrBuffer, 126);
                                    productName = Marshal.PtrToStringUni(ptrBuffer, 64);
                                    Marshal.FreeHGlobal(ptrBuffer);

                                    if (((byte)productName[0] == 'P') && (productName[1] == 'K') && (productName[2] == '2') && (productName[3] == 'M'))
                                    {
                                        info.IsPk2m = true;
                                        info.ToolName = "PK2M";
                                    }
                                    else
                                    {
                                        info.IsPk2m = false;
                                        info.ToolName = "PICkit 2";
                                    }
                                }
                                else if (DeviceAttributes.ProductID == KONST.Pk3DeviceID)
                                {
                                    info.IsPk3 = true;
                                    info.IsPkob = false;
                                    info.ToolName = "PICkit 3";
                                }
                                else
                                {
                                    info.IsPk3 = true;
                                    info.IsPkob = true;
                                    info.ToolName = "PKOB";
                                    
                                    // Read PKOB model
                                    IntPtr ptrBuffer = Marshal.AllocHGlobal(126);

                                    // get Product name string
                                    HidD_GetProductString(l_temp_handle, ptrBuffer, 126);
                                    info.PkobModel = Marshal.PtrToStringUni(ptrBuffer, 64);
                                    Marshal.FreeHGlobal(ptrBuffer);

                                }
                                if (l_num_found_devices == p_index)
                                {
                                    IntPtr ptrBuffer = Marshal.AllocHGlobal(126);

                                    // found the correct one
                                    l_found_device = true;

                                    // get serial string (UnitID)
                                    HidD_GetSerialNumberString(l_temp_handle, ptrBuffer, 126);
                                    unitIDSerial = Marshal.PtrToStringUni(ptrBuffer,64);
                                    Marshal.FreeHGlobal(ptrBuffer);

                                    if (((byte)unitIDSerial[0] == '\t') || (unitIDSerial[0] == 0) || (unitIDSerial[0] == 0x409))
                                    {   // For some reason not clear to me, the blank PK2s return 
                                        // {0x09, 0x04, 0x00} in the first 3 bytes of the SN. The 
                                        // Unicode conversion turns this to character "CYRILLIC 
                                        // CAPITAL LETTER LJE". So, add an extra check to catch
                                        // it.
                                        info.UnitId = "-";    // blank
                                    }
                                    else
                                    {
                                        info.UnitId = unitIDSerial;
                                    }
                                    // set return value
                                    p_WriteHandle = l_temp_handle;
                                    // get the device capabilities
                                    HidD_GetPreparsedData(l_temp_handle, ref PreparsedDataPointer);
                                    HidP_GetCaps(PreparsedDataPointer, ref Capabilities);
                                    // now create read handle
                                    p_ReadHandle = CreateFile(
                                                    SingledevicePathName,
                                                    GENERIC_READ | GENERIC_WRITE,
                                                    FILE_SHARE_READ | FILE_SHARE_WRITE,
                                                    ref Security,
                                                    OPEN_EXISTING,
                                                    //0,
                                                    FILE_FLAG_OVERLAPPED,
                                                    0);
                                    
                                    // now free up the resource, don't need anymore
                                    HidD_FreePreparsedData(ref PreparsedDataPointer);
                                    // get out of loop
                                    break;
                                }
                                CloseHandle(l_temp_handle); 
                                l_num_found_devices++;
                            }
                            else
                            {
                                l_found_device = false;
                                CloseHandle(l_temp_handle);
                            }
                        }
                        else
                        {
                            // There was a problem w/ HidD_GetAttributes
                            l_found_device = false;
                            CloseHandle(l_temp_handle);
                        } // if result == true
                    } // if HIDHandle
                }  // if result == true
            }  // end for
            //Free the memory reserved for the DeviceInfoSet returned by SetupDiGetClassDevs.
            SetupDiDestroyDeviceInfoList(DeviceInfoSet);
            info.Found = l_found_device;
            return info;
        }

        // Overlapped event and state for the write and read paths. Each path uses a
        // manual-reset event reused across transfers, created lazily on first use.
        private Kernel32.OVERLAPPED wrOverlapped;
        private Kernel32.OVERLAPPED rdOverlapped;
        private IntPtr wrEvent = IntPtr.Zero;
        private IntPtr rdEvent = IntPtr.Zero;

        public UsbTransferStatus Write(IntPtr writeHandle, byte[] buffer)
        {
            if (writeHandle == IntPtr.Zero)
                return UsbTransferStatus.Error;
            if (wrEvent == IntPtr.Zero)
            {
                wrEvent = CreateEvent(IntPtr.Zero, true, true, "");
                wrOverlapped.hEvent = wrEvent;
                wrOverlapped.Offset = 0;
                wrOverlapped.OffsetHigh = 0;
            }
            int bytesWritten = 0;
            WriteFile(writeHandle, buffer, buffer.Length, ref bytesWritten, ref wrOverlapped);
            uint result = WaitForSingleObject(wrEvent, 1000);
            UsbTransferStatus status;
            if (result == KONST.WAIT_OBJECT_0)
            {
                status = UsbTransferStatus.Success;
            }
            else if (result == KONST.WAIT_TIMEOUT)
            {
                // Cancel the pending overlapped write before reporting the timeout so the
                // caller's disconnect policy starts from a quiesced handle.
                CancelIo(writeHandle);
                status = UsbTransferStatus.Timeout;
            }
            else
            {
                status = UsbTransferStatus.Error;
            }
            ResetEvent(wrEvent);
            return status;
        }

        public UsbTransferStatus Read(IntPtr readHandle, byte[] buffer)
        {
            if (readHandle == IntPtr.Zero)
                return UsbTransferStatus.Error;
            if (rdEvent == IntPtr.Zero)
            {
                rdEvent = CreateEvent(IntPtr.Zero, true, true, "");
                rdOverlapped.hEvent = rdEvent;
                rdOverlapped.Offset = 0;
                rdOverlapped.OffsetHigh = 0;
            }
            int bytesRead = 0;
            ReadFile(readHandle, buffer, buffer.Length, ref bytesRead, ref rdOverlapped);
            uint result = WaitForSingleObject(rdEvent, 1000);
            UsbTransferStatus status;
            if (result == KONST.WAIT_OBJECT_0)
            {
                status = UsbTransferStatus.Success;
            }
            else if (result == KONST.WAIT_TIMEOUT)
            {
                CancelIo(readHandle);
                status = UsbTransferStatus.Timeout;
            }
            else
            {
                status = UsbTransferStatus.Error;
            }
            ResetEvent(rdEvent);
            return status;
        }

        public void Close(IntPtr handle)
        {
            CloseHandle(handle);
        }
    }
}
