using Plato.Security.Cryptography;
using ReflectSoftware.Insight.Common.Threading.Guard;
using RI.Threading.Guard;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;


namespace ReflectSoftware.Insight.Common
{
    [Serializable]
    public struct RISessionInfo
    {
        public ulong SessionId;

        public bool Hooked;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Reserved;
    }

    [Serializable]
    public struct RIGlobalViewerSessions
    {
        public double TimeStamp;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public RISessionInfo[] Sessions;
    }

    public class RISessionBase : IDisposable
    {
        protected static MutexSecurity FMutexSecurity;

        protected static SecurityIdentifier FEveryoneSID;

        protected ulong FSessionId;

        protected SingleWriteMultipleReadGuard FGlobalViewerGuard;

        protected SingleWriteMultipleReadGuard FViewerSessionGuard;

        protected SharedMemory<RIGlobalViewerSessions> FGlobalViewerSessions;

        protected SharedMemory<RISessionInfo> FViewerSession;

        public bool Disposed { get; private set; }

        public double RawTimeStamp => FGlobalViewerSessions.Memory.TimeStamp;

        public DateTime TimeStamp => DateTime.FromOADate(RawTimeStamp);

        public ulong SessionId => FSessionId;

        static RISessionBase()
        {
            FEveryoneSID = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            FMutexSecurity = new MutexSecurity();
            FMutexSecurity.AddAccessRule(new MutexAccessRule(FEveryoneSID, MutexRights.FullControl, AccessControlType.Allow));
        }

        public RISessionBase()
        {
            Disposed = false;
            FGlobalViewerGuard = new SingleWriteMultipleReadGuard("Global\\ReflectInsight.GlobalViewerSessions");
            FViewerSessionGuard = new SingleWriteMultipleReadGuard("Local\\ReflectInsight.ViewerSession");
            using (new WriteGuardControllor(FGlobalViewerGuard))
            {
                string text = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\ReflectSoftware\\ReflectInsight";
                if (!Directory.Exists(text))
                {
                    DirectorySecurity directorySecurity = new DirectorySecurity();
                    directorySecurity.AddAccessRule(new FileSystemAccessRule(FEveryoneSID, FileSystemRights.FullControl, AccessControlType.Allow));
                    Directory.CreateDirectory(text);//, directorySecurity);
                }

                string fileName = $"{text}\\Sessions.bin";
                FGlobalViewerSessions = new SharedMemory<RIGlobalViewerSessions>(fileName, "ReflectInsight.Sessions");
                if (FGlobalViewerSessions.Creator)
                {
                    RIGlobalViewerSessions memory = FGlobalViewerSessions.Memory;
                    for (int i = 0; i < memory.Sessions.Length; i++)
                    {
                        memory.Sessions[i].SessionId = 0uL;
                        memory.Sessions[i].Hooked = false;
                    }

                    memory.TimeStamp = DateTime.Now.ToOADate();
                    FGlobalViewerSessions.Memory = memory;
                }
            }

            using (new WriteGuardControllor(FViewerSessionGuard))
            {
                FViewerSession = new SharedMemory<RISessionInfo>("Local\\ReflectInsight.WindowStationSession");
                if (FViewerSession.Creator)
                {
                    FSessionId = CryptoServices.RandomIdToUInt64();
                    RISessionInfo memory2 = FViewerSession.Memory;
                    memory2.SessionId = FSessionId;
                    memory2.Hooked = false;
                    FViewerSession.Memory = memory2;
                }
                else
                {
                    FSessionId = FViewerSession.Memory.SessionId;
                }
            }

            if (!FGlobalViewerSessions.Creator)
            {
                RemoveDeadSessions();
            }
        }

        private void RemoveDeadSessions()
        {
            using (new WriteGuardControllor(FGlobalViewerGuard))
            {
                bool flag = false;
                RIGlobalViewerSessions memory = FGlobalViewerSessions.Memory;
                for (int i = 0; i < memory.Sessions.Length; i++)
                {
                    if (memory.Sessions[i].SessionId == 0L || memory.Sessions[i].SessionId == FSessionId)
                    {
                        continue;
                    }

                    bool createdNew;
                    using (new Mutex(initiallyOwned: true, $"Global\\ReflectInsight.GlobalViewerSessions.Mutex.{memory.Sessions[i].SessionId}", out createdNew))//, FMutexSecurity))
                    {
                        if (createdNew)
                        {
                            flag = true;
                            memory.Sessions[i].SessionId = 0uL;
                            memory.Sessions[i].Hooked = false;
                        }
                    }
                }

                if (flag)
                {
                    memory.TimeStamp = DateTime.Now.ToOADate();
                    FGlobalViewerSessions.Memory = memory;
                }
            }
        }

        public virtual void Dispose()
        {
            lock (this)
            {
                if (Disposed)
                {
                    return;
                }

                Disposed = true;
                GC.SuppressFinalize(this);
                using (new WriteGuardControllor(FViewerSessionGuard))
                {
                    if (FViewerSession != null)
                    {
                        FViewerSession.Dispose();
                        FViewerSession = null;
                    }
                }

                using (new WriteGuardControllor(FGlobalViewerGuard))
                {
                    if (FGlobalViewerSessions != null)
                    {
                        FGlobalViewerSessions.Dispose();
                        FGlobalViewerSessions = null;
                    }
                }

                if (FViewerSessionGuard != null)
                {
                    FViewerSessionGuard.Dispose();
                    FViewerSessionGuard = null;
                }

                if (FGlobalViewerGuard != null)
                {
                    FGlobalViewerGuard.Dispose();
                    FGlobalViewerGuard = null;
                }
            }
        }

        public void RemoveSession(ulong sid)
        {
            using (new WriteGuardControllor(FGlobalViewerGuard))
            {
                RIGlobalViewerSessions memory = FGlobalViewerSessions.Memory;
                for (int i = 0; i < memory.Sessions.Length; i++)
                {
                    if (memory.Sessions[i].SessionId == sid)
                    {
                        memory.Sessions[i].SessionId = 0uL;
                        memory.Sessions[i].Hooked = false;
                        memory.TimeStamp = DateTime.Now.ToOADate();
                        FGlobalViewerSessions.Memory = memory;
                        break;
                    }
                }
            }
        }
    }
}
