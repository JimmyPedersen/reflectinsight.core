
using RI.Utils.ClassFactory;
using RI.Utils.Miscellaneous;
using RI.Utils.Strings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.ServiceModel.Channels;
using System.Threading;

namespace RI.Threading.Worker
{
    [ComVisible(false)]
    public delegate void WorkManagerOnHandler(WorkManager wManager);

    [ComVisible(false)]
    public class WorkManager : IDisposable
    {
        private readonly object FLockHandleObject;

        private readonly Hashtable FServices;

        private readonly List<BaseWorker> FWorkers;

        private Notification FNotification;

        private ThreadWatcher FThreadWatcher;

        private MessageManager FMessageManager;

        private List<ClassInfo> FClassFactories;

        private WorkManagerOnHandler FOnInitServicesHandler;

        private WorkManagerOnHandler FOnUninitServicesHandler;

        private WorkManagerOnHandler FOnConfigChangeHandler;

        public bool Disposed { get; private set; }

        public Notification Notification
        {
            get
            {
                lock (FLockHandleObject)
                {
                    return FNotification;
                }
            }
            set
            {
                lock (FLockHandleObject)
                {
                    FNotification = value;
                    FMessageManager.Notification = value;
                    FThreadWatcher.Notification = value;
                }
            }
        }

        public WorkManagerOnHandler OnInitServicesHandler
        {
            get
            {
                lock (FLockHandleObject)
                {
                    return FOnInitServicesHandler;
                }
            }
            set
            {
                lock (FLockHandleObject)
                {
                    FOnInitServicesHandler = value;
                }
            }
        }

        public WorkManagerOnHandler OnUninitServicesHandler
        {
            get
            {
                lock (FLockHandleObject)
                {
                    return FOnUninitServicesHandler;
                }
            }
            set
            {
                lock (FLockHandleObject)
                {
                    FOnUninitServicesHandler = value;
                }
            }
        }

        public WorkManagerOnHandler OnConfigChangeHandler
        {
            get
            {
                lock (FLockHandleObject)
                {
                    return FOnConfigChangeHandler;
                }
            }
            set
            {
                lock (FLockHandleObject)
                {
                    FOnConfigChangeHandler = value;
                }
            }
        }

        public WorkManager(Notification notifier)
        {
            Disposed = false;
            FLockHandleObject = new object();
            FThreadWatcher = new ThreadWatcher(this);
            FMessageManager = new MessageManager(this);
            Notification = notifier ?? new WorkManagerNotification();
            FClassFactories = new List<ClassInfo>();
            FServices = new Hashtable();
            FWorkers = new List<BaseWorker>();
            WorkManagerConfig.OnConfigChange += ConfigFileChanged;
        }

        public WorkManager()
            : this(null)
        {
        }

        public void Dispose()
        {
            lock (this)
            {
                if (Disposed)
                {
                    return;
                }

                Disposed = true;
                GC.SuppressFinalize(this);
                WorkManagerConfig.OnConfigChange -= ConfigFileChanged;
                if (FThreadWatcher != null)
                {
                    FThreadWatcher.Dispose();
                    FThreadWatcher = null;
                }

                if (FMessageManager != null)
                {
                    FMessageManager.Dispose();
                    FMessageManager = null;
                }

                if (FNotification != null)
                {
                    if (FNotification is IDisposable)
                    {
                        (FNotification as IDisposable).Dispose();
                    }

                    FNotification = null;
                }
            }
        }

        private void LoadClassFactories()
        {
            lock (this)
            {
                FClassFactories = ClassFactoryHelper.GetClassInfoList(WorkManagerConfig.Container, "./workManager/worker");
            }
        }

        private void ConfigFileChanged()
        {
            lock (this)
            {
                Stop();
                DoOnConfigChange();
                Start();
                WaitForAllWorkersToFullyStart();
            }
        }

        private void DoOnConfigChange()
        {
            lock (this)
            {
                if (OnConfigChangeHandler != null)
                {
                    try
                    {
                        OnConfigChangeHandler(this);
                    }
                    catch (Exception ex)
                    {
                        Notification.SendException(new WorkManagerException($"OnConfigChange handler detected an exception: {ex.Message}", ex), bIgnoreTracker: true);
                    }
                }
            }
        }

        private void DoOnInitServices()
        {
            if (OnInitServicesHandler != null)
            {
                try
                {
                    OnInitServicesHandler(this);
                }
                catch (Exception ex)
                {
                    Notification.SendException(new WorkManagerException($"OnOnInitServices handler detected an exception: {ex.Message}", ex), bIgnoreTracker: true);
                }
            }
        }

        private void DoOnUninitServices()
        {
            if (OnUninitServicesHandler != null)
            {
                try
                {
                    OnUninitServicesHandler(this);
                }
                catch (Exception ex)
                {
                    Notification.SendException(new WorkManagerException($"OnUninitServices handler detected an exception: {ex.Message}", ex), bIgnoreTracker: true);
                }
            }
        }

