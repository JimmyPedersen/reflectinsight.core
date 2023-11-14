using RI.Utils.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace RI.Threading.Worker
{
    [ComVisible(false)]
    public abstract class BaseThread : IDisposable
    {
        public enum BaseThreadState
        {
            Stopped,
            Started
        }

        private static int ReferenceCount;

        private static readonly object LockObject;

        private static readonly List<BaseThread> ActiveThreads;

        private readonly object FThreadLock;

        private readonly object FHandlerLock;

        private Thread FThread;

        protected BaseThreadState FThreadState;

        protected bool FTerminated;

        protected Notification FNotification;

        public bool Disposed { get; private set; }

        public ulong DoWorkExecutionCount { get; private set; }

        public string Name { get; set; }

        public bool Terminated => FTerminated;

        public Notification Notification
        {
            get
            {
                lock (FHandlerLock)
                {
                    return FNotification;
                }
            }
            set
            {
                lock (FHandlerLock)
                {
                    FNotification = value;
                }
            }
        }

        public Thread ActiveThread => FThread;

        static BaseThread()
        {
            ReferenceCount = 0;
            LockObject = new object();
            ActiveThreads = new List<BaseThread>();
        }

        public BaseThread(string name, Notification notification)
        {
            FThreadLock = new object();
            FHandlerLock = new object();
            Disposed = false;
            Name = name;
            FThread = null;
            FTerminated = true;
            FNotification = notification ?? new Notification();
            FThreadState = BaseThreadState.Stopped;
            DoWorkExecutionCount = 0uL;
        }

        public BaseThread(string name)
            : this(name, new Notification())
        {
        }

        private void ProcessExiting()
        {
            lock (FThreadLock)
            {
                if (FThread != null)
                {
                    Stop();
                }
            }
        }

        public static void OnStartup()
        {
            lock (LockObject)
            {
                ReferenceCount++;
                _ = ReferenceCount;
            }
        }

        public static void OnShutdown()
        {
            lock (LockObject)
            {
                ReferenceCount--;
                if (ReferenceCount <= 0)
                {
                    BaseThread[] array = ActiveThreads.ToArray();
                    BaseThread[] array2 = array;
                    foreach (BaseThread baseThread in array2)
                    {
                        baseThread.ProcessExiting();
                    }

                    ReferenceCount = 0;
                    ActiveThreads.Clear();
                    ActiveThreads.Capacity = 0;
                }
            }
        }

        private static void AddActiveThread(BaseThread thread)
        {
            lock (ActiveThreads)
            {
                ActiveThreads.Add(thread);
            }
        }

        private static void RemoveActiveThread(BaseThread thread)
        {
            lock (ActiveThreads)
            {
                ActiveThreads.Remove(thread);
            }
        }

        protected virtual void Dispose(bool bDisposing)
        {
            lock (this)
            {
                if (Disposed)
                {
                    return;
                }

                Disposed = true;
                GC.SuppressFinalize(this);
                if (bDisposing)
                {
                    try
                    {
                        OnDispose();
                    }
                    catch (Exception ex)
                    {
                        string msg = $"OnDispose failed: An unhandled exception was detected in Thread '{Name} -> {ex.Message}'.";
                        throw new WorkManagerException(msg, ex);
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(bDisposing: true);
        }

        private void ThreadBeginSequence()
        {
            try
            {
                OnInitializeThread();
                OnStartedThread();
                Notification.SendEvent($"Thread '{Name}' was successfully started.", NotificationType.Information);
                FThreadState = BaseThreadState.Started;
            }
            catch (Exception ex)
            {
                Notification.SendException(ex);
            }
        }

        private void ThreadEndSequence()
        {
            try
            {
                OnStoppedThread();
                OnUninitializeThread();
                Notification.SendEvent($"Thread '{Name}' was successfully stopped.", NotificationType.Information);
                FThreadState = BaseThreadState.Stopped;
            }
            catch (Exception ex)
            {
                Notification.SendException(ex);
            }
        }

        private void _SafeThreadStop()
        {
            Stop();
        }

        private void Execute()
        {
            ThreadBeginSequence();
            try
            {
                FTerminated = false;
                while (!FTerminated)
                {
                    try
                    {
                        OnWork();
                        DoWorkExecutionCount++;
                        PassiveSleep(GetWorkSleepValue());
                    }
                    catch (ThreadAbortException)
                    {
                        Notification.SendEvent($"The following thread '{Name}' was aborted.", NotificationType.Fatal);
                        FTerminated = true;
                        return;
                    }
                    catch (Exception ex2)
                    {
                        string msg = $"Thread Execution: An unhandled exception was detected in thread '{Name}' -> {ex2.Message}.";
                        Notification.SendException(new WorkManagerException(msg, ex2), bIgnoreTracker: false);
                    }
                }
            }
            finally
            {
                ThreadEndSequence();
            }
        }

        private void SafeThreadStop()
        {
            FTerminated = true;
            new Thread(_SafeThreadStop).Start();
        }

        protected abstract void OnWork();

        protected virtual void OnDispose()
        {
        }

        protected virtual void OnInitializeThread()
        {
        }

        protected virtual void OnUninitializeThread()
        {
        }

        protected virtual void OnStartedThread()
        {
        }

        protected virtual void OnStoppedThread()
        {
        }

        protected virtual void OnStarting()
        {
        }

        protected virtual void OnStarted()
        {
        }

        protected virtual void OnStopping()
        {
        }

        protected virtual void OnStopped()
        {
        }

        protected virtual void PassiveSleep(int msecSleep)
        {
            MiscHelper.PassiveSleep(msecSleep, ref FTerminated);
        }

        protected virtual int GetWorkSleepValue()
        {
            return 5000;
        }

        protected virtual int GetWaitOnTerminateThreadValue()
        {
            return 10000;
        }

        public bool WaitForThreadState(BaseThreadState state, int mSec)
        {
            if (mSec != 0)
            {
                long num = mSec;
                while ((num > 0 || mSec == -1) && FThreadState != state)
                {
                    Thread.Sleep(100);
                    num -= 100;
                }
            }

            return FThreadState == state;
        }

        public void Start(bool bStartInBackground)
        {
            lock (FThreadLock)
            {
                if (FThread == null)
                {
                    OnStarting();
                    try
                    {
                        FTerminated = false;
                        Thread thread = new Thread(Execute);
                        thread.IsBackground = bStartInBackground;
                        thread.Start();
                        FThread = thread;
                        AddActiveThread(this);
                    }
                    finally
                    {
                        OnStarted();
                    }
                }
            }
        }

        public void Start()
        {
            Start(bStartInBackground: false);
        }

        public void Stop()
        {
            lock (FThreadLock)
            {
                if (FThread == null)
                {
                    return;
                }

                if (FThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
                {
                    OnStopping();
                    try
                    {
                        FTerminated = true;
                        WaitForThreadState(BaseThreadState.Stopped, GetWaitOnTerminateThreadValue());
                        if (FThreadState != 0)
                        {
                            FThread.Abort();
                        }

                        FThread = null;
                        RemoveActiveThread(this);
                    }
                    finally
                    {
                        OnStopped();
                    }
                }
                else
                {
                    SafeThreadStop();
                }
            }
        }

        internal void Terminate()
        {
            FTerminated = true;
        }
    }
}