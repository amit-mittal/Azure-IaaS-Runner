using System;
using System.IO;
using AzureRunner.Workflows;

namespace AzureRunner.Tests
{
    public class DeployRedHatVm : BaseLinuxLifecycle
    {
        public DeployRedHatVm(string region) : base(region)
        {
            string guid = Guid.NewGuid().ToString().Substring(0, 5);

            groupName = "IsoRunnerRG-RedHat-" + guid;
            deploymentName = "RunnerDeployment-RedHat-" + guid;
            template = File.ReadAllText(@"ArmTemplates\linux.customdata.noplan.json");
            templateParameters = File.ReadAllText(@"ArmParameters\linux.distros.redhat.parameters.json");
            username = "<USER>";
            password = "<PASSWORD>";
            customData = "YXZyLc6Izq0tMXMzNDLOiM6t";
        }
    }
}
