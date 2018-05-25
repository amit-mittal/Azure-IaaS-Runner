using System;
using AzureRunner.Helpers;
using Microsoft.Azure.Management.Network.Models;
using Microsoft.Rest;
using Renci.SshNet;

namespace AzureRunner.Workflows
{
    public abstract class BaseLinuxLifecycle : IaasBasicLifecycle
    {
        protected string customData;

        public BaseLinuxLifecycle(string region) : base(region)
        {
        }

        public override bool Validate()
        {
            // TODO add these under a timeout
            try
            {
                // TODO Include test name in the logs
                logger.Info("Validations for Linux in progress...");

                var token = AzureHelper.GetAccessTokenAsync();
                var credential = new TokenCredentials(token.Result.AccessToken);
                bool ifCustomDataValid = true;

                PublicIPAddress ipAddress = AzureHelper.GetPublicAddressAsync(credential, groupName, subscriptionId, "myPublicIP").Result;

                using (var client = new SshClient(ipAddress.IpAddress, username, password))
                {
                    client.Connect();

                    SshCommand command = client.RunCommand("echo 'hello'");
                    logger.Info("Result of command 'x': " + command.Result);

                    if (!string.IsNullOrWhiteSpace(customData))
                    {
                        command = client.RunCommand("echo '<PASSWORD>' | sudo -S cat /var/lib/waagent/CustomData");
                        ifCustomDataValid = command.Result.Contains(customData);
                    }

                    client.Disconnect();
                }

                if (!ifCustomDataValid)
                {
                    throw new ArgumentException("Incorrect custom data!!");
                }

                logger.Info("Validations for Linux...success!!");
            }
            catch (Exception e)
            {
                logger.Error(e);
                return false;
            }

            return true;
        }
    }
}
