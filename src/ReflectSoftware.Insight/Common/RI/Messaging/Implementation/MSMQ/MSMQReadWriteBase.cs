using MSMQ.Messaging;
using RI.Messaging.ReadWriter;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using MQ = MSMQ.Messaging.MessageQueue;

namespace ReflectSoftware.Insight.Common.RI.Messaging.Implementation.MSMQ
{
    public abstract class MSMQReadWriteBase : IMessageReadWriterBase, IThreadExceptionEvent, IDisposable
    {
        internal MQ FMessageQueue;

        protected MSMQMessageFormatter FMessageFormatter;

        public MSMQConfigSetting Settings { get; private set; }

        public bool Disposed { get; private set; }

        public event OnThreadExceptionHandler OnThreadException;

        protected virtual void Initialize(MSMQConfigSetting settings)
        {
            Disposed = false;
            FMessageQueue = null;
            FMessageFormatter = new MSMQMessageFormatter();
            Settings = (MSMQConfigSetting)settings.Clone();
        }

        public MSMQReadWriteBase(string settingsId)
        {
            MSMQConfigSetting setting = MSMQConfiguration.GetSetting(settingsId);
            if (setting == null)
            {
                string message = $"Unable to obtain MSMQ settings from settings Id: {settingsId}. Please check configuration settings.";
                throw new Exception(message);
            }

            Initialize(setting);
        }

        public MSMQReadWriteBase(string name, string queuePath)
        {
            MSMQConfigSetting settings = new MSMQConfigSetting
            {
                Name = name,
                QueuePath = queuePath
            };
            Initialize(settings);
        }

        public MSMQReadWriteBase(MSMQConfigSetting settings)
        {
            Initialize(settings);
        }

        public MSMQReadWriteBase(NameValueCollection parameters)
        {
            MSMQConfigSetting settings = new MSMQConfigSetting
            {
                Name = parameters["name"],
                QueuePath = parameters["queuePath"]
            };
            Initialize(settings);
        }

        ~MSMQReadWriteBase()
        {
            Dispose(disposing: false);
        }

        protected void DoOnThreadException(Exception ex)
        {
            try
            {
                if (this.OnThreadException != null)
                {
                    lock (this)
                    {
                        this.OnThreadException(ex);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public bool IsThreadSafe()
        {
            return true;
        }

        protected abstract QueueAccessMode GetQueueAccessMode();

        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (!Disposed)
                {
                    Disposed = true;
                    GC.SuppressFinalize(this);
                    Close();
                    if (FMessageFormatter != null)
                    {
                        FMessageFormatter.Dispose();
                        FMessageFormatter = null;
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

        public bool IsOpen()
        {
            return FMessageQueue != null;
        }

        public virtual void Open()
        {
            //IL_0013: Unknown result type (might be due to invalid IL or missing references)
            //IL_0018: Unknown result type (might be due to invalid IL or missing references)
            //IL_0022: Expected O, but got Unknown
            Close();
            //            FMessageQueue = new MessageQueue(Settings.QueuePath, GetQueueAccessMode());
        }

        public virtual void Close()
        {
            if (FMessageQueue != null)
            {
                FMessageQueue.Close();
                ((Component)(object)FMessageQueue).Dispose();
                FMessageQueue = null;
            }
        }
    }
}
