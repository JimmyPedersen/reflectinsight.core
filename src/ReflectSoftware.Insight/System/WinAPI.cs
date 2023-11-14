
using System;
using System.Runtime.InteropServices;

namespace RI.System.WinAPI
{
    [ComVisible(false)]
    public static class Kernel
    {
        [Flags]
        public enum FormatFlags : uint
        {
            FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100u,
            FORMAT_MESSAGE_IGNORE_INSERTS = 0x200u,
            FORMAT_MESSAGE_FROM_SYSTEM = 0x1000u
        }

        [Flags]
        public enum FileMapProtection : uint
        {
            PageReadonly = 0x2u,
            PageReadWrite = 0x4u,
            PageWriteCopy = 0x8u,
            PageExecuteRead = 0x20u,
            PageExecuteReadWrite = 0x40u,
            SectionCommit = 0x8000000u,
            SectionImage = 0x1000000u,
            SectionNoCache = 0x10000000u,
            SectionReserve = 0x4000000u
        }

        [Flags]
        public enum FileMapAccess : uint
        {
            FileMapCopy = 0x1u,
            FileMapWrite = 0x2u,
            FileMapRead = 0x4u,
            FileMapAllAccess = 0x1Fu,
            fileMapExecute = 0x20u
        }

        [Serializable]
        public struct CRITICAL_SECTION
        {
            public IntPtr DebugInfo;

            public long LockCount;

            public long RecursionCount;

            public IntPtr OwningThread;

            public IntPtr LockSemaphore;

            public IntPtr SpinCount;
        }

        [Serializable]
        internal struct MEMORYSTATUS32
        {
            public uint dwLength;

            public uint dwMemoryLoad;

            public uint dwTotalPhys;

            public uint dwAvailPhys;

            public uint dwTotalPageFile;

            public uint dwAvailPageFile;

            public uint dwTotalVirtual;

            public uint dwAvailVirtual;
        }

        [Serializable]
        public struct MEMORYSTATUS
        {
            public uint dwLength;

            public uint dwMemoryLoad;

            public ulong dwTotalPhys;

            public ulong dwAvailPhys;

            public ulong dwTotalPageFile;

            public ulong dwAvailPageFile;

            public ulong dwTotalVirtual;

            public ulong dwAvailVirtual;
        }

        [Serializable]
        public struct SECURITY_ATTRIBUTES
        {
            public uint nLength;

            public IntPtr lpSecurityDescriptor;

            public bool bInheritHandle;
        }

        public const uint STANDARD_RIGHTS_REQUIRED = 983040u;

        public const uint SYNCHRONIZE = 1048576u;

        public const uint EVENT_ALL_ACCESS = 2031619u;

        public const uint EVENT_MODIFY_STATE = 2u;

        public const uint ERROR_FILE_NOT_FOUND = 2u;

        public const uint DELETE = 65536u;

        public const uint READ_CONTROL = 131072u;

        public const uint WRITE_DAC = 262144u;

        public const uint WRITE_OWNER = 524288u;

        public const uint MUTEX_ALL_ACCESS = 2031617u;

        public const uint MUTEX_MODIFY_STATE = 1u;

        public const uint SEMAPHORE_ALL_ACCESS = 2031619u;

        public const uint SEMAPHORE_MODIFY_STATE = 2u;

        public const uint TIMER_ALL_ACCESS = 2031619u;

        public const uint TIMER_MODIFY_STATE = 2u;

        public const uint TIMER_QUERY_STATE = 1u;

        public const uint WAIT_ABANDONED = 128u;

        public const uint WAIT_OBJECT_0 = 0u;

        public const uint WAIT_TIMEOUT = 258u;

        public const uint WAIT_FAILED = uint.MaxValue;

        public const uint INFINITE = uint.MaxValue;

        public const uint SECTION_QUERY = 1u;

        public const uint SECTION_MAP_WRITE = 2u;

        public const uint SECTION_MAP_READ = 4u;

        public const uint SECTION_MAP_EXECUTE = 8u;

        public const uint SECTION_EXTEND_SIZE = 16u;

        public const uint SECTION_ALL_ACCESS = 983071u;

