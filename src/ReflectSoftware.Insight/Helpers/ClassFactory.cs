using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace RI.Utils.ClassFactory
{
    [Serializable]
    [ComVisible(false)]
    public class ClassInfo
    {
        public string Name;

        public NameValueCollection Parameters;

        public ClassInfo(string name, NameValueCollection parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }

    [ComVisible(false)]
    public interface IClassFactoryInitialization
    {
        void Initialize(string name, NameValueCollection parameters, params object[] args);
    }
}