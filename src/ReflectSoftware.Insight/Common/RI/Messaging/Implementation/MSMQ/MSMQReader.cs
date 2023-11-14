using MSMQ.Messaging;
using RI.Messaging.ReadWriter;
using System;
using System.Collections.Specialized;

namespace ReflectSoftware.Insight.Common.RI.Messaging.Implementation.MSMQ
{
    public class MSMQReader : MSMQReadWriteBase, IMessageReader, IMessageReadWriterBase, IDisposable
    {
        protected TimeoutException FTimeoutException;

        protected override void Initialize(MSMQConfigSetting settings)
        {
            base.Initialize(settings);
            FTimeoutException = new TimeoutException();
        }

        public MSMQReader(string settingsId)
            : base(settingsId)
        {
        }

        public MSMQReader(string name, string queuePath)
            : base(name, queuePath)
        {
        }

        public MSMQReader(MSMQConfigSetting settings)
            : base(settings)
        {
        }

        public MSMQReader(NameValueCollection parameters)
            : base(parameters)
        {
        }

        protected override QueueAccessMode GetQueueAccessMode()
        {
            return (QueueAccessMode)1;
        }

        public byte[] Read(int msecTimeout)
        {
            //IL_0060: Expected O, but got Unknown
            //IL_0061: Unknown result type (might be due to invalid IL or missing references)
            //IL_006b: Invalid comparison between Unknown and I4
            if (IsOpen())
            {
                try
                {
                    Message val = ((msecTimeout != -1) ? FMessageQueue.Receive(TimeSpan.FromMilliseconds(msecTimeout)) : FMessageQueue.Receive());
                    try
                    {
                        val.Formatter = ((IMessageFormatter)(object)FMessageFormatter);
                        return val.Body as byte[];
                    }
                    finally
                    {
                        ((IDisposable)val)?.Dispose();
                    }
                }
                catch (MessageQueueException val2)
                {
                    MessageQueueException val3 = val2;
                    if ((int)val3.MessageQueueErrorCode == -1072824293)
                    {
                        throw FTimeoutException;
                    }

                    Close();
                    throw;
                }
            }

            throw new Exception($"MSMQ connection not opened for reader: '{base.Settings.Name}'");
        }

        public byte[] Read()
        {
            return Read(-1);
        }
    }
}
