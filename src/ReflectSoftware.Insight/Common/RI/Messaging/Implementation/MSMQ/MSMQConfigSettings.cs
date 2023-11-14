using System;
using System.Configuration;

namespace ReflectSoftware.Insight.Common.RI.Messaging.Implementation.MSMQ
{
    [ConfigurationCollection(typeof(MSMQConfigSetting))]
    public class MSMQConfigSettings : ConfigurationElementCollection
    {
        private const string PropertyName = "setting";

        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMapAlternate;

        protected override string ElementName => "setting";

        public MSMQConfigSetting this[int idx] => (MSMQConfigSetting)BaseGet(idx);

        public new MSMQConfigSetting this[string name] => (MSMQConfigSetting)BaseGet(name);

        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals("setting", StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MSMQConfigSetting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MSMQConfigSetting)element).Name;
        }
    }
}
