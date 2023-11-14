using System;
using System.Configuration;

namespace ReflectSoftware.Insight.Common.RI.Messaging.Implementation.MSMQ
{
    public class MSMQConfigSetting : ConfigurationElement, ICloneable
    {
        private const string NameProperty = "name";

        private const string QueuePathProperty = "queuePath";

        private const string TimeToReceiveProperty = "timeToReceive";

        private const string TimeToReachProperty = "timeToReach";

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return base["name"] as string;
            }
            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("queuePath", IsRequired = true)]
        public string QueuePath
        {
            get
            {
                return base["queuePath"] as string;
            }
            set
            {
                base["queuePath"] = value;
            }
        }

        [ConfigurationProperty("timeToReceive", IsRequired = false)]
        public string TimeToReceive
        {
            get
            {
                return base["timeToReceive"] as string;
            }
            set
            {
                base["timeToReceive"] = value;
            }
        }

        [ConfigurationProperty("timeToReach", IsRequired = false)]
        public string TimeToReach
        {
            get
            {
                return base["timeToReach"] as string;
            }
            set
            {
                base["timeToReach"] = value;
            }
        }

        public object Clone()
        {
            MSMQConfigSetting mSMQConfigSetting = new MSMQConfigSetting();
            mSMQConfigSetting.Name = Name;
            mSMQConfigSetting.QueuePath = QueuePath;
            mSMQConfigSetting.TimeToReceive = TimeToReceive;
            mSMQConfigSetting.TimeToReach = TimeToReach;
            return mSMQConfigSetting;
        }
    }
}
