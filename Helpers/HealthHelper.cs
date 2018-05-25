using System;
using System.Diagnostics;

namespace AzureRunner.Helpers
{
    public class HealthHelper
    {
        // 0 - cloudservice
        // 1 - runner
        private int healthMode;

        public HealthHelper(int mode)
        {
            this.healthMode = mode;
        }

        public void Healthy(object message)
        {
            switch (healthMode)
            {
                case 0:
                    Trace.TraceInformation(message.ToString());
                    break;

                case 1:
                    Console.WriteLine("[Healthy] " + message.ToString());
                    break;

                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        public void Unhealthy(object message)
        {
            switch (healthMode)
            {
                case 0:
                    Trace.TraceError(message.ToString());
                    break;

                case 1:
                    Console.WriteLine("[Unhealthy] " + message.ToString());
                    break;

                default:
                    throw new NotImplementedException();
                    break;
            }
        }
    }
}
