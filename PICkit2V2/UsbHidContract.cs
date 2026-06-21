using System;

namespace PICkit2V2
{
    // Outcome of a single HID report transfer. The caller maps Timeout to its own
    // disconnect policy; the transport performs any mechanism-level cancellation
    // internally before returning.
    public enum UsbTransferStatus
    {
        Success,
        Timeout,
        Error
    }

    // Identity of a located PICkit unit, returned by FindDevice so the transport
    // does not reach into consumer state. PkobModel is meaningful only when IsPkob.
    public struct Pk2DeviceInfo
    {
        public bool Found;
        public string UnitId;
        public bool IsPk3;
        public bool IsPkob;
        public bool IsPk2m;
        public string ToolName;
        public string PkobModel;
    }

    // Platform-independent HID transport for PICkit2/PICkit3/PKOB programmers.
    // Handles are opaque to the consumer: a Win32 file handle on Windows, a hidapi
    // device pointer on other platforms. Buffers follow the report layout where
    // index 0 holds report id 0 and report data occupies indices 1..PACKET_SIZE-1.
    public interface IUsbHidTransport
    {
        Pk2DeviceInfo FindDevice(ushort index, out IntPtr readHandle, out IntPtr writeHandle);
        UsbTransferStatus Write(IntPtr writeHandle, byte[] buffer);
        UsbTransferStatus Read(IntPtr readHandle, byte[] buffer);
        void Close(IntPtr handle);
    }
}
