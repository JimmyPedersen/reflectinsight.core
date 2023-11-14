using ReflectSoftware.Insight;
using System;

namespace TestHarness.Playgrounds
{
    public static class ConfigurationPlayground
    {
        static public void Run()
        {
            var ri = RILogManager.Get("Test");

            Console.WriteLine($"Active group:{RIListenerGroupManager.ActiveGroup.Name}");
            /*
                        var useGroup = RIListenerGroupManager.Get("DevelopmentGroup");
                        RIListenerGroupManager.ActiveGroup = useGroup;
                        Console.WriteLine($"New Active group:{RIListenerGroupManager.ActiveGroup.Name}");
            */

            ri.EnterMethod("MyEnter");
            ri.SendMessage("Test1");
            ri.SendMessage("Test2");
            ri.ExitMethod("MyEnter");
        }
    }
}
