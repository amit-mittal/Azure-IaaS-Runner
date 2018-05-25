using System;
using System.Diagnostics;

namespace AzureRunner.Helpers
{
    public class LogHelper
    {
        // 0 - cloudservice
        // 1 - runner
        private int loggingMode;

        public LogHelper(int mode)
        {
            this.loggingMode = mode;
        }

        public void Info(object message)
        {
            switch (loggingMode)
            {
                case 0:
                    Trace.TraceError(message.ToString());
                    break;

                case 1:
                    Console.WriteLine("[Info] " + message);
                    break;

                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        public void Error(object message)
        {
            switch (loggingMode)
            {
                case 0:
                    Trace.TraceError(message.ToString());
                    break;

                case 1:
                    Console.WriteLine("[Error] " + message);
                    break;

                default:
                    throw new NotImplementedException();
                    break;
            }
        }
    }
}
