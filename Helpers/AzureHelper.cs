using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Network;
using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure;

namespace AzureRunner.Helpers
{
    public class AzureHelper
    {
        public static async Task<DeploymentExtended> CreateTemplateDeploymentAsync(
           TokenCredentials credential,
           string groupName,
           string deploymentName,
           string subscriptionId,
           object template,
           object parameters)
        {
            var resourceManagementClient = new ResourceManagementClient(credential)
            { SubscriptionId = subscriptionId };

            var deployment = new Deployment();
            deployment.Properties = new DeploymentProperties
            {
                Mode = DeploymentMode.Incremental,
                Template = template,
                Parameters = parameters
            };

            return await resourceManagementClient.Deployments.CreateOrUpdateAsync(
              groupName,
              deploymentName,
              deployment
            );
        }

        public static async Task<ResourceGroup> CreateResourceGroupAsync(
            TokenCredentials credential,
            string groupName,
            string subscriptionId,
            string location)
        {
            var resourceManagementClient = new ResourceManagementClient(credential)
            { SubscriptionId = subscriptionId };

            var resourceGroup = new ResourceGroup { Location = location };
            return await resourceManagementClient.ResourceGroups.CreateOrUpdateAsync(groupName, resourceGroup);
        }

        public static async Task DeleteResourceGroupAsync(
              TokenCredentials credential,
              string groupName,
              string subscriptionId)
        {
            var resourceManagementClient = new ResourceManagementClient(credential)
            { SubscriptionId = subscriptionId };
            await resourceManagementClient.ResourceGroups.DeleteAsync(groupName);
        }

        public static async Task<PublicIPAddress> GetPublicAddressAsync(
            TokenCredentials credential,
            string groupName,
            string subscriptionId,
            string publicIpName)
        {
            var client = new NetworkManagementClient(credential) {SubscriptionId = subscriptionId};
            return await PublicIPAddressesOperationsExtensions.GetAsync(client.PublicIPAddresses, groupName, publicIpName);
        }

        public static List<string> GetResourceGroups(
            TokenCredentials credential,
            string subscriptionId,
            List<string> locations)
        {
            var resourceGroups = new List<string>();

            var client = new ResourceManagementClient(credential) { SubscriptionId = subscriptionId };
            AzureOperationResponse<IPage<ResourceGroup>> result = client.ResourceGroups.ListWithHttpMessagesAsync().Result;
            var body = result.Body as Microsoft.Azure.Management.ResourceManager.Models.Page<ResourceGroup>;

            if (body != null)
            {
                IEnumerator<ResourceGroup> enumerator = body.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ResourceGroup item = enumerator.Current;
                    if (item.Name.StartsWith(ConfigurationManager.ResourceGroupsPrefix, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (locations.Count == 0)
                        {
                            resourceGroups.Add(item.Name);
                        }
                        else
                        {
                            resourceGroups.AddRange(
                                from location in locations
                                where item.Location.Equals(location, StringComparison.InvariantCultureIgnoreCase)
                                select item.Name);
                        }
                    }
                }
            }

            return resourceGroups;
        }

        public static async Task<AuthenticationResult> GetAccessTokenAsync()
        {
            var cc = new ClientCredential("<app-id>", "<app-secret>");
            var context = new AuthenticationContext("<AAD-Auth>");
            var token = await context.AcquireTokenAsync("https://management.azure.com/", cc);
            if (token == null)
            {
                throw new InvalidOperationException("Could not get the token.");
            }

            return token;
        }
    }
}
