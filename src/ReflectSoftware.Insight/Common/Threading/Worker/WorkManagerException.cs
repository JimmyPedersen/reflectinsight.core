using System;
using System.Runtime.InteropServices;

namespace RI.Threading.Worker
{
    [Serializable]
    [ComVisible(false)]
    public class WorkManagerException : ApplicationException
    {
        public WorkManagerException(string msg)
            : base(msg)
        {
        }

        public WorkManagerException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }
    }
}