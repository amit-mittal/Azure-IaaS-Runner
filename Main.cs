using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AzureRunner.Helpers;
using AzureRunner.Tests;
using AzureRunner.TestSuite;

namespace AzureRunner
{
    public class Main
    {
        public static int Mode;

        private LogHelper logger;

        private HealthHelper healthHelper;

        public Main(int mode)
        {
            Mode = mode;

            this.logger = new LogHelper(Main.Mode);
            this.healthHelper = new HealthHelper(Main.Mode);
        }

        public bool OnStart()
        {
            TestSetup.AddUser();
            Task.Run(() => TestSetup.CleanupLeakedResourceGroups())
                .Wait(TimeSpan.FromMinutes(10));

            return true;
        }

        public void Run()
        {
            logger.Info("AzureRunner is running");

            while (true)
            {
                try
                {
                    if (DoWork())
                    {
                        healthHelper.Healthy("Runner looking good");
                    }
                    else
                    {
                        healthHelper.Unhealthy("Issue while doing work");
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Got the execption" + e);
                    healthHelper.Unhealthy("Exception while doing work.");
                }
                finally
                {
                    logger.Info("Sleeping for 10 minutes...");
                    Thread.Sleep(TimeSpan.FromMinutes(10));
                }
            }
        }

        /// <summary>
        /// TODO push metrics on how many tests as passing in each run
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, bool> DoWork()
        {
            var testResults = new Dictionary<string, bool>();
            TimeSpan maxTimeout = TimeSpan.FromMinutes(45);
            Task<bool>[] tasks = {
                Task.Run(() => this.RunOneTestSuite(new WestCentralUsIsoTestSuite())),
                Task.Run(() => this.RunOneTestSuite(new JapanEastIsoTestSuite())),
                Task.Run(() => this.RunOneTestSuite(new BrazilSouthIsoTestSuite())),
                Task.Run(() => this.RunOneTestSuite(new WestIndiaIsoTestSuite())),
                Task.Run(() => this.RunOneTestSuite(new KoreaSouthIsoTestSuite())),
            };

            if (!Task.WaitAll(tasks, maxTimeout))
            {
                logger.Error("Some test suite timedout...");
                return testResults;
            }

            //foreach (Task<bool> task in tasks)
            //{
            // TODO Learn how to write good code...
            testResults.Add("WestCentralUsIsoTestSuite", tasks[0].Result);
            testResults.Add("JapanEastIsoTestSuite", tasks[1].Result);
            testResults.Add("BrazilSouthIsoTestSuite", tasks[2].Result);
            testResults.Add("WestIndiaIsoTestSuite", tasks[3].Result);
            testResults.Add("KoreaSouthIsoTestSuite", tasks[4].Result);
            //}

            // TODO Add a test which doesn't need a new CS file and no extra ARM, etc.
            // Only Windows vs Linux vs Region vs VM Type
            // Write code to change JSON parameter file in memory

            return testResults;
        }

        private bool RunOneTestSuite(BaseTestSuite testSuite)
        {
            bool result = true;
            string testSuiteName = testSuite.SuiteName;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                logger.Info("Executing TestSuite: " + testSuite);

                result = testSuite.Execute();
                if (!result)
                {
                    logger.Error("Error in running test suite: " + testSuiteName);
                }
                else
                {
                    logger.Info("Successful in running test suite: " + testSuiteName);
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Error("Error while running test suite: " + testSuiteName + ". Exception: " + e);
            }

            MetricsHelper.PushTestSuiteMetric(Convert.ToInt64(stopWatch.Elapsed.TotalMinutes), testSuiteName, result.ToString());

            return result;
        }
    }
}
