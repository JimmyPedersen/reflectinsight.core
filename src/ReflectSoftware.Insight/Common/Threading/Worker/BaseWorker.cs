using ReflectSoftware.Insight;
using RI.Utils.ClassFactory;
using RI.Utils.Strings;
using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace RI.Threading.Worker
{
    [ComVisible(false)]
    public class WorkPackage
    {
        internal MessageManager Messenger;

        internal ThreadWatcher Watcher;

        public WorkManager Manager;

        public object Data;

        public WorkPackage(WorkManager manager, object data)
        {
            Manager = manager;
            Data = data;
        }
    }


    [ComVisible(false)]
    public abstract class BaseWorker : BaseThread, IClassFactoryInitialization
    {
        private int FWorkSleep;

        private NameValueCollection FParameters;

        private WorkPackage FWorkPackage;

        private ThreadWatcherInfo FThreadWatcherInfo;

        public WorkPackage WorkPackage => FWorkPackage;

        public BaseWorker()
            : base(string.Empty)
        {
        }

        public void Initialize(string name, NameValueCollection parameters, params object[] args)
        {
            lock (this)
            {
                try
                {
                    FWorkPackage = (WorkPackage)args[0];
                    if (FWorkPackage != null)
                    {
                        base.Notification = FWorkPackage.Manager.Notification;
                    }

                    base.Name = name;
                    FParameters = parameters;
                    FWorkSleep = int.Parse(Parameters("workSleep", "60000"));
                    FThreadWatcherInfo = new ThreadWatcherInfo(this, int.Parse(Parameters("aliveResponseWindow", "0")));
                    OnInitialize();
                }
                catch (Exception ex)
                {
                    string msg = $"Initialize failed: An unhandled exception was detected in Thread '{base.Name} -> {ex.Message}'.";
                    throw new WorkManagerException(msg, ex);
                }
            }
        }

        protected string Parameters(string name, string defaultValue)
        {
            return StringHelper.IfNullOrEmptyUseDefault(FParameters[name], defaultValue);
        }

        protected virtual void OnInitialize()
        {
        }

        protected override int GetWorkSleepValue()
        {
            return FWorkSleep;
        }

        protected override void PassiveSleep(int sleep)
        {
            ThreadWatcher.PassiveSleep(FThreadWatcherInfo, sleep, ref FTerminated);
        }

        protected override int GetWaitOnTerminateThreadValue()
        {
            return WorkManagerConfig.WaitOnTerminateThread;
        }

        protected override void OnStartedThread()
        {
            FWorkPackage.Watcher.AddThread(FThreadWatcherInfo);
        }

        protected override void OnStoppedThread()
        {
            FWorkPackage.Watcher.RemoveThread(FThreadWatcherInfo);
        }

        protected bool WaitForWorkerOnState(BaseWorker worker, BaseThreadState state, int waitmsec)
        {
            if (waitmsec != 0)
            {
                long num = waitmsec;
                while ((num > 0 || waitmsec == -1) && !FTerminated && worker.FThreadState != state)
                {
                    PassiveSleep(100);
                    num -= 100;
                }
            }

            return FThreadState == state;
        }

        protected BaseWorker CreateWorker(string name, NameValueCollection parameters, object data)
        {
            return FWorkPackage.Manager.CreateWorker(name, parameters, data);
        }

        protected BaseWorker CreateWorker(string name, NameValueCollection parameters)
        {
            return CreateWorker(name, parameters, null);
        }

        protected BaseWorker CreateWorker(string name)
        {
            return CreateWorker(name, null);
        }

        protected BaseWorker SpawnWorker(string name, NameValueCollection parameters, object data, int waitmsec)
        {
            BaseWorker baseWorker = FWorkPackage.Manager.SpawnWorker(name, parameters, data);
            WaitForWorkerOnState(baseWorker, BaseThreadState.Started, waitmsec);
            return baseWorker;
        }

        protected BaseWorker SpawnWorker(string name, NameValueCollection parameters, object data)
        {
            return SpawnWorker(name, parameters, data, 0);
        }

        protected BaseWorker SpawnWorker(string name, NameValueCollection parameters)
        {
            return SpawnWorker(name, parameters, null);
        }

        protected static BaseWorker SpawnWorker(string name, int waitmsec)
        {
            return SpawnWorker(name, waitmsec);
        }

        protected BaseWorker SpawnWorker(BaseWorker worker, int waitmsec)
        {
            FWorkPackage.Manager.SpawnWorker(worker);
            WaitForWorkerOnState(worker, BaseThreadState.Started, waitmsec);
            return worker;
        }

        protected BaseWorker SpawnWorker(BaseWorker worker)
        {
            return SpawnWorker(worker, 0);
        }

        protected void TerminateWorker(BaseWorker worker)
        {
            FWorkPackage.Manager.TerminateWorker(worker);
        }

        protected void TerminateWorker(string name)
        {
            FWorkPackage.Manager.TerminateWorker(name);
        }

        public void Join(BaseWorker worker, int waitmsec)
        {
            if (waitmsec != 0)
            {
                long num = waitmsec;
                while ((num > 0 || waitmsec == -1) && !FTerminated && worker.ActiveThread != null)
                {
                    PassiveSleep(100);
                    num -= 100;
                }
            }
        }

        public void Join(BaseWorker worker)
        {
            Join(worker, -1);
        }

        public new void Terminate()
        {
            base.Terminate();
            TerminateWorker(this);
        }

        protected BaseWorker GetWorker(string name)
        {
            return FWorkPackage.Manager.GetWorker(name);
        }
    }
}