        public const uint FILE_MAP_ALL_ACCESS = 983071u;

        public const uint CREATE_NEW = 1u;

        public const uint CREATE_ALWAYS = 2u;

        public const uint OPEN_EXISTING = 3u;

        public const uint OPEN_ALWAYS = 4u;

        public const uint TRUNCATE_EXISTING = 5u;

        public const uint FILE_FLAG_WRITE_THROUGH = 2147483648u;

        public const uint FILE_FLAG_OVERLAPPED = 1073741824u;

        public const uint FILE_FLAG_NO_BUFFERING = 536870912u;

        public const uint FILE_FLAG_RANDOM_ACCESS = 268435456u;

        public const uint FILE_FLAG_SEQUENTIAL_SCAN = 134217728u;

        public const uint FILE_FLAG_DELETE_ON_CLOSE = 67108864u;

        public const uint FILE_FLAG_BACKUP_SEMANTICS = 33554432u;

        public const uint FILE_FLAG_POSIX_SEMANTICS = 16777216u;

        public const uint FILE_FLAG_OPEN_REPARSE_POINT = 2097152u;

        public const uint FILE_FLAG_OPEN_NO_RECALL = 1048576u;

        public const uint FILE_FLAG_FIRST_PIPE_INSTANCE = 524288u;

        public const uint FILE_SHARE_READ = 1u;

        public const uint FILE_SHARE_WRITE = 2u;

        public const uint FILE_SHARE_DELETE = 4u;

        public const uint FILE_ATTRIBUTE_READONLY = 1u;

        public const uint FILE_ATTRIBUTE_HIDDEN = 2u;

        public const uint FILE_ATTRIBUTE_SYSTEM = 4u;

        public const uint FILE_ATTRIBUTE_DIRECTORY = 16u;

        public const uint FILE_ATTRIBUTE_ARCHIVE = 32u;

        public const uint FILE_ATTRIBUTE_DEVICE = 64u;

        public const uint FILE_ATTRIBUTE_NORMAL = 128u;

        public const uint FILE_ATTRIBUTE_TEMPORARY = 256u;

        public const uint FILE_ATTRIBUTE_SPARSE_FILE = 512u;

        public const uint FILE_ATTRIBUTE_REPARSE_POINT = 1024u;

        public const uint FILE_ATTRIBUTE_COMPRESSED = 2048u;

        public const uint FILE_ATTRIBUTE_OFFLINE = 4096u;

        public const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 8192u;

        public const uint FILE_ATTRIBUTE_ENCRYPTED = 16384u;

        public const uint GENERIC_READ = 2147483648u;

        public const uint GENERIC_WRITE = 1073741824u;

        public const uint GENERIC_EXECUTE = 536870912u;

