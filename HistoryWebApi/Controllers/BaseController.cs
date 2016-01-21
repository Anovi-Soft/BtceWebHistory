using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using BtceLog;

namespace HistoryWebApi.Controllers
{
    public class BaseController : ApiController
    {
        static BtceLog.BtceLogger logger = BtceLog.BtceLogger.Instance;
        [HttpGet]
        public string Status()
        {
            if (!IsActive)
                return "Status: Deactive";
            return "Status: Active" + "\r\n" +
                   $"Operative memory used:{(double) Process.GetCurrentProcess().PrivateMemorySize64/1024/1024 :F}mb";
        }

        private void Update()
        {
            lock (logger)
                do
                {
                    WebRequest req = WebRequest.Create("http://webhistory.azurewebsites.net/api/base/status");
                    WebResponse resp = req.GetResponse();
                    Stream stream = resp.GetResponseStream();
                    string Out;
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        Out = sr.ReadToEnd();
                    }
                    Thread.Sleep(new TimeSpan(0, 1, 0));
                } while (IsActive);
        }
        private bool IsActive => logger.IsActive;
        [HttpGet]
        public string On()
        {
            if (IsActive)
                return "Server already active";
            logger.LogIt();
            Task.Factory.StartNew(Update);
            return "Activated OK";
        }
        [HttpGet]
        public string Off()
        {
            if (!IsActive)
                return "Server already deactive";
            logger.Stop();
            return "Deactivated OK";
        }
    }

}
