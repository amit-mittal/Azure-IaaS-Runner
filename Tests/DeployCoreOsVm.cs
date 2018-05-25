using System;
using System.IO;
using AzureRunner.Workflows;

namespace AzureRunner.Tests
{
    public class DeployCoreOsVm : BaseLinuxLifecycle
    {
        public DeployCoreOsVm(string region) : base(region)
        {
            string guid = Guid.NewGuid().ToString().Substring(0, 5);

            groupName = "IsoRunnerRG-Core-" + guid;
            deploymentName = "RunnerDeployment-Core-" + guid;
            template = File.ReadAllText(@"ArmTemplates\linux.noplan.json");
            templateParameters = File.ReadAllText(@"ArmParameters\linux.distros.coreos.parameters.json");
            username = "<USER>";
            password = "<PASSWORD>";
        }
    }
}