        public const uint GENERIC_ALL = 268435456u;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentProcessId();

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern uint WaitForMultipleObjects(uint nCount, IntPtr[] lpHandles, bool bWaitAll, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WaitForSingleObject(IntPtr Handle, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void InitializeCriticalSection(out CRITICAL_SECTION lpCriticalSection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void DeleteCriticalSection(ref CRITICAL_SECTION lpCriticalSection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void EnterCriticalSection(ref CRITICAL_SECTION lpCriticalSection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void LeaveCriticalSection(ref CRITICAL_SECTION lpCriticalSection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TryEnterCriticalSection(ref CRITICAL_SECTION lpCriticalSection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern FileHandle LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int FreeLibrary(FileHandle hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport("kernel32.dll", EntryPoint = "GlobalMemoryStatus", SetLastError = true)]
        private static extern void GlobalMemoryStatus32(ref MEMORYSTATUS32 lpBuffer);

        [DllImport("kernel32.dll", EntryPoint = "GlobalMemoryStatus", SetLastError = true)]
        private static extern void GlobalMemoryStatus64(ref MEMORYSTATUS lpBuffer);

        [DllImport("kernel32.dll")]
        private static extern uint FormatMessage(FormatFlags dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer, uint nSize, IntPtr Arguments);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern FileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, FileHandle hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern FileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, ref SECURITY_ATTRIBUTES lpFileMappingAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, FileHandle hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern FileHandle OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle, string lpName);

        [DllImport("kernel32.dll", EntryPoint = "FlushViewOfFile", SetLastError = true)]
        private static extern bool FlushViewOfFile32(IntPtr lpBaseAddress, uint dwNumberOfBytesToFlush);

        [DllImport("kernel32.dll", EntryPoint = "FlushViewOfFile", SetLastError = true)]
        private static extern bool FlushViewOfFile64(IntPtr lpBaseAddress, ulong dwNumberOfBytesToFlush);

        [DllImport("kernel32.dll", EntryPoint = "MapViewOfFile", SetLastError = true)]
        private static extern IntPtr MapViewOfFile32(FileHandle hFileMappingObject, FileMapAccess dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", EntryPoint = "MapViewOfFile", SetLastError = true)]
        private static extern IntPtr MapViewOfFile64(FileHandle hFileMappingObject, FileMapAccess dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, ulong dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern FileHandle CreateFileMapping(FileHandle hFile, ref SECURITY_ATTRIBUTES lpFileMappingAttributes, FileMapProtection flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        public static int GetFrameworkRuntimeCPUBitSize()
        {
            return IntPtr.Size * 8;
        }

        public static string GetFormatMessage(int lastError)
        {
            IntPtr lpBuffer = IntPtr.Zero;
            try
            {
                FormatFlags dwFlags = FormatFlags.FORMAT_MESSAGE_ALLOCATE_BUFFER | FormatFlags.FORMAT_MESSAGE_IGNORE_INSERTS | FormatFlags.FORMAT_MESSAGE_FROM_SYSTEM;
                if (FormatMessage(dwFlags, IntPtr.Zero, (uint)lastError, 0u, ref lpBuffer, 0u, IntPtr.Zero) == 0)
                {
                    return string.Empty;
                }

                return Marshal.PtrToStringAnsi(lpBuffer);
            }
            finally
            {
                if (lpBuffer != IntPtr.Zero)
                {
                    LocalFree(lpBuffer);
                }
            }
        }

        public static void GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer)
        {
            lpBuffer.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUS));
            if (GetFrameworkRuntimeCPUBitSize() == 64)
            {
                GlobalMemoryStatus64(ref lpBuffer);
                return;
            }

            MEMORYSTATUS32 lpBuffer2 = default(MEMORYSTATUS32);
            lpBuffer2.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUS));
            GlobalMemoryStatus32(ref lpBuffer2);
            lpBuffer.dwAvailPageFile = lpBuffer2.dwAvailPageFile;
            lpBuffer.dwAvailPhys = lpBuffer2.dwAvailPhys;
            lpBuffer.dwAvailVirtual = lpBuffer2.dwAvailVirtual;
            lpBuffer.dwMemoryLoad = lpBuffer2.dwMemoryLoad;
            lpBuffer.dwTotalPageFile = lpBuffer2.dwTotalPageFile;
            lpBuffer.dwTotalPhys = lpBuffer2.dwTotalPhys;
            lpBuffer.dwTotalVirtual = lpBuffer2.dwTotalVirtual;
        }

        public static IntPtr MapViewOfFile(FileHandle hFileMappingObject, FileMapAccess dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, ulong dwNumberOfBytesToMap)
        {
            if (GetFrameworkRuntimeCPUBitSize() == 64)
            {
                return MapViewOfFile64(hFileMappingObject, dwDesiredAccess, dwFileOffsetHigh, dwFileOffsetLow, dwNumberOfBytesToMap);
            }

            return MapViewOfFile32(hFileMappingObject, dwDesiredAccess, dwFileOffsetHigh, dwFileOffsetLow, (uint)dwNumberOfBytesToMap);
        }

        public static bool FlushViewOfFile(IntPtr lpBaseAddress, ulong dwNumberOfBytesToFlush)
        {
            if (GetFrameworkRuntimeCPUBitSize() == 64)
            {
                return FlushViewOfFile64(lpBaseAddress, dwNumberOfBytesToFlush);
            }

            return FlushViewOfFile32(lpBaseAddress, (uint)dwNumberOfBytesToFlush);
        }
    }
}