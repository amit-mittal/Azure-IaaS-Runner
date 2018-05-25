using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureRunner.TestSuite
{
    public abstract class BaseTestSuite
    {
        public string SuiteName;

        public abstract bool Execute();
    }
}
