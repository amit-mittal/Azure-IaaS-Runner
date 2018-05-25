using System;
using System.IO;
using AzureRunner.Workflows;

namespace AzureRunner.Tests
{
    public class DeployUbuntuVm : BaseLinuxLifecycle
    {
        public DeployUbuntuVm(string region) : base(region)
        {
            string guid = Guid.NewGuid().ToString().Substring(0, 5);

            groupName = "IsoRunnerRG-Ubuntu-" + guid;
            deploymentName = "RunnerDeployment-Ubuntu-" + guid;
            template = File.ReadAllText(@"ArmTemplates\linux.ssh.json");
            templateParameters = File.ReadAllText(@"ArmParameters\linux.ssh.parameters.json");
            username = "<USER>";
            password = "<PASSWORD>";
        }
    }
}
