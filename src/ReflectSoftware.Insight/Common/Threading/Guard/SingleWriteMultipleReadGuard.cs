
using RI.System.WinAPI;
using RI.Utils.Strings;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace RI.Threading.Guard
{
    [ComVisible(false)]
    public class SingleWriteMultipleReadGuard : IDisposable
    {
        private static readonly MutexSecurity FMutexSecurity;

        private static readonly SemaphoreSecurity FSemaphoreSecurity;

        private static readonly EventWaitHandleSecurity FEventWaitHandleSecurity;

        private static readonly Hashtable FCurrentActiveWritingThread;

        private readonly string FName;

        private readonly ulong FNameID;

        private Mutex FMutexNoWriter;

        private Semaphore FSemNumReaders;

        private EventWaitHandle FEventNoReaders;

        public bool Disposed { get; private set; }

        public string Name => FName;

        static SingleWriteMultipleReadGuard()
        {
            SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            FMutexSecurity = new MutexSecurity();
            FMutexSecurity.AddAccessRule(new MutexAccessRule(identity, MutexRights.FullControl, AccessControlType.Allow));
            FSemaphoreSecurity = new SemaphoreSecurity();
            FSemaphoreSecurity.AddAccessRule(new SemaphoreAccessRule(identity, SemaphoreRights.FullControl, AccessControlType.Allow));
            FEventWaitHandleSecurity = new EventWaitHandleSecurity();
            FEventWaitHandleSecurity.AddAccessRule(new EventWaitHandleAccessRule(identity, EventWaitHandleRights.FullControl, AccessControlType.Allow));
            FCurrentActiveWritingThread = new Hashtable();
        }

        public SingleWriteMultipleReadGuard(string name)
        {
            Disposed = false;
            FName = name;
            FMutexNoWriter = null;
            FSemNumReaders = null;
            FEventNoReaders = null;
            try
            {
                //               FMutexNoWriter = new Mutex(initiallyOwned: false, $"{name}::SWMRGMutexNoWriter", out var createdNew, FMutexSecurity);
                FMutexNoWriter = new Mutex(false, $"{name}::SWMRGMutexNoWriter", out var createdNew);//, FMutexSecurity);
                                                                                                     //                FSemNumReaders = new Semaphore(0, int.MaxValue, $"{name}::SWMRGSemNumReaders", out createdNew, FSemaphoreSecurity);
                FSemNumReaders = new Semaphore(0, int.MaxValue, $"{name}::SWMRGSemNumReaders", out createdNew);//, FSemaphoreSecurity);
                                                                                                               //                FEventNoReaders = new EventWaitHandle(initialState: true, EventResetMode.ManualReset, $"{name}::SWMRGEventNoReaders", out createdNew, FEventWaitHandleSecurity);
                FEventNoReaders = new EventWaitHandle(true, EventResetMode.ManualReset, $"{name}::SWMRGEventNoReaders", out createdNew);//, FEventWaitHandleSecurity);

                FNameID = (ulong)(StringHash.BKDRHash(FName) << 32);
            }
            catch (Exception)
            {
                FreeResources();
                throw;
            }
        }

        public SingleWriteMultipleReadGuard()
            : this(Guid.NewGuid().ToString())
        {
        }

        ~SingleWriteMultipleReadGuard()
        {
            Dispose(bDisposing: false);
        }

        protected virtual void FreeResources()
        {
            lock (this)
            {
                if (FMutexNoWriter != null)
                {
                    FMutexNoWriter.Dispose();
                }

                if (FSemNumReaders != null)
                {
                    FSemNumReaders.Dispose();
                }

                if (FEventNoReaders != null)
                {
                    FEventNoReaders.Dispose();
                }

                FMutexNoWriter = null;
                FSemNumReaders = null;
                FEventNoReaders = null;
            }
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

        public bool Reading(int timeout)
        {
            if (!FMutexNoWriter.WaitOne(timeout))
            {
                return false;
            }

            try
            {
                if (FSemNumReaders.Release(1) == 0)
                {
                    FEventNoReaders.Reset();
                }

                return true;
            }
            finally
            {
                FMutexNoWriter.ReleaseMutex();
            }
        }

        public bool Reading()
        {
            return Reading(-1);
        }

        public void DoneReading()
        {
            try
            {
                IntPtr[] array = new IntPtr[2]
                {
                    FMutexNoWriter.SafeWaitHandle.DangerousGetHandle(),
                    FSemNumReaders.SafeWaitHandle.DangerousGetHandle()
                };
                Kernel.WaitForMultipleObjects((uint)array.Length, array, bWaitAll: true, uint.MaxValue);
                if (!FSemNumReaders.WaitOne(0))
                {
                    FEventNoReaders.Set();
                }
                else
                {
                    FSemNumReaders.Release(1);
                }
            }
            finally
            {
                FMutexNoWriter.ReleaseMutex();
            }
        }

        public bool Writing(int timeout)
        {
            ulong num = FNameID + Kernel.GetCurrentThreadId();
            lock (FCurrentActiveWritingThread)
            {
                object obj = FCurrentActiveWritingThread[num];
                if (obj != null)
                {
                    int num2 = (int)obj;
                    if (num2 > 0)
                    {
                        FCurrentActiveWritingThread[num] = num2 + 1;
                        return true;
                    }
                }
            }

            IntPtr[] array = new IntPtr[2]
            {
                FMutexNoWriter.SafeWaitHandle.DangerousGetHandle(),
                FEventNoReaders.SafeWaitHandle.DangerousGetHandle()
            };
            bool flag = Kernel.WaitForMultipleObjects((uint)array.Length, array, bWaitAll: true, (uint)timeout) != 258;
            if (flag)
            {
                lock (FCurrentActiveWritingThread)
                {
                    FCurrentActiveWritingThread[num] = 1;
                    return flag;
                }
            }

            return flag;
        }

        public bool Writing()
        {
            return Writing(-1);
        }

        public void DoneWriting()
        {
            lock (FCurrentActiveWritingThread)
            {
                ulong num = FNameID + Kernel.GetCurrentThreadId();
                object obj = FCurrentActiveWritingThread[num];
                if (obj == null)
                {
                    return;
                }

                int num2 = (int)obj;
                if (num2 > 0)
                {
                    FCurrentActiveWritingThread[num] = num2 - 1;
                    if (num2 - 1 == 0)
                    {
                        FCurrentActiveWritingThread.Remove(num);
                        FMutexNoWriter.ReleaseMutex();
                    }
                }
            }
        }
    }
}