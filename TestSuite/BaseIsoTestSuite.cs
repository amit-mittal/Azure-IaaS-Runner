using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AzureRunner.Helpers;
using AzureRunner.Tests;

namespace AzureRunner.TestSuite
{
    public abstract class BaseIsoTestSuite : BaseTestSuite
    {
        protected LogHelper logger;

        protected string Region;

        protected BaseIsoTestSuite(string region)
        {
            this.logger = new LogHelper(Main.Mode);

            this.Region = region;
            this.SuiteName = string.Format("{0}_IsoMarketplace", Region);
        }

        public override bool Execute()
        {
            bool final = true;
            TimeSpan maxTimeout = TimeSpan.FromMinutes(30);
            Task<bool>[] tasks = {
                Task.Run(() => this.RunOneTest("WindowsVm", new DeployWindowsVm(Region))),
                Task.Run(() => this.RunOneTest("WindowsCustomDataVm", new DeployWindowsCustomDataVm(Region))),
                Task.Run(() => this.RunOneTest("UbuntuVm", new DeployUbuntuVm(Region))),
                Task.Run(() => this.RunOneTest("CheckpointVm", new DeployCheckpointVm(Region))),
                Task.Run(() => this.RunOneTest("FreeBSD11.0Vm", new DeployFreeBsd11Vm(Region))),
                Task.Run(() => this.RunOneTest("FreeBSD10.3Vm", new DeployFreeBsd10Vm(Region))),
                Task.Run(() => this.RunOneTest("CoreOSVm", new DeployCoreOsVm(Region))),
                Task.Run(() => this.RunOneTest("RedHatCustomDataVm", new DeployRedHatVm(Region))),
                Task.Run(() => this.RunOneTest("CentOSVm", new DeployCentOsVm(Region))),
                //Task.Run(() => this.RunOneTest("UbuntuCoreVm", new DeployUbuntuCoreVm(Region))),
            };

            if (!Task.WaitAll(tasks, maxTimeout))
            {
                logger.Error("Some test timedout, there can be a possible leak of resources.");
                return false;
            }

            foreach (Task<bool> task in tasks)
            {
                final &= task.Result;
            }

            return final;
        }

        private bool RunOneTest(string testName, BaseTest test)
        {
            bool result = true;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                logger.Info("Executing test: " + testName);

                result = test.Execute();
                if (!result)
                {
                    logger.Error("Error in running test: " + testName);
                }
                else
                {
                    logger.Info("Successful in running test: " + testName);
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Error("Error while running test: " + testName + ". Exception: " + e);
            }

            stopWatch.Stop();

            MetricsHelper.PushTestMetric(Convert.ToInt64(stopWatch.Elapsed.TotalSeconds),
                                         this.SuiteName,
                                         testName,
                                         result.ToString());

            return result;
        }
    }
}
