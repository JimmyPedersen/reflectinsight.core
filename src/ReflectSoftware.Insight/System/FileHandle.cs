using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace RI.System.WinAPI
{
    [ComVisible(false)]
    public class FileHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        public FileHandle()
            : base(ownsHandle: true)
        {
        }

        public FileHandle(IntPtr hHandle)
            : base(ownsHandle: true)
        {
            handle = hHandle;
        }

        public FileHandle(int hHandle)
            : this(new IntPtr(hHandle))
        {
        }

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }
}