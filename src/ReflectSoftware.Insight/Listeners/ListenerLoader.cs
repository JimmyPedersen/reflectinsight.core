using ReflectSoftware.Insight.Common;
using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ReflectSoftware.Insight
{
	static internal class ListenerLoader
	{
        private readonly static Hashtable FListenerTypeList;        
        private readonly static Hashtable FAssemblies;

		static ListenerLoader()
		{
            FAssemblies = new Hashtable();

            FListenerTypeList = new Hashtable
            {                
                ["BinaryFile"] = "ReflectSoftware.Insight.ListenerBinaryFile, ReflectSoftware.Insight",
                ["TextFile"] = "ReflectSoftware.Insight.ListenerTextFile, ReflectSoftware.Insight",
                ["Console"] = "ReflectSoftware.Insight.ListenerConsole, ReflectSoftware.Insight",
                ["Router"] = "ReflectSoftware.Insight.ListenerRouter, ReflectSoftware.Insight"
            };
        }

        private static Object LoadObject(String typeString)
        {
            if (string.IsNullOrWhiteSpace(typeString))
            {
                return null;
            }

            var regSeparate = new Regex(@",\s*");
            var sList = regSeparate.Split(typeString);

            if (sList.Length != 2)
            {                
                throw new ReflectInsightException(String.Format("Invalid Object type setting '{0}'.", typeString));
            }

            var rValue = (object)null;
            var objectType = sList[0].Trim();
            var objectAssembly = sList[1].Trim();

            Assembly oAssembly = (Assembly)FAssemblies[objectAssembly];
            if (oAssembly == null)
            { 
                oAssembly = AppDomain.CurrentDomain.Load(objectAssembly);
                FAssemblies[objectAssembly] = oAssembly;
            }

            var oType = oAssembly.GetType(objectType, false, true);
            if (oType != null)
            {
                rValue = Activator.CreateInstance(oType, true);
            }

            return rValue;
        }

        public static IReflectInsightListener Get(String listenerName)
        {
            lock (FListenerTypeList)
            {
                String typeString = ReflectInsightConfig.Settings.GetListenerType(listenerName);
                if (typeString == null)
                {
                    // Not defined in configuration.
                    // Lets search the default list
                    typeString = (String)FListenerTypeList[listenerName];

                    if (typeString == null)
                    {
                        throw new ReflectInsightException(String.Format("Cannot find Listener definition for: '{0}'", listenerName));
                    }
                }

                var rValue = (IReflectInsightListener)LoadObject(typeString);
                if (rValue == null)
                {
                    throw new ReflectInsightException(String.Format("Cannot find or load Listener '{0}' for type: {1}", listenerName, typeString));
                }

                return rValue;
            }
        }
	}
}