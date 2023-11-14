#region Assembly ReflectSoftware.Insight, Version=5.7.1.1706, Culture=neutral, PublicKeyToken=c78ddbdaf1f32b08
// C:\Work\Medexa\MSWC_v5\500-Software_PC\packages\ReflectSoftware.Insight.5.7.1.1\lib\net45\ReflectSoftware.Insight.dll
// Decompiled with ICSharpCode.Decompiler 7.1.0.6543
#endregion

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RI.Threading.Worker
{
    [ComVisible(false)]
    public enum NotificationType
    {
        Message,
        Debug,
        Information,
        Warning,
        Error,
        Fatal,
        Exception
    }

    [ComVisible(false)]
    public class Notification
    {
        protected virtual void WriteEntry(string source, string msg, EventLogEntryType eType, int eventID, short category, byte[] rawData)
        {
        }

        public virtual void SendEvent(string msg, NotificationType nType)
        {
        }

        public virtual void SendMessage(string msg, NotificationType nType, Exception ex)
        {
        }

        public void SendMessage(string msg, NotificationType nType)
        {
            SendMessage(msg, nType, null);
        }

        public virtual void SendException(Exception ex, bool bIgnoreTracker)
        {
        }

        public void SendException(Exception ex)
        {
            SendException(ex, bIgnoreTracker: false);
        }
    }
}