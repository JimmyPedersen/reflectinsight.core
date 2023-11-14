using RI.Threading.Guard;
using System;
using System.Runtime.InteropServices;

namespace ReflectSoftware.Insight.Common.Threading.Guard
{
    [ComVisible(false)]
    public class WriteGuardControllor : IDisposable
    {
        private SingleWriteMultipleReadGuard FGuard;

        public bool Disposed { get; private set; }

        public WriteGuardControllor(SingleWriteMultipleReadGuard guard)
        {
            Disposed = false;
            FGuard = guard;
            FGuard.Writing();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (!Disposed)
                {
                    Disposed = true;
                    GC.SuppressFinalize(this);
                    if (FGuard != null)
                    {
                        FGuard.DoneWriting();
                        FGuard = null;
                    }
                }
            }
        }
    }
}
