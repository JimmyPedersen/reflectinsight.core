using MSMQ.Messaging;
using RI.Messaging.ReadWriter;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace ReflectSoftware.Insight.Common.RI.Messaging.Implementation.MSMQ
{
    public class MSMQWriter : MSMQReadWriteBase, IMessageWriter, IMessageReadWriterBase, IDisposable
    {
        protected Message FMessage;

        public MSMQWriter(string settingsId)
            : base(settingsId)
        {
            CreateMessageContainer();
        }

        public MSMQWriter(string name, string queuePath, string timeToReceive, string timeToReach)
            : base(name, queuePath)
        {
            base.Settings.TimeToReceive = timeToReceive;
            base.Settings.TimeToReach = timeToReach;
            CreateMessageContainer();
        }

        public MSMQWriter(MSMQConfigSetting settings)
            : base(settings)
        {
            CreateMessageContainer();
        }

        public MSMQWriter(NameValueCollection parameters)
            : base(parameters)
        {
            base.Settings.TimeToReceive = parameters["timeToReceive"];
            base.Settings.TimeToReach = parameters["timeToReach"];
            CreateMessageContainer();
        }

        protected void CreateMessageContainer()
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_000b: Expected O, but got Unknown
            FMessage = new Message();
            FMessage.Label = "ReflectInsight";
            FMessage.Priority = MessagePriority.Normal;
            FMessage.Recoverable = false;
            FMessage.UseDeadLetterQueue = false;
            FMessage.UseAuthentication = false;
            FMessage.UseEncryption = false;
            FMessage.Formatter = ((IMessageFormatter)(object)FMessageFormatter);
            TimeSpan? timeSpan = GetTimeSpan(base.Settings.TimeToReceive);
            TimeSpan? timeSpan2 = GetTimeSpan(base.Settings.TimeToReach);
            if (timeSpan2.HasValue && timeSpan2.Value != TimeSpan.Zero)
            {
                FMessage.TimeToReachQueue = timeSpan2.Value;
            }

            if (timeSpan.HasValue && timeSpan.Value != TimeSpan.Zero)
            {
                FMessage.TimeToBeReceived = timeSpan.Value;
            }
        }

        private static TimeSpan? GetTimeSpan(string timeValue)
        {
            try
            {
                string[] array = timeValue.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length == 0)
                {
                    return null;
                }

                Array.Reverse(array);
                TimeSpan value = default(TimeSpan);
                if (array.Length >= 1)
                {
                    int result = 0;
                    int.TryParse(array[0], out result);
                    value = value.Add(TimeSpan.FromSeconds(result));
                }

                if (array.Length >= 2)
                {
                    int result2 = 0;
                    int.TryParse(array[1], out result2);
                    value = value.Add(TimeSpan.FromMinutes(result2));
                }

                if (array.Length >= 3)
                {
                    int result3 = 0;
                    int.TryParse(array[2], out result3);
                    value = value.Add(TimeSpan.FromHours(result3));
                }

                if (array.Length >= 4)
                {
                    int result4 = 0;
                    int.TryParse(array[3], out result4);
                    value = value.Add(TimeSpan.FromHours(result4));
                }

                return value;
            }
            catch (Exception innerException)
            {
                throw new Exception($"Invalid MSQM time value '{timeValue}'", innerException);
            }
        }

        protected override QueueAccessMode GetQueueAccessMode()
        {
            return (QueueAccessMode)2;
        }

        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                base.Dispose(disposing);
                if (FMessage != null)
                {
                    ((Component)(object)FMessage).Dispose();
                    FMessage = null;
                }
            }
        }

        public void Write(byte[] data)
        {
            if (!IsOpen())
            {
                throw new Exception($"MSMQ connection not opened for writer: '{base.Settings.Name}'");
            }

            FMessage.Body = (object)data;
            FMessageQueue.Send((object)FMessage);
        }

        public void Write(string data)
        {
            Write(Encoding.UTF8.GetBytes(data));
        }
    }
}
