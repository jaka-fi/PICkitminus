using System;
using System.Runtime.InteropServices;
using KONST = PICkit2V2.Constants;

namespace PICkit2V2
{
    // HID transport for non-Windows platforms, implemented over hidapi. Handles
    // returned to the consumer are hidapi device pointers. A located unit is opened
    // twice so the read and write handles are independent, mirroring the Windows
    // two-handle model where DisconnectPICkit2Unit closes each handle once.
    public class UsbHidLinux : IUsbHidTransport
    {
        // Number of wchar_t held by the string buffers passed to hidapi, matching the
        // 64-unit limit the Windows path reads for serial and product strings.
        private const int WideStringUnits = 64;

        // Reusable report payload buffer, sized to the consumer buffer minus the
        // leading report-id slot. Held across reads so the programming path does not
        // allocate on every transfer.
        private byte[] readReport;

        public UsbHidLinux()
        {
            HidApiNative.hid_init();
        }

        public Pk2DeviceInfo FindDevice(ushort index, out IntPtr readHandle, out IntPtr writeHandle)
        {
            Pk2DeviceInfo info = new Pk2DeviceInfo();
            info.UnitId = "";
            info.ToolName = "PICkit 2";
            info.PkobModel = "PKOB";
            readHandle = IntPtr.Zero;
            writeHandle = IntPtr.Zero;

            IntPtr enumeration = HidApiNative.hid_enumerate(KONST.MChipVendorID, 0);
            IntPtr node = enumeration;
            ushort foundCount = 0;
            IntPtr matchedPath = IntPtr.Zero;
            ushort matchedProduct = 0;
            while ((node != IntPtr.Zero) && (matchedPath == IntPtr.Zero))
            {
                HidApiNative.hid_device_info devInfo =
                    (HidApiNative.hid_device_info)Marshal.PtrToStructure(node, typeof(HidApiNative.hid_device_info));
                node = devInfo.next;
                if (!IsSupportedProduct(devInfo.product_id))
                    continue;
                if (foundCount == index)
                {
                    matchedPath = devInfo.path;
                    matchedProduct = devInfo.product_id;
                }
                else
                {
                    foundCount++;
                }
            }

            if (matchedPath != IntPtr.Zero)
            {
                IntPtr writeDevice = HidApiNative.hid_open_path(matchedPath);
                IntPtr readDevice = HidApiNative.hid_open_path(matchedPath);
                if ((writeDevice != IntPtr.Zero) && (readDevice != IntPtr.Zero))
                {
                    PopulateIdentity(writeDevice, matchedProduct, ref info);
                    writeHandle = writeDevice;
                    readHandle = readDevice;
                    info.Found = true;
                }
                else
                {
                    // One or both opens failed; release whichever succeeded so the
                    // device file descriptors do not leak.
                    if (writeDevice != IntPtr.Zero)
                        HidApiNative.hid_close(writeDevice);
                    if (readDevice != IntPtr.Zero)
                        HidApiNative.hid_close(readDevice);
                    // The device was enumerated but could not be opened, which on Linux
                    // is almost always a hidraw permission problem. Report it so the
                    // cause is visible instead of appearing as a missing device.
                    string reason = HidApiNative.PtrToWideString(HidApiNative.hid_error(IntPtr.Zero));
                    Console.Error.WriteLine(
                        "PICkit device (VID 0x{0:X4} PID 0x{1:X4}) was found but could not be opened: {2}. "
                        + "Check hidraw node permissions; see 60-pickit.rules.",
                        KONST.MChipVendorID, matchedProduct, reason);
                    info.Found = false;
                }
            }
            else
            {
                info.Found = false;
            }

            if (enumeration != IntPtr.Zero)
                HidApiNative.hid_free_enumeration(enumeration);
            return info;
        }

        public UsbTransferStatus Write(IntPtr writeHandle, byte[] buffer)
        {
            if (writeHandle == IntPtr.Zero)
                return UsbTransferStatus.Error;
            int written = HidApiNative.hid_write(writeHandle, buffer, new UIntPtr((uint)buffer.Length));
            UsbTransferStatus status;
            if (written < 0)
                status = UsbTransferStatus.Error;
            else
                status = UsbTransferStatus.Success;
            return status;
        }

        public UsbTransferStatus Read(IntPtr readHandle, byte[] buffer)
        {
            if (readHandle == IntPtr.Zero)
                return UsbTransferStatus.Error;
            if ((readReport == null) || (readReport.Length != buffer.Length - 1))
                readReport = new byte[buffer.Length - 1];
            int read = HidApiNative.hid_read_timeout(readHandle, readReport, new UIntPtr((uint)readReport.Length), 1000);
            UsbTransferStatus status;
            if (read < 0)
            {
                status = UsbTransferStatus.Error;
            }
            else if (read == 0)
            {
                status = UsbTransferStatus.Timeout;
            }
            else
            {
                // hidapi returns unnumbered report data without the leading report id;
                // shift the payload to index 1 so consumers see the same layout as the
                // Windows read, where byte 0 holds report id 0.
                buffer[0] = 0;
                Array.Copy(readReport, 0, buffer, 1, read);
                status = UsbTransferStatus.Success;
            }
            return status;
        }

        public void Close(IntPtr handle)
        {
            HidApiNative.hid_close(handle);
        }

        private static bool IsSupportedProduct(ushort productId)
        {
            return (productId == KONST.Pk2DeviceID)
                || (productId == KONST.Pk3DeviceID)
                || (productId == KONST.PkobDeviceID);
        }

        private static void PopulateIdentity(IntPtr device, ushort productId, ref Pk2DeviceInfo info)
        {
            if (productId == KONST.Pk2DeviceID)
            {
                info.IsPk3 = false;
                info.IsPkob = false;
                string productName = ReadProductString(device);
                if (IsPk2m(productName))
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
            else if (productId == KONST.Pk3DeviceID)
            {
                info.IsPk3 = true;
                info.IsPkob = false;
                info.ToolName = "PICkit 3";
            }
            else
            {
                // The enumeration is filtered to the three supported products, so the
                // remaining case is PKOB.
                info.IsPk3 = true;
                info.IsPkob = true;
                info.ToolName = "PKOB";
                info.PkobModel = ReadProductString(device);
            }
            info.UnitId = ResolveUnitId(device);
        }

        private static bool IsPk2m(string productName)
        {
            return (productName.Length >= 4)
                && (productName[0] == 'P')
                && (productName[1] == 'K')
                && (productName[2] == '2')
                && (productName[3] == 'M');
        }

        private static string ResolveUnitId(IntPtr device)
        {
            string serial = ReadSerialString(device);
            string unitId;
            if ((serial.Length == 0)
                || (serial[0] == '\t')
                || (serial[0] == (char)0)
                || (serial[0] == (char)0x409))
            {
                // Blank units report a serial that decodes to an empty or sentinel
                // value; present it as a blank unit id, matching the Windows path.
                unitId = "-";
            }
            else
            {
                unitId = serial;
            }
            return unitId;
        }

        private static string ReadProductString(IntPtr device)
        {
            byte[] buffer = new byte[WideStringUnits * 4];
            HidApiNative.hid_get_product_string(device, buffer, new UIntPtr((uint)WideStringUnits));
            return HidApiNative.DecodeWideString(buffer, WideStringUnits);
        }

        private static string ReadSerialString(IntPtr device)
        {
            byte[] buffer = new byte[WideStringUnits * 4];
            HidApiNative.hid_get_serial_number_string(device, buffer, new UIntPtr((uint)WideStringUnits));
            return HidApiNative.DecodeWideString(buffer, WideStringUnits);
        }
    }
}
