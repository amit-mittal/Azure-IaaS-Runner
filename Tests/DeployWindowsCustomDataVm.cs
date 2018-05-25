using System;
using System.IO;
using AzureRunner.Workflows;

namespace AzureRunner.Tests
{
    class DeployWindowsCustomDataVm : BaseWindowsLifecycle
    {
        public DeployWindowsCustomDataVm(string region) : base(region)
        {
            string guid = Guid.NewGuid().ToString().Substring(0, 5);

            groupName = "IsoRunnerRG-WinCD-" + guid;
            deploymentName = "RunnerDeployment-WinCD-" + guid;
            template = File.ReadAllText(@"ArmTemplates\windows.customdata.json");
            templateParameters = File.ReadAllText(@"ArmParameters\windows.customdata.parameters.json");
            username = "<USER>";
            password = "<PASSWORD>";
            customData = "avr-Έέ-1s342Έέ";
        }
    }
}
