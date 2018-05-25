using System;
using System.IO;
using AzureRunner.Workflows;

namespace AzureRunner.Tests
{
    public class DeployCheckpointVm : BaseLinuxLifecycle
    {
        public DeployCheckpointVm(string region) : base(region)
        {
            string guid = Guid.NewGuid().ToString().Substring(0, 5);

            groupName = "IsoRunnerRG-CP-" + guid;
            deploymentName = "RunnerDeployment-CP-" + guid;
            template = File.ReadAllText(@"ArmTemplates\linux.distros.json");
            templateParameters = File.ReadAllText(@"ArmParameters\linux.distros.checkpoint.parameters.json");
            username = "admin";
            password = "<PASSWORD>";
        }
    }
}
