using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PICkit2V2
{
    // Raw P/Invoke bindings for the hidapi 0.12.0 library, used only on non-Windows
    // platforms. The Mono dllmap in the application config resolves "hidapi" to the
    // platform shared object. size_t parameters are marshalled as UIntPtr so the
    // bindings are correct under both 32-bit and 64-bit runtimes.
    internal static class HidApiNative
    {
        private const string Library = "hidapi";

        // Mirrors struct hid_device_info from hidapi.h. Pointer members are held as
        // IntPtr so the field layout and the next-node offset are correct regardless
        // of process bitness.
        [StructLayout(LayoutKind.Sequential)]
        public struct hid_device_info
        {
            public IntPtr path;
            public ushort vendor_id;
            public ushort product_id;
            public IntPtr serial_number;
            public ushort release_number;
            public IntPtr manufacturer_string;
            public IntPtr product_string;
            public ushort usage_page;
            public ushort usage;
            public int interface_number;
            public IntPtr next;
        }

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_init();

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_exit();

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr hid_enumerate(ushort vendor_id, ushort product_id);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        public static extern void hid_free_enumeration(IntPtr devs);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr hid_open_path(IntPtr path);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_write(IntPtr device, byte[] data, UIntPtr length);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_read_timeout(IntPtr device, byte[] data, UIntPtr length, int milliseconds);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        public static extern void hid_close(IntPtr device);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_get_serial_number_string(IntPtr device, byte[] str, UIntPtr maxlen);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_get_product_string(IntPtr device, byte[] str, UIntPtr maxlen);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr hid_error(IntPtr device);

        // hidapi wide strings use the platform wchar_t, which is four bytes (UTF-32)
        // on the Linux and macOS targets. Decode the little-endian buffer up to the
        // first null code unit. maxUnits is the number of wchar_t the buffer holds.
        public static string DecodeWideString(byte[] buffer, int maxUnits)
        {
            return DecodeWideUnits(maxUnits, unit => BitConverter.ToInt32(buffer, unit * 4));
        }

        // Decode a null-terminated platform wchar_t string at an unmanaged pointer,
        // as returned by hid_error. The cap bounds the scan in case the library
        // returns a string that is not terminated within the expected length.
        public static string PtrToWideString(IntPtr wideString)
        {
            if (wideString == IntPtr.Zero)
                return "";
            const int maxUnits = 256;
            return DecodeWideUnits(maxUnits, unit => Marshal.ReadInt32(wideString, unit * 4));
        }

        // Build a string from successive UTF-32 code units up to the first null or
        // maxUnits, whichever comes first. readUnit isolates the source so a managed
        // buffer and an unmanaged pointer share one scan.
        private static string DecodeWideUnits(int maxUnits, Func<int, int> readUnit)
        {
            StringBuilder builder = new StringBuilder();
            bool terminated = false;
            for (int unit = 0; (unit < maxUnits) && !terminated; unit++)
            {
                int codePoint = readUnit(unit);
                if (codePoint == 0)
                    terminated = true;
                else
                    builder.Append(char.ConvertFromUtf32(codePoint));
            }
            return builder.ToString();
        }
    }
}
