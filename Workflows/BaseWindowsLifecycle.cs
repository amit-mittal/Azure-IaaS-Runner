using System;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Management.Automation.Runspaces;
using System.Security;
using AzureRunner.Helpers;
using AzureRunner.Tests;
using Microsoft.Azure.Management.Network.Models;
using Microsoft.Rest;

namespace AzureRunner.Workflows
{
    public abstract class BaseWindowsLifecycle : IaasBasicLifecycle
    {
        protected string customData;

        public BaseWindowsLifecycle(string region) : base(region)
        {
        }

        public override bool Validate()
        {
            // TODO failures in create and cleanup should be ignored for couple of times
            // TODO Add retries in validation logic
            // TODO talk to guest agent and gets it version and validate if latest
            object t = Impersonation.Impersonate(ConfigurationManager.UserName, ConfigurationManager.Password);

            try
            {
                var token = AzureHelper.GetAccessTokenAsync();
                var credential = new TokenCredentials(token.Result.AccessToken);

                PublicIPAddress ipAddress = AzureHelper.GetPublicAddressAsync(credential, groupName, subscriptionId, "myPublicIP").Result;

                // TODO: make username-password at one place instead of at both ARM and here
                SecureString securePwd = new SecureString();
                password.ToCharArray().ToList().ForEach(p => securePwd.AppendChar(p));

                // this is the entrypoint to interact with the system (interfaced for testing).
                var remoteComputer = new Uri(string.Format("{0}://{1}:5986", "https", ipAddress.IpAddress));

                var connection = new WSManConnectionInfo(remoteComputer, String.Empty, new PSCredential(username, securePwd));

                var option = new PSSessionOption();
                option.SkipCACheck = true;
                option.SkipCNCheck = true;
                option.SkipRevocationCheck = true;
                connection.SetSessionOptions(option);

                connection.AuthenticationMechanism = AuthenticationMechanism.Negotiate;

                // TODO What if powershell session gets stuck in between

                var runspace = RunspaceFactory.CreateRunspace(connection);
                runspace.Open();

                var powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                powershell.AddScript("get-psdrive –psprovider filesystem");
                var results = powershell.Invoke();
                foreach (var output in results.Where(o => o != null))
                {

                }

                bool ifCustomData = true;
                if (!string.IsNullOrWhiteSpace(customData))
                {
                    powershell.AddScript("Get-Content C:\\AzureData\\CustomData.bin -Encoding UTF8");
                    results = powershell.Invoke();
                    ifCustomData = (results.Where(o => o != null).ToList().Count == 1);
                    foreach (var output in results.Where(o => o != null))
                    {
                        logger.Info(string.Format("Expected: {0} Actual: {1}", customData, output));
                        ifCustomData &= (output.ToString() == customData);
                    }
                }

                runspace.Close();

                if (!ifCustomData)
                {
                    throw new ArgumentException("Incorrect custom data!!");
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                return false;
            }
            finally
            {
                Impersonation.UndoImpersonation(t);
            }

            return true;
        }
    }
}
