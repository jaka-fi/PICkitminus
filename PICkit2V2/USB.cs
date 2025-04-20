using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using CONST = PICkit2V2.Constants;
using KONST = PICkit2V2.Constants;

namespace PICkit2V2
{
    public class USB
    {
        public static string UnitID = "";
    
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
            ref Kernel32.OVERLAPPED Overlapped                      // overlapped buffer - not used
            //int Overlapped                      // overlapped buffer - not used
            );

        [DllImport("kernel32", SetLastError = true)]
        public static extern unsafe UInt32 ReadFile(
              IntPtr hFile,                       // handle to file
              byte[] Buffer,                      // data buffer
              int NumberOfBytesToRead,            // number of bytes to read
              ref int pNumberOfBytesRead,         // number of bytes read
              ref Kernel32.OVERLAPPED Overlapped                      // overlapped buffer - not used
              //int Overlapped                      // overlapped buffer - not used
              );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 CancelIo(IntPtr hFile);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 ResetEvent(IntPtr hHandle);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        //        public static bool Find_This_Device(ushort p_VendorID,
        //                                           ushort p_PoductID,
        //                                           ushort p_index,
        //                                           ref IntPtr p_ReadHandle,
        //                                           ref IntPtr p_WriteHandle)
        public static bool Find_This_Device(ushort p_index,
                                           ref IntPtr p_ReadHandle,
                                           ref IntPtr p_WriteHandle)
        {
            // Zero based p_index is used to identify which PICkit 2 we wish to talk to
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
//                            if (DeviceAttributes.VendorID == p_VendorID &&
//                                DeviceAttributes.ProductID == p_PoductID)
//                            if (DeviceAttributes.VendorID == KONST.MChipVendorID &&
//                                (DeviceAttributes.ProductID == KONST.Pk2DeviceID || DeviceAttributes.ProductID == KONST.Pk3DeviceID))
                            if (DeviceAttributes.VendorID == KONST.MChipVendorID &&
                                (DeviceAttributes.ProductID == KONST.Pk2DeviceID || 
                                DeviceAttributes.ProductID == KONST.Pk3DeviceID ||
                                DeviceAttributes.ProductID == KONST.PkobDeviceID))
                            {
                                if (DeviceAttributes.ProductID == KONST.Pk2DeviceID)
                                {
                                    PICkitFunctions.isPK3 = false;
                                    PICkitFunctions.isPKOB = false;
                                
                                    // Detect if PICkit2 is PK2M
                                    IntPtr ptrBuffer = Marshal.AllocHGlobal(126);

                                    // get Product name string
                                    HidD_GetProductString(l_temp_handle, ptrBuffer, 126);
                                    productName = Marshal.PtrToStringUni(ptrBuffer, 64);
                                    Marshal.FreeHGlobal(ptrBuffer);

                                    if (((byte)productName[0] == 'P') && (productName[1] == 'K') && (productName[2] == '2') && (productName[3] == 'M'))
                                    {
                                        PICkitFunctions.isPK2M = true;
                                        PICkitFunctions.ToolName = "PK2M";
                                    }
                                    else
                                    {
                                        PICkitFunctions.isPK2M = false;
                                        PICkitFunctions.ToolName = "PICkit 2";
                                    }
                                }
                                else if (DeviceAttributes.ProductID == KONST.Pk3DeviceID)
                                {
                                    PICkitFunctions.isPK3 = true;
                                    PICkitFunctions.isPKOB = false;
                                    PICkitFunctions.ToolName = "PICkit 3";
                                }
                                else
                                {
                                    PICkitFunctions.isPK3 = true;
                                    PICkitFunctions.isPKOB = true;
                                    PICkitFunctions.ToolName = "PKOB";
                                    
                                    // Read PKOB model
                                    IntPtr ptrBuffer = Marshal.AllocHGlobal(126);

                                    // get Product name string
                                    HidD_GetProductString(l_temp_handle, ptrBuffer, 126);
                                    PICkitFunctions.pkobDeviceString = Marshal.PtrToStringUni(ptrBuffer, 64);
                                    //PICkitFunctions.pkobDeviceString = PICkitFunctions.pkobDeviceString.ToString();
                                    //PICkitFunctions.pkobDeviceString = "jaka";
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
                                        UnitID = "-";    // blank
                                    }
                                    else
                                    {
                                        UnitID = unitIDSerial;
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
            return l_found_device;
        }
        
        /////////
        /*
        private void read_overlapped()
            {
            // anonymous
            //   Thread this_thread = new Thread(delegate()
            //       {
            CONST.WAIT l_result = CONST.WAIT.WAIT_FAILED;
            byte[] l_temp_read_buffer = new byte[CONST.PACKET_SIZE];
            int l_num_bytes_read = 0;
            int l_x = 0;
            bool l_read = true;
            bool l_com_func = false;
            string l_temp = "";
            string l_time = "";
            string l_title = "";
            string l_good_data_str = "";
            string l_temp_str = "";

            CONST.OVERLAPPED l_overlapped;
            l_overlapped.hEvent = 0;
            l_overlapped.Internal = 0;
            l_overlapped.InternalHigh = 0;
            l_overlapped.Offset = 0;
            l_overlapped.OffsetHigh = 0;

            
                Array.Clear(l_temp_read_buffer, 0, l_temp_read_buffer.Length);

                l_read = COMM.ReadFile(COMM.g_comm_params.HID_read_handle,
                              l_temp_read_buffer,
                              COMM.g_comm_params.irbl,
                              ref l_num_bytes_read,
                              ref l_overlapped); // COMM.g_comm_params.overlapped);
                l_result = COMM.WaitForSingleObject(COMM.g_comm_params.overlapped.hEvent, 1000);                    
                //
                // testing portion
                //    for (l_x = 0; l_x < m_send_byte_array.Length; l_x++)
                //        {
                //        l_temp_read_buffer[l_x] = m_send_byte_array[l_x];
                //        }
                l_result = COMM.WAIT.WAIT_OBJECT_0;
                if (l_num_bytes_read == 0 ||
                    !COMM.g_comm_params.we_are_in_read_loop)
                    {
                    // error with non-overlapped read - get out and don't print data
                    break;
                    }
                switch (l_result)
                    {
                    case COMM.WAIT.WAIT_OBJECT_0:
                            {
                            //       COMM.ResetEvent(ref COMM.g_comm_params.overlapped.hEvent);  // don't know if need or not, maybe helps, maybe not
                            // received data
                            // process the data
                            // reset l_temp_read_buffer
                            for (l_x = 0; l_x < l_group_index_array.Length; l_x++)
                                {
                                l_group_index_array[l_x] = 100;  // greater than 32
                                }
                            // first, find location of all the groups within this packet
                            l_com_func = COMM.find_groupings_within_this_packet(ref l_temp_read_buffer, ref l_group_index_array, ref l_num_groups_found);
                            if (l_num_groups_found > l_group_index_array.Length)
                                {
                                MessageBox.Show("Found too many groups . . .");
                                break;
                                }
                            if (l_com_func)
                                {
                                // now loop through these groups and display on listbox
                                COMM.format_time(ref l_time);
                                for (l_x = 0; l_x < l_num_groups_found; l_x++)
                                    {
                                    // get the display string
                                    l_com_func = COMM.process_this_group(ref l_temp_read_buffer, l_group_index_array[l_x], ref l_good_data_str, ref l_title);
                                    if (l_com_func)
                                        {
                                        l_temp = l_time + l_title;
                                        m_display_receive_data_in_listbox_using_transaction_width_preference.Invoke(l_temp, ref l_good_data_str);
                                        m_update_listbox_receive.Invoke("");
                                        }
                                    else
                                        {
                                        COMM.format_time(ref l_temp);
                                        l_temp_str = "Could not interpret group within packet: \n" + l_good_data_str;
                                        COMM.update_logfile(ref l_temp_str, true);
                                        l_temp += l_temp_str;
                                        m_update_listbox_receive.Invoke(l_temp);
                                        m_update_listbox_receive.Invoke("");
                                        }
                                    //                                     m_blink_data_received.Invoke();
                                    }
                                }
                            else
                                {
                                COMM.format_time(ref l_temp);
                                l_temp_str = "USB packet formatted incorrectly - cannot interpret.";
                                l_temp += l_temp_str;
                                string l_str_temp = "";
                                string l_byte_str = "";
                                // just show the data
                                for (int l_z = 0; l_z < l_num_bytes_read; l_z++)
                                    {
                                    if (l_num_bytes_read > COMM.Constants.PACKET_SIZE)
                                        {
                                        // somethings wrong - get out of here
                                        break;
                                        }
                                    l_byte_str = string.Format("{0:X2} ", l_temp_read_buffer[l_z]);
                                    l_str_temp += l_byte_str;
                                    }
                                m_display_receive_data_in_listbox_using_transaction_width_preference.Invoke(l_temp_str, ref l_str_temp);
                                m_update_listbox_receive.Invoke("");
                                m_blink_data_received.Invoke();
                                }
                            break;
                            }
                    case COMM.WAIT.WAIT_ABANDONED:
                            {
                            // error
                            COMM.format_time(ref l_temp);
                            l_temp_str = "Wait Abandoned Error while trying to read USB buffer.";
                            COMM.update_logfile(ref l_temp_str, true);
                            l_temp += l_temp_str;
                            m_update_listbox_receive.Invoke(l_temp);
                            m_update_listbox_receive.Invoke("");
                            break;
                            }
                    case COMM.WAIT.WAIT_IO_COMPLETION:
                            {
                            // not sure what this is
                            COMM.format_time(ref l_temp);
                            l_temp_str += "Error - Wait IO Completion while trying to read USB buffer.";
                            COMM.update_logfile(ref l_temp_str, true);
                            l_temp += l_temp_str;
                            m_update_listbox_receive.Invoke(l_temp);
                            m_update_listbox_receive.Invoke("");
                            break;
                            }
                    case COMM.WAIT.WAIT_TIMEOUT:
                            {
                            // no data arrived within alloted timeout - just repeat;
                            COMM.CancelIo(COMM.g_comm_params.HID_read_handle);  // needed this or would sometimes get no response due to sync issues
                            break;
                            }
                    case COMM.WAIT.WAIT_FAILED:
                            {
                            // error
                            COMM.format_time(ref l_temp);
                            l_temp += "Wait Fail Error while trying to read USB buffer.";
                            m_update_listbox_receive.Invoke(l_temp);
                            m_update_listbox_receive.Invoke("");
                            break;
                            }
                    }
                
            m_change_polling_state.Invoke(false);  // just reset button
            //     });
            // this_thread.IsBackground = true;  // set so will stop when main thread is terminated
            // this_thread.Name = "ReadThread";
            // this_thread.Start();
            }
         */
    }
}
