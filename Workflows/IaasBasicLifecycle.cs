using System;
using System.Threading.Tasks;
using AzureRunner.Helpers;
using AzureRunner.Tests;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest;

namespace AzureRunner.Workflows
{
    public abstract class IaasBasicLifecycle : BaseTest
    {
        protected LogHelper logger;

        protected string groupName;

        protected string subscriptionId;

        protected string deploymentName;

        protected string location;

        protected string template;

        protected string templateParameters;

        protected string username;

        protected string password;

        protected IaasBasicLifecycle(string region)
        {
            this.logger = new LogHelper(Main.Mode);

            this.subscriptionId = ConfigurationManager.SubscriptionId;
            this.location = region;
        }

        public bool Create()
        {
            logger.Info("Doing the work to deploy IaaS VM");

            var token = AzureHelper.GetAccessTokenAsync();
            var credential = new TokenCredentials(token.Result.AccessToken);

            logger.Info("Success in getting token for deployment creation!");

            try
            {
                logger.Info("Creating the resource group...");
                var rgResult = AzureHelper.CreateResourceGroupAsync(
                    credential,
                    groupName,
                    subscriptionId,
                    location);
                logger.Info("Success in generating RG. ProvisioningState: " + rgResult.Result.Properties.ProvisioningState);

                Console.WriteLine("Creating the template deployment...");
                Task<DeploymentExtended> dpResult = AzureHelper.CreateTemplateDeploymentAsync(
                    credential,
                    groupName,
                    deploymentName,
                    subscriptionId,
                    template,
                    templateParameters);
                logger.Info("Successfully created deployment. ProvisioningState: " + dpResult.Result.Properties.ProvisioningState);
            }
            catch (Exception e)
            {
                logger.Error("During deployment, exception: " + e);
                return false;
            }

            return true;
        }

        public bool CleanupAsync()
        {
            var token = AzureHelper.GetAccessTokenAsync();
            var credential = new TokenCredentials(token.Result.AccessToken);

            logger.Info("Success in getting token for cleanup-async!");

            logger.Info("Deleting deployment...");
            Task t = AzureHelper.DeleteResourceGroupAsync(
              credential,
              groupName,
              subscriptionId);

            logger.Info("Deletion of resource group triggered!!");

            return true;
        }

        public bool Cleanup()
        {
            var token = AzureHelper.GetAccessTokenAsync();
            var credential = new TokenCredentials(token.Result.AccessToken);

            logger.Info("Success in getting token for cleanup!");

            logger.Info("Deleting deployment...");
            Task t = AzureHelper.DeleteResourceGroupAsync(
              credential,
              groupName,
              subscriptionId);
            t.Wait(TimeSpan.FromMinutes(15));

            logger.Info("Success in deleting deployment!!");

            return true;
        }

        public override bool Execute()
        {
            bool create = true;
            bool validate = true;

            logger.Info("Lifecycle state: Precleanup");
            try
            {
                Cleanup();
            }
            catch (Exception e)
            {
                logger.Error("Precleanup Error: " + e.Message);
            }

            logger.Info("Moving to Lifecycle state: Create");

            create = Create();
            if (create)
            {
                logger.Info("Moving to Lifecycle state: Validate");

                for (int i = 0; i < 3; i++)
                {
                    validate = Validate();
                    if (validate)
                    {
                        logger.Info("Try #" + i + ": Lifecycle state: Validate PASSED!");
                        break;
                    }

                    logger.Info("Try #" + i + ": Lifecycle state: Validate FAILED!!");
                }

                if (!validate)
                {
                    // Cleanup of ResourceGroup will still happen
                    logger.Error("Lifecycle state: Validate FAILED!!");
                }
            }

            logger.Info("Moving to Lifecycle state: CleanupAsync");
            CleanupAsync();

            return (create && validate);
        }

        public virtual bool Validate()
        {
            throw new NotImplementedException();
        }
    }
}
