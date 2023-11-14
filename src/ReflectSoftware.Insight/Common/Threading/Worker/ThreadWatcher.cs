
using System;
using System.Collections.Generic;
using System.Threading;

namespace RI.Threading.Worker
{
    internal class ThreadWatcherInfo
    {
        public object ThreadObject;

        public TimeSpan MaxAliveWindow;

        public DateTime LastResponseTime;

        public ThreadWatcherInfo(object thread, int aliveWindowMinutes)
        {
            ThreadObject = thread;
            MaxAliveWindow = new TimeSpan(0, aliveWindowMinutes, 0);
            LastResponseTime = DateTime.Now;
        }
    }

    internal class ThreadWatcher : BaseThread
    {
        private readonly List<ThreadWatcherInfo> Threads;

        private readonly WorkManager FWorkManager;

        private int FWorkSleep;

        public ThreadWatcher(WorkManager manager)
            : base("RI.Threading.Worker.ThreadWatcher")
        {
            Threads = new List<ThreadWatcherInfo>();
            FWorkSleep = 250;
            FWorkManager = manager;
        }

        protected override int GetWorkSleepValue()
        {
            return FWorkSleep;
        }

        protected override int GetWaitOnTerminateThreadValue()
        {
            return 5000;
        }

        protected override void OnWork()
        {
            try
            {
                ThreadWatcherInfo[] array = null;
                lock (Threads)
                {
                    array = Threads.ToArray();
                }

                if (array.Length == 0)
                {
                    return;
                }

                for (int i = 0; i < array.Length; i++)
                {
                    if (DateTime.Now.Subtract(array[i].LastResponseTime) > array[i].MaxAliveWindow)
                    {
                        if (FWorkManager != null)
                        {
                            FWorkManager.RemoveNonResponsiveWorker(array[i]);
                        }

                        lock (Threads)
                        {
                            Threads.Remove(array[i]);
                        }
                    }
                }

                PassiveSleep(FWorkSleep);
            }
            catch (Exception ex)
            {
                base.Notification.SendException(ex, bIgnoreTracker: true);
            }
        }

        public void AddThread(ThreadWatcherInfo tInfo)
        {
            if (!(tInfo.MaxAliveWindow == TimeSpan.Zero))
            {
                lock (Threads)
                {
                    Threads.Add(tInfo);
                    tInfo.LastResponseTime = DateTime.Now;
                }
            }
        }

        public void RemoveThread(ThreadWatcherInfo tInfo)
        {
            if (!(tInfo.MaxAliveWindow == TimeSpan.Zero))
            {
                lock (Threads)
                {
                    Threads.Remove(tInfo);
                }
            }
        }

        public static void PassiveSleep(ThreadWatcherInfo tInfo, long mSec, ref bool terminate)
        {
            long num = mSec;
            while (num > 0 && !terminate)
            {
                tInfo.LastResponseTime = DateTime.Now;
                Thread.Sleep(100);
                num -= 100;
            }

            tInfo.LastResponseTime = DateTime.Now;
        }
    }
}