        private void RemoveAndDisposeAllServiceObjects()
        {
            foreach (object value in FServices.Values)
            {
                MiscHelper.DisposeObject(value);
            }

            FServices.Clear();
        }

        private void AddWorker(BaseWorker worker)
        {
            lock (FWorkers)
            {
                FWorkers.Add(worker);
            }
        }

        private void RemoveWorker(BaseWorker worker)
        {
            lock (FWorkers)
            {
                FWorkers.Remove(worker);
            }
        }

        internal void ReceivedMQMessage(MessageManagerInfo msg)
        {
            try
            {
                switch (msg.FMessageID)
                {
                    case MessageManagerID.SpawnWorker:
                        MQSpawnWorker(msg);
                        break;
                    case MessageManagerID.TerminateWorker:
                        MQTerminateWorker(msg);
                        break;
                    case MessageManagerID.WorkerFailedToRespond:
                        MQRemoveNonResponsiveWorker(msg);
                        break;
                }
            }
            catch (Exception ex)
            {
                Notification.SendException(ex, bIgnoreTracker: true);
            }
        }

        private void MQRemoveNonResponsiveWorker(MessageManagerInfo msg)
        {
            ThreadWatcherInfo threadWatcherInfo = (ThreadWatcherInfo)msg.FMessageData;
            BaseWorker baseWorker = threadWatcherInfo.ThreadObject as BaseWorker;
            if (baseWorker != null)
            {
                Notification.SendException(new Exception($"The following thread: '{baseWorker.Name}' failed to respond."), bIgnoreTracker: true);
                TerminateWorker(baseWorker);
            }
        }

        private void MQSpawnWorker(MessageManagerInfo msg)
        {
            BaseWorker baseWorker = (BaseWorker)msg.FMessageData;
            AddWorker(baseWorker);
            baseWorker.Start(bStartInBackground: true);
        }

        private void MQTerminateWorker(MessageManagerInfo msg)
        {
            BaseWorker baseWorker = (BaseWorker)msg.FMessageData;
            try
            {
                if (baseWorker != null)
                {
                    baseWorker.Stop();
                    DisposeWorker(baseWorker);
                    RemoveWorker(baseWorker);
                }
            }
            catch (WorkManagerException ex)
            {
                Notification.SendException(ex);
            }
            catch (Exception innerException)
            {
                string msg2 = $"An unhandled exception was detected while Terminating Worker '{baseWorker.Name}'";
                Notification.SendException(new WorkManagerException(msg2, innerException), bIgnoreTracker: true);
            }
        }

        internal BaseWorker CreateWorker(ClassInfo cInfo, object data)
        {
            if (cInfo == null)
            {
                return null;
            }

            WorkPackage workPackage = new WorkPackage(this, data);
            workPackage.Messenger = FMessageManager;
            workPackage.Watcher = FThreadWatcher;
            WorkPackage workPackage2 = workPackage;
            return ClassFactoryHelper.CreateInstance<BaseWorker>(cInfo, new object[1] { workPackage2 });
        }

        internal BaseWorker CreateWorker(string name, NameValueCollection parameters, object data)
        {
            ClassInfo classInfo = ClassFactoryHelper.GetClassInfo(FClassFactories, name);
            if (classInfo == null)
            {
                return null;
            }

            if (parameters != null)
            {
                string[] allKeys = parameters.AllKeys;
                foreach (string name2 in allKeys)
                {
                    classInfo.Parameters.Remove(name2);
                    classInfo.Parameters.Add(name2, parameters[name2]);
                }
            }

            return CreateWorker(classInfo, data);
        }

        internal BaseWorker SpawnWorker(BaseWorker worker)
        {
            FMessageManager.SendMessage(MessageManagerID.SpawnWorker, worker);
            return worker;
        }

        internal BaseWorker SpawnWorker(string name, NameValueCollection parameters, object data)
        {
            BaseWorker worker = CreateWorker(name, parameters, data);
            return SpawnWorker(worker);
        }

        internal void TerminateWorker(BaseWorker worker)
        {
            FMessageManager.SendMessage(MessageManagerID.TerminateWorker, worker);
        }

        internal void TerminateWorker(string name)
        {
            BaseWorker worker = GetWorker(name);
            if (worker != null)
            {
                FMessageManager.SendMessage(MessageManagerID.TerminateWorker, worker);
            }
        }

        internal void RemoveNonResponsiveWorker(ThreadWatcherInfo tInfo)
        {
            FMessageManager.SendMessage(MessageManagerID.WorkerFailedToRespond, tInfo);
        }

        internal BaseWorker GetWorker(string name)
        {
            lock (FWorkers)
            {
                foreach (BaseWorker fWorker in FWorkers)
                {
                    if (fWorker.Name == name)
                    {
                        return fWorker;
                    }
                }

                return null;
            }
        }

