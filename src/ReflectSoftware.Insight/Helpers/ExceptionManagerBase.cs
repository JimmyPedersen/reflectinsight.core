
using Plato.Configuration;
using RI.Utils.ClassFactory;
using RI.Utils.Miscellaneous;
using RI.Utils.Strings;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RI.Utils.ExceptionManagement
{
    [ComVisible(false)]
    public class ExceptionManagerBase : IDisposable
    {
        protected class UnhandledExceptionInfo
        {
            public ClassInfo FClassInfo;

            public Exception FException;
        }

        protected string FEventLogSource;

        protected ConfigContainer FConfig;

        protected List<PublisherInfo> FPublishers;

        protected List<UnhandledExceptionInfo> FUnhandledExceptions;

        public Action OnConfigChange;

        public bool Disposed { get; private set; }

        public PublisherManagerMode Mode { get; private set; }

        public int PublisherCount
        {
            get
            {
                lock (FPublishers)
                {
                    return FPublishers.Count;
                }
            }
        }

        public PublisherInfo[] PublisherInfos
        {
            get
            {
                lock (FPublishers)
                {
                    return FPublishers.ToArray();
                }
            }
        }

        public IExceptionPublisher[] Publishers
        {
            get
            {
                lock (FPublishers)
                {
                    List<IExceptionPublisher> list = new List<IExceptionPublisher>();
                    foreach (PublisherInfo fPublisher in FPublishers)
                    {
                        list.Add(fPublisher.FPublisher);
                    }

                    return list.ToArray();
                }
            }
        }

        public ExceptionManagerBase(string fileName, string configSection, string eventLogSource, bool bTrackConfigChange, bool bInitLoadPublishers)
        {
            Disposed = false;
            FUnhandledExceptions = new List<UnhandledExceptionInfo>();
            FPublishers = new List<PublisherInfo>();
            FEventLogSource = eventLogSource;
            string fileName2 = ((!string.IsNullOrEmpty(fileName)) ? fileName : AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            FConfig = new ConfigContainer(bTrackConfigChange, fileName2, configSection);
            if (bInitLoadPublishers)
            {
                ReloadPublishers();
            }
        }

        public ExceptionManagerBase(string configSection, string eventLogSource, bool bTrackConfigChange, bool bInitLoadPublishers)
            : this(null, configSection, eventLogSource, bTrackConfigChange, bInitLoadPublishers)
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
                    FreePublishers();
                    if (FConfig != null)
                    {
                        FConfig.Dispose();
                        FConfig = null;
                    }
                }
            }
        }

        public void ForceLoadConfigFile()
        {
            FConfig.ForceReloadConfigFile();
        }

        public void OnConfigFileChange()
        {
            lock (FPublishers)
            {
                if (string.Compare(FConfig.Node.GetAttribute(".", "mode", "on"), "on", ignoreCase: false) != 0)
                {
                    Mode = PublisherManagerMode.Off;
                }
                else
                {
                    Mode = PublisherManagerMode.On;
                }

                ReloadPublishers();
                if (OnConfigChange != null)
                {
                    OnConfigChange();
                }
            }
        }

        public void FreePublishers()
        {
            lock (FPublishers)
            {
                foreach (PublisherInfo fPublisher in FPublishers)
                {
                    MiscHelper.DisposeObject(fPublisher.FPublisher);
                }

                FPublishers.Clear();
                FPublishers.Capacity = 0;
            }
        }

        protected void ReloadPublishers()
        {
            lock (FPublishers)
            {
                FreePublishers();
                if (Mode == PublisherManagerMode.Off)
                {
                    return;
                }

                List<ClassInfo> classInfoList = ClassFactoryHelper.GetClassInfoList(FConfig, "publisher");
                foreach (ClassInfo item2 in classInfoList)
                {
                    if (string.Compare(StringHelper.IfNullOrEmptyUseDefault(item2.Parameters["mode"], "on"), "on", ignoreCase: false) == 0)
                    {
                        IExceptionPublisher exceptionPublisher = ClassFactoryHelper.CreateInstance<IExceptionPublisher>(item2, new object[0]);
                        if (exceptionPublisher != null)
                        {
                            PublisherInfo publisherInfo = new PublisherInfo();
                            publisherInfo.FClassInfo = item2;
                            publisherInfo.FPublisher = exceptionPublisher;
                            PublisherInfo item = publisherInfo;
                            FPublishers.Add(item);
                        }
                    }
                }
            }
        }

        public void Publish(Exception ex, NameValueCollection additionalParameters)
        {
            lock (FPublishers)
            {
                try
                {
                    try
                    {
                        FUnhandledExceptions.Clear();
                        FUnhandledExceptions.Capacity = 0;
                        foreach (PublisherInfo fPublisher in FPublishers)
                        {
                            try
                            {
                                fPublisher.FPublisher.Publish(ex, additionalParameters);
                            }
                            catch (Exception fException)
                            {
                                UnhandledExceptionInfo unhandledExceptionInfo = new UnhandledExceptionInfo();
                                unhandledExceptionInfo.FClassInfo = fPublisher.FClassInfo;
                                unhandledExceptionInfo.FException = fException;
                                UnhandledExceptionInfo item = unhandledExceptionInfo;
                                FUnhandledExceptions.Add(item);
                            }
                        }

                        foreach (UnhandledExceptionInfo fUnhandledException in FUnhandledExceptions)
                        {
                            string message = $"The following Publisher: '{fUnhandledException.FClassInfo.Name}' caused an Unhandled exception";
                            Exception exception = new Exception(message, fUnhandledException.FException);
                            EventLog.WriteEntry($"Internal {FEventLogSource}", ExceptionBasePublisher.ConstructMessage(exception), EventLogEntryType.Error);
                        }
                    }
                    catch (Exception exception2)
                    {
                        EventLog.WriteEntry($"Internal {FEventLogSource}", ExceptionBasePublisher.ConstructMessage(exception2), EventLogEntryType.Error);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public void Publish(Exception ex)
        {
            Publish(ex, new NameValueCollection());
        }

        public void RemovePublisherByType(Type pType)
        {
            lock (FPublishers)
            {
                foreach (PublisherInfo fPublisher in FPublishers)
                {
                    if (fPublisher.FPublisher.GetType() == pType)
                    {
                        FPublishers.Remove(fPublisher);
                        MiscHelper.DisposeObject(fPublisher.FPublisher);
                        break;
                    }
                }
            }
        }

        public void RemovePublisherByName(string name)
        {
            lock (FPublishers)
            {
                foreach (PublisherInfo fPublisher in FPublishers)
                {
                    if (string.Compare(fPublisher.FClassInfo.Name, name, ignoreCase: false) == 0)
                    {
                        FPublishers.Remove(fPublisher);
                        MiscHelper.DisposeObject(fPublisher.FPublisher);
                        break;
                    }
                }
            }
        }

        public void AddPublisher(IExceptionPublisher publisher, NameValueCollection parameters)
        {
            lock (FPublishers)
            {
                ClassInfo classInfo = new ClassInfo(publisher.GetType().FullName, parameters);
                publisher.Initialize(classInfo.Name, classInfo.Parameters, classInfo);
                PublisherInfo publisherInfo = new PublisherInfo();
                publisherInfo.FClassInfo = classInfo;
                publisherInfo.FPublisher = publisher;
                PublisherInfo item = publisherInfo;
                FPublishers.Add(item);
            }
        }

        public void AddPublisher(IExceptionPublisher publisher)
        {
            AddPublisher(publisher, new NameValueCollection());
        }
    }
}