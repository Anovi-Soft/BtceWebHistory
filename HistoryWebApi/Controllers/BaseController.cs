using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BtceLog;

namespace HistoryWebApi.Controllers
{
    public class BaseController : ApiController
    {
        BtceLog.BtceLog logger = BtceLog.BtceLog.Instance;
        Task loggerTask;
        //Request.RequestUri.Query
        [HttpGet]
        public string Status()
        {
            return "Status:" + (IsActive ? "Active" : "Deactive");
        }

        private bool IsActive => !(loggerTask == null || loggerTask.IsCanceled);
        //Request.RequestUri.Query
        [HttpGet]
        public string Activate()
        {
            if (IsActive)
                return "Server already active";
            loggerTask = logger.LogIt();
            return "OK";
        }
        public string Deactive()
        {
            if (!IsActive)
                return "Server already deactive";
            logger.Stop();
            return "OK";
        }
    }
    
}
