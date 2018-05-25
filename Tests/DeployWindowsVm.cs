using System;
using System.IO;
using AzureRunner.Workflows;

namespace AzureRunner.Tests
{
    class DeployWindowsVm : BaseWindowsLifecycle
    {
        public DeployWindowsVm(string region) : base(region)
        {
            string guid = Guid.NewGuid().ToString().Substring(0, 5);

            groupName = "IsoRunnerRG-Win-" + guid;
            deploymentName = "RunnerDeployment-Win-" + guid;
            template = File.ReadAllText(@"ArmTemplates\windows.winrm.json");
            templateParameters = File.ReadAllText(@"ArmParameters\windows.winrm.parameters.json");
            username = "<USER>";
            password = "<PASSWORD>";
        }
    }
}
