using System;

namespace PICkit2V2
{
    // Selects the HID transport for the current platform. Windows keeps its native
    // hid.dll/setupapi path; every other platform uses the hidapi implementation.
    public static class UsbHidFactory
    {
        public static IUsbHidTransport Create()
        {
            IUsbHidTransport transport;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                transport = new UsbHidWindows();
            else
                transport = new UsbHidLinux();
            return transport;
        }
    }
}
