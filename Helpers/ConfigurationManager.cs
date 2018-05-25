using System.Collections.Generic;

namespace AzureRunner.Helpers
{
    public class ConfigurationManager
    {
        public static string UserName = "<USER-NAME>";

        public static string Password = "<PASSWORD>";

        public static string ResourceGroupsPrefix = "IsoRunnerRG-";

        public static string SubscriptionId = "<Azure-Subscription-Id>";

        public static List<string> Locations = new List<string>()
        {
            "westcentralus",
            "japaneast",
            "brazilsouth",
            "westindia",
            "koreasouth"
        };
    }
}
