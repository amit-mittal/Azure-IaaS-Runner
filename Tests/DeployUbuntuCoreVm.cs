using System;
using System.IO;
using AzureRunner.Workflows;

namespace AzureRunner.Tests
{
    public class DeployUbuntuCoreVm : BaseLinuxLifecycle
    {
        public DeployUbuntuCoreVm(string region) : base(region)
        {
            string guid = Guid.NewGuid().ToString().Substring(0, 5);

            groupName = "IsoRunnerRG-UbuntuCore-" + guid;
            deploymentName = "RunnerDeployment-UbuntuCore-" + guid;
            template = File.ReadAllText(@"ArmTemplates\linux.noplan.json");
            templateParameters = File.ReadAllText(@"ArmParameters\linux.distros.ubuntucore.parameters.json");
            username = "<USER>";
            password = "<PASSWORD>";
        }
    }
}
