using RI.Utils.Miscellaneous;
using System;

namespace RI.Threading.Worker
{
    internal enum MessageManagerID
    {
        SpawnWorker,
        TerminateWorker,
        WorkerFailedToRespond,
        RestartManager
    }

    internal class MessageManagerInfo : IDisposable
    {
        public MessageManagerID FMessageID;

        public object FMessageData;

        public bool Disposed { get; private set; }

        public MessageManagerInfo(MessageManagerID mid, object mData)
        {
            Disposed = false;
            FMessageID = mid;
            FMessageData = mData;
        }

        public void Dispose()
        {
            lock (this)
            {
                if (!Disposed)
                {
                    Disposed = true;
                    GC.SuppressFinalize(this);
                    if (FMessageData != null)
                    {
                        MiscHelper.DisposeObject(FMessageData);
                        FMessageData = null;
                    }
                }
            }
        }
    }
}