        private void LoadAndStartWorkers()
        {
            try
            {
                List<MessageManagerInfo> list = new List<MessageManagerInfo>();
                try
                {
                    LoadClassFactories();
                    foreach (ClassInfo fClassFactory in FClassFactories)
                    {
                        if (string.Compare(StringHelper.IfNullUseDefault(fClassFactory.Parameters["mode"], "on"), "on", ignoreCase: false) != 0)
                        {
                            continue;
                        }

                        string name = fClassFactory.Name;
                        int num = int.Parse(StringHelper.IfNullUseDefault(fClassFactory.Parameters["instances"], "1"));
                        if (num <= 0)
                        {
                            throw new WorkManagerException($"Worker: '{fClassFactory.Name}' 'instances' setting must be greater than 0. Please check the application configuration file.");
                        }

                        for (int i = 0; i < num; i++)
                        {
                            if (num > 1)
                            {
                                fClassFactory.Name = $"{name}.{i + 1}";
                            }

                            BaseWorker baseWorker = CreateWorker(fClassFactory, null);
                            if (baseWorker == null)
                            {
                                throw new WorkManagerException($"Unable to load worker: '{fClassFactory.Name}'. Please check the application configuration file.");
                            }

                            list.Add(new MessageManagerInfo(MessageManagerID.SpawnWorker, baseWorker));
                        }
                    }

                    if (list.Count == 0)
                    {
                        Notification.SendMessage("No Workers have been loaded.", NotificationType.Warning);
                    }
                }
                catch (Exception)
                {
                    foreach (MessageManagerInfo item in list)
                    {
                        item.Dispose();
                    }

                    throw;
                }

                FMessageManager.SendMessages(list);
            }
            catch (Exception ex2)
            {
                Notification.SendException(ex2, bIgnoreTracker: true);
            }
        }

        private void DisposeWorker(BaseWorker worker)
        {
            try
            {
                worker?.Dispose();
            }
            catch (Exception innerException)
            {
                string msg = $"An unhandled exception was detected disposing Worker '{worker.Name}'";
                Notification.SendException(new WorkManagerException(msg, innerException), bIgnoreTracker: true);
            }
        }

        private void TerminateWorkers()
        {
            BaseWorker[] array = null;
            lock (FWorkers)
            {
                array = new BaseWorker[FWorkers.Count];
                FWorkers.CopyTo(array);
            }

            BaseWorker[] array2 = array;
            foreach (BaseWorker worker in array2)
            {
                TerminateWorker(worker);
            }

            while (true)
            {
                lock (FWorkers)
                {
                    if (FWorkers.Count == 0)
                    {
                        return;
                    }
                }

                Thread.Sleep(100);
            }
        }

        private void WaitForAllWorkersToFullyStart()
        {
            FMessageManager.WaitOnEmptyQueue();
            foreach (BaseWorker fWorker in FWorkers)
            {
                fWorker.WaitForThreadState(BaseThread.BaseThreadState.Started, 1000);
            }
        }

        public void Start()
        {
            lock (this)
            {
                try
                {
                    FMessageManager.Start(bStartInBackground: true);
                    FThreadWatcher.Start(bStartInBackground: true);
                    DoOnInitServices();
                    LoadAndStartWorkers();
                }
                catch (Exception ex)
                {
                    try
                    {
                        Stop();
                        Notification.SendException(ex, bIgnoreTracker: true);
                    }
                    catch (Exception)
                    {
                    }

                    throw;
                }
            }
        }

        public void Stop()
        {
            lock (this)
            {
                try
                {
                    FMessageManager.SetAllMessagesToState(MessageState.Ignore);
                    FMessageManager.SetMessageState(MessageManagerID.TerminateWorker, MessageState.Allow);
                    TerminateWorkers();
                    FThreadWatcher.Stop();
                    FMessageManager.Stop();
                }
                catch (Exception ex)
                {
                    try
                    {
                        Notification.SendException(ex, bIgnoreTracker: true);
                    }
                    catch (Exception)
                    {
                    }

                    throw;
                }
                finally
                {
                    DoOnUninitServices();
                    RemoveAndDisposeAllServiceObjects();
                }
            }
        }

        public void RemoveService(string name, bool bDispose)
        {
            lock (FServices)
            {
                if (bDispose)
                {
                    MiscHelper.DisposeObject(FServices[name]);
                }

                FServices.Remove(name);
            }
        }

        public void RemoveService(string name)
        {
            RemoveService(name, bDispose: false);
        }

        public void AddService(string name, object sObject)
        {
            lock (FServices)
            {
                RemoveService(name);
                FServices[name] = sObject;
            }
        }

        public object GetService(string name)
        {
            lock (FServices)
            {
                return FServices[name];
            }
        }
    }
}