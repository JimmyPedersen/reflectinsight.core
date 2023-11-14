using System.Configuration;

namespace ReflectSoftware.Insight.Common.RI.Messaging.Implementation.MSMQ
{
    public class MSMQConfiguration : ConfigurationSection
    {
        private const string ConfigurationCollectionProperty = "settings";

        private const string ConfigurationSection = "msmqConfiguration";

        [ConfigurationProperty("settings")]
        protected MSMQConfigSettings MSMQConfigurationElement => (MSMQConfigSettings)base["settings"];

        public static MSMQConfigSetting GetSetting(string name)
        {
            MSMQConfiguration mSMQConfiguration = (MSMQConfiguration)ConfigurationManager.GetSection("msmqConfiguration");
            return mSMQConfiguration.MSMQConfigurationElement[name];
        }
    }
}
