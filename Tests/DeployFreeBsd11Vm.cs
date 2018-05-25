using System;
using System.IO;
using AzureRunner.Workflows;

namespace AzureRunner.Tests
{
    public class DeployFreeBsd11Vm : BaseLinuxLifecycle
    {
        public DeployFreeBsd11Vm(string region) : base(region)
        {
            string guid = Guid.NewGuid().ToString().Substring(0, 5);

            groupName = "IsoRunnerRG-BSD11-" + guid;
            deploymentName = "RunnerDeployment-BSD11-" + guid;
            template = File.ReadAllText(@"ArmTemplates\linux.noplan.json");
            templateParameters = File.ReadAllText(@"ArmParameters\linux.distros.freebsd11.parameters.json");
            username = "<USER>";
            password = "<PASSWORD>";
        }
    }
}
