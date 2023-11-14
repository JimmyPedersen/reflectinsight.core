
using RI.System.WinAPI;
using RI.Utils.Miscellaneous;
using RI.Utils.Strings;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace RI.Threading.Guard
{
    [ComVisible(false)]
    public class SharedMemory : IDisposable
    {
        private static readonly object LockObject;

        private static readonly FileSecurity FFileSecurity;

        private static IntPtr FFileSecurityDescriptor;

        private static Kernel.SECURITY_ATTRIBUTES FlpAttributes;

        private string FName;

        private string FFileName;

        private uint FSize;

        private FileHandle FFileMapHandle;

        private FileHandle FTmpFileHandle;

        private IntPtr FFileView;

        private bool FCreator;

        public bool Disposed { get; private set; }

        public string Name => FName;

        public uint Size => FSize;

        public IntPtr MemoryPointer => FFileView;

        public bool Creator => FCreator;

        static SharedMemory()
        {
            LockObject = new object();
            FFileSecurity = new FileSecurity();
            FFileSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, AccessControlType.Allow));
            byte[] securityDescriptorBinaryForm = FFileSecurity.GetSecurityDescriptorBinaryForm();
            FFileSecurityDescriptor = Marshal.AllocHGlobal(securityDescriptorBinaryForm.Length);
            Marshal.Copy(securityDescriptorBinaryForm, 0, FFileSecurityDescriptor, securityDescriptorBinaryForm.Length);
            FlpAttributes = default(Kernel.SECURITY_ATTRIBUTES);
            FlpAttributes.nLength = (uint)Marshal.SizeOf(typeof(Kernel.SECURITY_ATTRIBUTES));
            FlpAttributes.bInheritHandle = false;
            FlpAttributes.lpSecurityDescriptor = FFileSecurityDescriptor;
        }

        public static void OnStartup()
        {
        }

        public static void OnShutdown()
        {
            lock (LockObject)
            {
                if (FFileSecurityDescriptor != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(FFileSecurityDescriptor);
                    FFileSecurityDescriptor = IntPtr.Zero;
                }
            }
        }

        private void CreateFileMapping(string name, uint size, FileHandle hFileHandle)
        {
            try
            {
                FFileName = null;
                FName = name;
                FSize = size;
                FFileView = IntPtr.Zero;
                FFileMapHandle = Kernel.CreateFileMapping(hFileHandle, ref FlpAttributes, Kernel.FileMapProtection.PageReadWrite, 0u, size, name);
                int lastWin32Error = Marshal.GetLastWin32Error();
                if (lastWin32Error != 0)
                {
                    string errorName = WinError.GetErrorName(lastWin32Error);
                    if (!StringHelper.IsNullOrEmpty(errorName))
                    {
                        throw new Win32Exception(lastWin32Error);
                    }
                }

                FCreator = lastWin32Error == 0;
                FFileView = Kernel.MapViewOfFile(FFileMapHandle, Kernel.FileMapAccess.FileMapWrite | Kernel.FileMapAccess.FileMapRead, 0u, 0u, size);
                if (FFileView == IntPtr.Zero)
                {
                    throw new Exception("SharedMemory.CreateFileMapping: Unable to obtain MapView");
                }
            }
            catch (Exception)
            {
                FreeResources();
                throw;
            }
        }

        public SharedMemory(string name, uint size, FileHandle hFileHandle)
        {
            Disposed = false;
            FTmpFileHandle = null;
            CreateFileMapping(name, size, hFileHandle);
        }

        public SharedMemory(string name, uint size)
        {
            Disposed = false;
            using (FTmpFileHandle = new FileHandle(-1))
            {
                CreateFileMapping(name, size, FTmpFileHandle);
            }
        }

        public SharedMemory(string fileName, string name, uint size)
        {
            Disposed = false;
            using FileHandle hTemplateFile = new FileHandle(IntPtr.Zero);
            using (new ShortTermResourceLock(fileName, ResourceLockType.Global))
            {
                try
                {
                    DeleteFile(fileName);
                    bool flag = File.Exists(fileName);
                    uint dwCreationDisposition = 4u;
                    uint dwShareMode = 3u;
                    uint dwFlagsAndAttributes = 2147483904u;
                    FTmpFileHandle = Kernel.CreateFile(fileName, 3221225472u, dwShareMode, ref FlpAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    if (FTmpFileHandle.IsInvalid)
                    {
                        string formatMessage = Kernel.GetFormatMessage(lastWin32Error);
                        throw new SharedMemoryException($"{formatMessage}: SharedMemory for file: {fileName}");
                    }

                    CreateFileMapping(null, size, FTmpFileHandle);
                    FCreator = !flag;
                    FName = name;
                    FFileName = fileName;
                }
                catch (Exception)
                {
                    FreeResources();
                    throw;
                }
            }
        }

        ~SharedMemory()
        {
            Dispose(bDisposing: false);
        }

        protected static void DeleteFile(string fileName)
        {
            if (StringHelper.IsNullOrEmpty(fileName))
            {
                return;
            }

            using (new ShortTermResourceLock(fileName, ResourceLockType.Global))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception)
                {
                }
            }
        }

        protected virtual void FreeResources()
        {
            if (FFileView != IntPtr.Zero)
            {
                Kernel.UnmapViewOfFile(FFileView);
            }

            if (FFileMapHandle != null)
            {
                FFileMapHandle.Close();
            }

            if (FTmpFileHandle != null)
            {
                FTmpFileHandle.Close();
            }

            DeleteFile(FFileName);
            FFileName = null;
            FFileMapHandle = null;
            FTmpFileHandle = null;
            FFileView = IntPtr.Zero;
        }

        protected void Dispose(bool bDisposing)
        {
            lock (this)
            {
                if (!Disposed)
                {
                    Disposed = true;
                    GC.SuppressFinalize(this);
                    FreeResources();
                }
            }
        }

        public void Dispose()
        {
            Dispose(bDisposing: true);
        }

        public virtual bool Flush()
        {
            return Kernel.FlushViewOfFile(FFileView, 0uL);
        }
    }





    [ComVisible(false)]
    public class SharedMemory<T> : SharedMemory
    {
        public virtual T Memory
        {
            get
            {
                return (T)Marshal.PtrToStructure(base.MemoryPointer, typeof(T));
            }
            set
            {
                Marshal.StructureToPtr((object)value, base.MemoryPointer, fDeleteOld: false);
                Flush();
            }
        }

        public SharedMemory(string name, FileHandle hFileHandle)
            : base(name, (uint)Marshal.SizeOf(typeof(T)), hFileHandle)
        {
        }

        public SharedMemory(string name)
            : base(name, (uint)Marshal.SizeOf(typeof(T)))
        {
        }

        public SharedMemory(string fileName, string name)
            : base(fileName, name, (uint)Marshal.SizeOf(typeof(T)))
        {
        }
    }


    [Serializable]
    [ComVisible(false)]
    public class SharedMemoryException : ApplicationException
    {
        public SharedMemoryException(string msg)
            : base(msg)
        {
        }

        public SharedMemoryException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }
    }


    [ComVisible(false)]
    public class SharedMemoryGuard : SharedMemory
    {
        private SingleWriteMultipleReadGuard FGuard;

        public SingleWriteMultipleReadGuard Guard => FGuard;

        public SharedMemoryGuard(string name, uint size, FileHandle hFileHandle)
            : base(name, size, hFileHandle)
        {
            FGuard = new SingleWriteMultipleReadGuard(name);
        }

        public SharedMemoryGuard(string name, uint size)
            : base(name, size)
        {
            FGuard = new SingleWriteMultipleReadGuard(name);
        }

        public SharedMemoryGuard(string tmpFilePrefix, string name, uint size)
            : base(tmpFilePrefix, name, size)
        {
            FGuard = new SingleWriteMultipleReadGuard(name);
        }

        protected override void FreeResources()
        {
            base.FreeResources();
            if (FGuard != null)
            {
                FGuard.Dispose();
                FGuard = null;
            }
        }
    }


    [ComVisible(false)]
    public class SharedMemoryGuard<T> : SharedMemory<T>
    {
        private SingleWriteMultipleReadGuard FGuard;

        public override T Memory
        {
            get
            {
                return GetMemory(-1);
            }
            set
            {
                SetMemory(value, -1);
            }
        }

        public SingleWriteMultipleReadGuard Guard => FGuard;

        public SharedMemoryGuard(string name, FileHandle hFileHandle)
            : base(name, hFileHandle)
        {
            FGuard = new SingleWriteMultipleReadGuard(name);
        }

        public SharedMemoryGuard(string name)
            : base(name)
        {
            FGuard = new SingleWriteMultipleReadGuard(name);
        }

        public SharedMemoryGuard(string tmpFilePrefix, string name)
            : base(tmpFilePrefix, name)
        {
            FGuard = new SingleWriteMultipleReadGuard(name);
        }

        protected override void FreeResources()
        {
            base.FreeResources();
            if (FGuard != null)
            {
                FGuard.Dispose();
                FGuard = null;
            }
        }

        public T GetMemory(int timeout)
        {
            T result = default(T);
            if (FGuard.Reading(timeout))
            {
                try
                {
                    return base.Memory;
                }
                finally
                {
                    FGuard.DoneReading();
                }
            }

            return result;
        }

        public bool SetMemory(T value, int timeout)
        {
            bool result = false;
            if (FGuard.Writing(timeout))
            {
                try
                {
                    base.Memory = value;
                    return true;
                }
                finally
                {
                    FGuard.DoneWriting();
                }
            }

            return result;
        }
    }
}