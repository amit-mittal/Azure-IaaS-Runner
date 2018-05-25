using System;
using System.IO;
using AzureRunner.Workflows;

namespace AzureRunner.Tests
{
    public class DeployCentOsVm : BaseLinuxLifecycle
    {
        public DeployCentOsVm(string region) : base(region)
        {
            string guid = Guid.NewGuid().ToString().Substring(0, 5);

            groupName =  "IsoRunnerRG-Cent-" + guid;
            deploymentName = "RunnerDeployment-Cent-" + guid;
            template = File.ReadAllText(@"ArmTemplates\linux.noplan.json");
            templateParameters = File.ReadAllText(@"ArmParameters\linux.distros.centos.parameters.json");
            username = "<USER>";
            password = "<PASSWORD>";
        }
    }
}
