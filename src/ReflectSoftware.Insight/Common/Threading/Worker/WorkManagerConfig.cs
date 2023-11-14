
using Plato.Configuration;
using System;
using System.Runtime.InteropServices;

namespace RI.Threading.Worker
{
    [ComVisible(false)]
    public static class WorkManagerConfig
    {
        private static readonly ConfigContainer FConfigCon;

        public static int WaitOnTerminateThread => int.Parse(FConfigCon.Node.GetAttribute("./workManager", "waitOnTerminateThread", "10000"));

        public static int EventTracker => int.Parse(FConfigCon.Node.GetAttribute("./workManager", "eventTracker", "20"));

        public static string ApplicationName => FConfigCon.Node.GetAttribute("./workManager", "applicationName", "RI.Threading.WorkManager");

        public static ConfigContainer Container => FConfigCon;

        public static event Action OnConfigChange;

        static WorkManagerConfig()
        {
            FConfigCon = ConfigManager.GetConfigurationBySection("./configuration/rsSettings");
            FConfigCon.WatchAllowOnChangeAttribute(".", "reactOnConfigChange", "true");
            FConfigCon.OnConfigChange += DoConfigFileChanged;
        }

        private static void DoConfigFileChanged()
        {
            if (WorkManagerConfig.OnConfigChange != null)
            {
                WorkManagerConfig.OnConfigChange();
            }
        }
    }
}