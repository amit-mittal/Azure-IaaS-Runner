using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Threading.Tasks;
using AzureRunner.Helpers;
using Microsoft.Rest;

namespace AzureRunner.Tests
{
    public class TestSetup
    {
        public static void AddUser()
        {
            try
            {
                using (PowerShell powerShellInstance = PowerShell.Create())
                {
                    string scriptBlock = string.Format("net user /add {0} {1};net localgroup Administrators {0} /add",
                        ConfigurationManager.UserName,
                        ConfigurationManager.Password);

                    // use "AddScript" to add the contents of a script file to the end of the execution pipeline.
                    // use "AddCommand" to add individual commands/cmdlets to the end of the execution pipeline.
                    powerShellInstance.AddScript(scriptBlock);

                    Collection<PSObject> psOutput = powerShellInstance.Invoke();

                    if (powerShellInstance.Streams.Error.Count > 0)
                    {
                        throw new ApplicationException("Failure found while adding the user!!");
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public static void CleanupLeakedResourceGroups()
        {
            try
            {
                var token = AzureHelper.GetAccessTokenAsync();
                var credential = new TokenCredentials(token.Result.AccessToken);

                List<string> resourceGroups = AzureHelper.GetResourceGroups(credential,
                    ConfigurationManager.SubscriptionId,
                    ConfigurationManager.Locations);

                foreach (string resourceGroup in resourceGroups)
                {
                    Task t = AzureHelper.DeleteResourceGroupAsync(credential,
                        resourceGroup,
                        ConfigurationManager.SubscriptionId);
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
