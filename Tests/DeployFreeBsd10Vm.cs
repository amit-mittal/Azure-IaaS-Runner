using System;
using System.IO;
using AzureRunner.Workflows;

namespace AzureRunner.Tests
{
    public class DeployFreeBsd10Vm : BaseLinuxLifecycle
    {
        public DeployFreeBsd10Vm(string region) : base(region)
        {
            string guid = Guid.NewGuid().ToString().Substring(0, 5);

            groupName = "IsoRunnerRG-BSD10-" + guid;
            deploymentName = "RunnerDeployment-BSD10-" + guid;
            template = File.ReadAllText(@"ArmTemplates\linux.noplan.json");
            templateParameters = File.ReadAllText(@"ArmParameters\linux.distros.freebsd10.parameters.json");
            username = "<USER>";
            password = "<PASSWORD>";
        }
    }
}
