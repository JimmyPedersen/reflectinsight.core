using MSMQ.Messaging;
using System;
using System.IO;

namespace ReflectSoftware.Insight.Common.RI.Messaging.Implementation.MSMQ
{
    public class MSMQMessageFormatter : ICloneable, IDisposable
    {
        private MemoryStream FStream;

        public bool Disposed { get; private set; }

        public MSMQMessageFormatter()
        {
            Disposed = false;
            FStream = new MemoryStream();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (!Disposed)
                {
                    Disposed = true;
                    GC.SuppressFinalize(this);
                    if (FStream != null)
                    {
                        FStream.Dispose();
                        FStream = null;
                    }
                }
            }
        }

        public bool CanRead(Message message)
        {
            return true;
        }

        public void Write(Message message, object obj)
        {
            byte[] array = (byte[])obj;
            FStream.Position = 0L;
            FStream.SetLength(0L);
            FStream.Write(array, 0, array.Length);
            message.BodyStream = (Stream)FStream;
        }

        public object Read(Message message)
        {
            byte[] array = new byte[message.BodyStream.Length];
            message.BodyStream.Read(array, 0, array.Length);
            return array;
        }

        public object Clone()
        {
            return this;
        }
    }
}