using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BtceLog = BtceLog.BtceLog;

namespace TMP
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = global::BtceLog.BtceLog.Instance;
            var task = logger.LogIt();
            task.Wait();
        }
    }
}
