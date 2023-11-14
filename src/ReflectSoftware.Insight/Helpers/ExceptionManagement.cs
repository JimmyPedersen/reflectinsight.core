#region Assembly ReflectSoftware.Insight, Version=5.7.1.1706, Culture=neutral, PublicKeyToken=c78ddbdaf1f32b08
// C:\Work\Medexa\MSWC_v5\500-Software_PC\packages\ReflectSoftware.Insight.5.7.1.1\lib\net45\ReflectSoftware.Insight.dll
// Decompiled with ICSharpCode.Decompiler 7.1.0.6543
#endregion

using RI.Utils.ClassFactory;
using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace RI.Utils.ExceptionManagement
{
    [ComVisible(false)]
    public interface IExceptionPublisher : IClassFactoryInitialization
    {
        void Publish(Exception ex, NameValueCollection additionalParameters);
    }

    [ComVisible(false)]
    public static class ExceptionManager
    {
        private static readonly object LockObject;

        private static ExceptionManagerBase FExceptionManagerComposite;

        public static int PublisherCount => FExceptionManagerComposite.PublisherCount;

        public static PublisherInfo[] PublisherInfos => FExceptionManagerComposite.PublisherInfos;

        public static IExceptionPublisher[] Publishers => FExceptionManagerComposite.Publishers;

        public static event Action OnConfigChange;

        static ExceptionManager()
        {
            LockObject = new object();
            FExceptionManagerComposite = new ExceptionManagerBase("./configuration/rsSettings/exceptionManagement", "RS.Utils.ExceptionManager", bTrackConfigChange: true, bInitLoadPublishers: true);
            FExceptionManagerComposite.OnConfigChange = OnConfigFileChanged;
        }

        public static void OnStartup()
        {
        }

        public static void OnShutdown()
        {
            lock (LockObject)
            {
                if (FExceptionManagerComposite != null)
                {
                    FExceptionManagerComposite.Dispose();
                    FExceptionManagerComposite = null;
                }
            }
        }

        private static void OnConfigFileChanged()
        {
            if (ExceptionManager.OnConfigChange != null)
            {
                ExceptionManager.OnConfigChange();
            }
        }

        public static void Publish(Exception ex, NameValueCollection additionalParameters)
        {
            FExceptionManagerComposite.Publish(ex, additionalParameters);
        }

        public static void Publish(Exception ex)
        {
            FExceptionManagerComposite.Publish(ex);
        }

        public static void RemovePublisherByType(Type pType)
        {
            FExceptionManagerComposite.RemovePublisherByType(pType);
        }

        public static void RemovePublisherByName(string name)
        {
            FExceptionManagerComposite.RemovePublisherByName(name);
        }

        public static void AddPublisher(IExceptionPublisher publisher, NameValueCollection parameters)
        {
            FExceptionManagerComposite.AddPublisher(publisher, parameters);
        }

        public static void AddPublisher(IExceptionPublisher publisher)
        {
            FExceptionManagerComposite.AddPublisher(publisher, new NameValueCollection());
        }
    }