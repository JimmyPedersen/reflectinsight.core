using System;
using System.Runtime.InteropServices;

namespace RI.Threading.Guard
{
    [ComVisible(false)]
    public class ReadGuardControllor : IDisposable
    {
        private SingleWriteMultipleReadGuard FGuard;

        public bool Disposed { get; private set; }

        public ReadGuardControllor(SingleWriteMultipleReadGuard guard)
        {
            Disposed = false;
            FGuard = guard;
            FGuard.Reading();
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
                        FGuard.DoneReading();
                        FGuard = null;
                    }
                }
            }
        }
    }
}