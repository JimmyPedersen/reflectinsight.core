
using Plato.Managers;
using Plato.Miscellaneous;
using RI.Utils.Strings;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RI.Threading.Worker
{
    [ComVisible(false)]
    public class WorkManagerNotification : Notification, IDisposable
    {
        private readonly string FSourceName;

        private TimeSpan FEventTracker;

        public bool Disposed { get; private set; }

        public WorkManagerNotification()
        {
            Disposed = false;
            FSourceName = WorkManagerConfig.ApplicationName;
            ObtainConfigInfo();
            ExceptionManager.OnConfigChange += ConfigChange;
        }

        private void ObtainConfigInfo()
        {
            lock (this)
            {
                FEventTracker = new TimeSpan(0, WorkManagerConfig.EventTracker, 0);
                NameValueCollection nameValueCollection = new NameValueCollection();
                nameValueCollection.Add("applicationName", FSourceName);
                ExceptionEventPublisher publisher = new ExceptionEventPublisher();
                ExceptionManager.RemovePublisherByType(typeof(ExceptionEventPublisher));
                ExceptionManager.AddPublisher(publisher, nameValueCollection);
            }
        }

        private void ConfigChange()
        {
            lock (this)
            {
                ObtainConfigInfo();
                OnConfig();
            }
        }

        protected virtual void OnConfig()
        {
        }

        public void Dispose()
        {
            lock (this)
            {
                if (!Disposed)
                {
                    Disposed = true;
                    GC.SuppressFinalize(this);
                    ExceptionManager.OnConfigChange -= ConfigChange;
                }
            }
        }

        public override void SendMessage(string msg, NotificationType nType, Exception ex)
        {
            Console.WriteLine("{0}: [{1}]", msg, nType);
        }

        protected override void WriteEntry(string source, string msg, EventLogEntryType eType, int eventID, short category, byte[] rawData)
        {
            try
            {
                EventLog.WriteEntry(source, msg, eType, eventID, category, rawData);
            }
            catch (Exception)
            {
            }
        }

        public override void SendEvent(string msg, NotificationType nType)
        {
            EventLogEntryType eventLogEntryType = EventLogEntryType.Information;
            switch (nType)
            {
                case NotificationType.Warning:
                    eventLogEntryType = EventLogEntryType.Warning;
                    break;
                case NotificationType.Error:
                case NotificationType.Fatal:
                case NotificationType.Exception:
                    eventLogEntryType = EventLogEntryType.Error;
                    break;
                default:
                    eventLogEntryType = EventLogEntryType.Information;
                    break;
            }

            SendMessage(msg, nType, null);
            WriteEntry(FSourceName, msg, eventLogEntryType, 0, 0, null);
        }

        public override void SendException(Exception ex, bool bIgnoreTracker)
        {
            if (bIgnoreTracker || TimeEventTracker.CanEvent((int)StringHash.BKDRHash(ex.Message), FEventTracker))
            {
                SendMessage(ex.Message, NotificationType.Exception, ex);
                ExceptionManager.Publish(ex);
            }
        }
    }
}