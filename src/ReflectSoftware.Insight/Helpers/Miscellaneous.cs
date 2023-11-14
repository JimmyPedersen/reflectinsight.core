using RI.Utils.Strings;
using System;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace RI.Utils.Miscellaneous
{
    public enum ResourceLockType
    {
        Local,
        Global
    }

    public class ResourceLock : IDisposable
    {
        private static readonly MutexSecurity FFileMutexSec;

        private Mutex FMutex;

        public bool Disposed { get; private set; }

        static ResourceLock()
        {
            FFileMutexSec = new MutexSecurity();
            FFileMutexSec.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow));
        }

        public ResourceLock(string name, ResourceLockType rType)
        {
            Disposed = false;
            string arg = ((rType == ResourceLockType.Local) ? "Local" : "Global");
            FMutex = new Mutex(false, $"{arg}\\RS.GlobalResourceLock.Resource:{(int)StringHash.BKDRHash(name.ToLower())}", out var _);//, FFileMutexSec); TODO: How does this affect the code?
        }

        public ResourceLock(string name)
            : this(name, ResourceLockType.Global)
        {
        }

        public void Lock()
        {
            if (FMutex != null)
            {
                FMutex.WaitOne();
            }
        }

        public void Unlock()
        {
            if (FMutex != null)
            {
                FMutex.ReleaseMutex();
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                if (!Disposed)
                {
                    Disposed = true;
                    GC.SuppressFinalize(this);
                    if (FMutex != null)
                    {
                        FMutex.Close();
                        FMutex.Dispose();
                        FMutex = null;
                    }
                }
            }
        }
    }

    public class ShortTermResourceLock : IDisposable
    {
        private ResourceLock FResourceLock;

        public bool Disposed { get; private set; }

        public ShortTermResourceLock(string name, ResourceLockType rType)
        {
            Disposed = false;
            FResourceLock = new ResourceLock(name, rType);
            FResourceLock.Lock();
        }

        public ShortTermResourceLock(string name)
            : this(name, ResourceLockType.Global)
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
                    if (FResourceLock != null)
                    {
                        FResourceLock.Unlock();
                        FResourceLock.Dispose();
                        FResourceLock = null;
                    }
                }
            }
        }
    }



    public static class MiscHelper
    {
        public static bool IsStandardType(Type typ)
        {
            if (typ == typeof(bool))
            {
                return true;
            }

            if (typ == typeof(byte))
            {
                return true;
            }

            if (typ == typeof(sbyte))
            {
                return true;
            }

            if (typ == typeof(char))
            {
                return true;
            }

            if (typ == typeof(decimal))
            {
                return true;
            }

            if (typ == typeof(double))
            {
                return true;
            }

            if (typ == typeof(float))
            {
                return true;
            }

            if (typ == typeof(int))
            {
                return true;
            }

            if (typ == typeof(uint))
            {
                return true;
            }

            if (typ == typeof(long))
            {
                return true;
            }

            if (typ == typeof(ulong))
            {
                return true;
            }

            if (typ == typeof(short))
            {
                return true;
            }

            if (typ == typeof(ushort))
            {
                return true;
            }

            if (typ == typeof(string))
            {
                return true;
            }

            if (typ == typeof(StringBuilder))
            {
                return true;
            }

            if (typ == typeof(DateTime))
            {
                return true;
            }

            if (typ.IsEnum)
            {
                return true;
            }

            return false;
        }

        public static bool IsStandardType(object obj)
        {
            return IsStandardType(obj.GetType());
        }

        public static void DisposeObject(object obj)
        {
            (obj as IDisposable)?.Dispose();
        }

        public static byte[] SerializeObjectToArray(object obj)
        {
            using MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            xmlSerializer.Serialize(memoryStream, obj);
            return memoryStream.ToArray();
        }

        public static T DeserializeArrayToObject<T>(byte[] bObj)
        {
            using MemoryStream stream = new MemoryStream(bObj);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(stream);
        }

        public static string SerializeObjectTo64BaseString(object obj)
        {
            return Convert.ToBase64String(SerializeObjectToArray(obj));
        }

        public static T Deserialize64BaseStringToObject<T>(string sObj)
        {
            return DeserializeArrayToObject<T>(Convert.FromBase64String(sObj));
        }

        public static T CreateInstance<T>()
        {
            return (T)Activator.CreateInstance(typeof(T), nonPublic: true);
        }

        public static T CreateInstance<T>(string objectType)
        {
            try
            {
                Type type = Type.GetType(objectType, throwOnError: false);
                if (type == null)
                {
                    return default(T);
                }

                return (T)Activator.CreateInstance(type, nonPublic: false);
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
        }

        public static void DelayAndProcessEvents(double mSeconds, int sleepInterval)
        {
            DateTime now = DateTime.Now;
            while (DateTime.Now.Subtract(now).TotalMilliseconds < mSeconds)
            {
                // FIX: What do we do here instead? If anything
                //                Application.DoEvents();
                Thread.Sleep(sleepInterval);
            }
        }

        public static void DelayAndProcessEvents(double mSeconds)
        {
            DelayAndProcessEvents(mSeconds, 10);
        }

        public static void DelayAndProcessEvents(TimeSpan mTimeSpan)
        {
            DelayAndProcessEvents(mTimeSpan.TotalMilliseconds);
        }

        public static void PassiveSleep(long mSec, ref bool terminate)
        {
            long num = mSec;
            while (num > 0 && !terminate)
            {
                Thread.Sleep(100);
                num -= 100;
            }
        }

        public static string GetTimeInUTCFormat(DateTime dt)
        {
            return dt.ToString("yyyy-MM-ddTHH:mm:ss+0000", CultureInfo.InvariantCulture);
        }

        public static string GetCurrentTimeInUTCFormat()
        {
            return GetTimeInUTCFormat(DateTime.Now.ToUniversalTime());
        }
        /*
                public static bool IsCurrentDomainWebApplication()
                {
                    return HttpContext.get_Current() != null;
                }
        */
        public static void GCCollect()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        public static string ShortEncode(Guid guid)
        {
            string text = Convert.ToBase64String(guid.ToByteArray());
            text = text.Replace("/", "_");
            text = text.Replace("+", "-");
            return text.Substring(0, 22);
        }

        public static string DetermineParameterPath(string path)
        {
            if (StringHelper.IsNullOrEmpty(path))
            {
                return path;
            }

            path = path.Replace("$(workingdir)", AppDomain.CurrentDomain.BaseDirectory);
            path = path.Replace("$(mydocuments)", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            if (path.Length >= 2 && string.Compare(path.Substring(0, 2), "\\\\", ignoreCase: false, CultureInfo.InvariantCulture) == 0)
            {
                path = string.Format(CultureInfo.InvariantCulture, "[&&]{0}", new object[1] { path.Substring(2, path.Length - 2) });
                path = path.Replace("\\\\", "\\");
            }

            path = path.Replace("[&&]", "\\\\");
            try
            {
                path = Path.GetFullPath(Environment.ExpandEnvironmentVariables(path));
                return path;
            }
            catch
            {
                return path;
            }
        }
    }
}