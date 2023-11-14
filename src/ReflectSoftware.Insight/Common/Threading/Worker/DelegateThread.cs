using System.Runtime.InteropServices;

namespace RI.Threading.Worker
{
    [ComVisible(false)]
    public delegate void DelegateThreadOnHandler();

    [ComVisible(false)]
    public delegate void DelegateThreadOnPassiveHandler(int msecSleep);

    [ComVisible(false)]
    public class DelegateThread : BaseThread
    {
        private readonly object FOnThreadHandlerLock;

        private DelegateThreadOnHandler FDelegateOnWork;

        private DelegateThreadOnHandler FDelegateOnInitializeThread;

        private DelegateThreadOnHandler FDelegateOnUninitializeThread;

        private DelegateThreadOnHandler FDelegateOnStartedThread;

        private DelegateThreadOnHandler FDelegateOnStoppedThread;

        private DelegateThreadOnHandler FDelegateOnStarting;

        private DelegateThreadOnHandler FDelegateOnStarted;

        private DelegateThreadOnHandler FDelegateOnStopping;

        private DelegateThreadOnHandler FDelegateOnStopped;

        private DelegateThreadOnHandler FDelegateOnDisposed;

        private DelegateThreadOnPassiveHandler FDelegateOnPassiveSleep;

        public int WorkSleepValue { get; set; }

        public int WaitOnTerminateValue { get; set; }

        public bool IsRunning => base.ActiveThread != null;

        public DelegateThreadOnHandler DelegateOnWork
        {
            get
            {
                lock (FOnThreadHandlerLock)
                {
                    return FDelegateOnWork;
                }
            }
            set
            {
                lock (FOnThreadHandlerLock)
                {
                    FDelegateOnWork = value;
                }
            }
        }

        public DelegateThreadOnHandler DelegateOnInitializeThread
        {
            get
            {
                lock (FOnThreadHandlerLock)
                {
                    return FDelegateOnInitializeThread;
                }
            }
            set
            {
                lock (FOnThreadHandlerLock)
                {
                    FDelegateOnInitializeThread = value;
                }
            }
        }

        public DelegateThreadOnHandler DelegateOnUninitializeThread
        {
            get
            {
                lock (FOnThreadHandlerLock)
                {
                    return FDelegateOnUninitializeThread;
                }
            }
            set
            {
                lock (FOnThreadHandlerLock)
                {
                    FDelegateOnUninitializeThread = value;
                }
            }
        }

        public DelegateThreadOnHandler DelegateOnStartedThread
        {
            get
            {
                lock (FOnThreadHandlerLock)
                {
                    return FDelegateOnStartedThread;
                }
            }
            set
            {
                lock (FOnThreadHandlerLock)
                {
                    FDelegateOnStartedThread = value;
                }
            }
        }

        public DelegateThreadOnHandler DelegateOnStoppedThread
        {
            get
            {
                lock (FOnThreadHandlerLock)
                {
                    return FDelegateOnStoppedThread;
                }
            }
            set
            {
                lock (FOnThreadHandlerLock)
                {
                    FDelegateOnStoppedThread = value;
                }
            }
        }

        public DelegateThreadOnHandler DelegateOnStarting
        {
            get
            {
                lock (FOnThreadHandlerLock)
                {
                    return FDelegateOnStarting;
                }
            }
            set
            {
                lock (FOnThreadHandlerLock)
                {
                    FDelegateOnStarting = value;
                }
            }
        }

        public DelegateThreadOnHandler DelegateOnStarted
        {
            get
            {
                lock (FOnThreadHandlerLock)
                {
                    return FDelegateOnStarted;
                }
            }
            set
            {
                lock (FOnThreadHandlerLock)
                {
                    FDelegateOnStarted = value;
                }
            }
        }

        public DelegateThreadOnHandler DelegateOnStopping
        {
            get
            {
                lock (FOnThreadHandlerLock)
                {
                    return FDelegateOnStopping;
                }
            }
            set
            {
                lock (FOnThreadHandlerLock)
                {
                    FDelegateOnStopping = value;
                }
            }
        }

        public DelegateThreadOnHandler DelegateOnStopped
        {
            get
            {
                lock (FOnThreadHandlerLock)
                {
                    return FDelegateOnStopped;
                }
            }
            set
            {
                lock (FOnThreadHandlerLock)
                {
                    FDelegateOnStopped = value;
                }
            }
        }

        public DelegateThreadOnHandler DelegateOnDispose
        {
            get
            {
                lock (FOnThreadHandlerLock)
                {
                    return FDelegateOnDisposed;
                }
            }
            set
            {
                lock (FOnThreadHandlerLock)
                {
                    FDelegateOnDisposed = value;
                }
            }
        }

        public DelegateThreadOnPassiveHandler DelegateOnPassiveSleep
        {
            get
            {
                lock (FOnThreadHandlerLock)
                {
                    return FDelegateOnPassiveSleep;
                }
            }
            set
            {
                lock (FOnThreadHandlerLock)
                {
                    FDelegateOnPassiveSleep = value;
                }
            }
        }

        public DelegateThread(string name)
            : base(name)
        {
            FOnThreadHandlerLock = new object();
            WorkSleepValue = 5000;
            WaitOnTerminateValue = 10000;
            DelegateOnWork = null;
            DelegateOnDispose = null;
            DelegateOnInitializeThread = null;
            DelegateOnUninitializeThread = null;
            DelegateOnStartedThread = null;
            DelegateOnStoppedThread = null;
            DelegateOnStarting = null;
            DelegateOnStarted = null;
            DelegateOnStopping = null;
            DelegateOnStopped = null;
            DelegateOnPassiveSleep = null;
        }

        protected override int GetWorkSleepValue()
        {
            return WorkSleepValue;
        }

        protected override int GetWaitOnTerminateThreadValue()
        {
            return WaitOnTerminateValue;
        }

        protected override void OnWork()
        {
            if (DelegateOnWork != null)
            {
                DelegateOnWork();
            }
        }

        protected override void PassiveSleep(int msecSleep)
        {
            if (DelegateOnPassiveSleep != null)
            {
                DelegateOnPassiveSleep(msecSleep);
            }
            else
            {
                base.PassiveSleep(msecSleep);
            }
        }

        protected override void OnInitializeThread()
        {
            if (DelegateOnInitializeThread != null)
            {
                DelegateOnInitializeThread();
            }
        }

        protected override void OnUninitializeThread()
        {
            if (DelegateOnUninitializeThread != null)
            {
                DelegateOnUninitializeThread();
            }
        }

        protected override void OnStartedThread()
        {
            if (DelegateOnStartedThread != null)
            {
                DelegateOnStartedThread();
            }
        }

        protected override void OnStoppedThread()
        {
            if (DelegateOnStoppedThread != null)
            {
                DelegateOnStoppedThread();
            }
        }

        protected override void OnStarting()
        {
            if (DelegateOnStarting != null)
            {
                DelegateOnStarting();
            }
        }

        protected override void OnStarted()
        {
            if (DelegateOnStarted != null)
            {
                DelegateOnStarted();
            }
        }

        protected override void OnStopping()
        {
            if (DelegateOnStopping != null)
            {
                DelegateOnStopping();
            }
        }

        protected override void OnStopped()
        {
            if (DelegateOnStopped != null)
            {
                DelegateOnStopped();
            }
        }

        protected override void OnDispose()
        {
            if (DelegateOnDispose != null)
            {
                DelegateOnDispose();
            }
        }
    }
}