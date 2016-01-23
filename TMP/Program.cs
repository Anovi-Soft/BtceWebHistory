using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BtceApi;
using BtceLog;
using AnoviHelpers;

namespace TMP
{
    class Program
    {
        static Task task;
        static BtceLogger logger = BtceLogger.Instance;
        static void Main(string[] args)
        {
            var lLogger = Logger.Instance;
            lLogger.Level = 10;
            lLogger.ALog(0, "Hello, I`m start work");
            logger.EventTickerAdd += ticker => Task.Run(()=>
                Logger.ASLog(1,$"\t[{((TradeType)ticker.TickerType).ToApiFormat()}]\tLast:{ticker.LastPrice}\tSell:{ticker.SellPrice}\tBuy:{ticker.BuyPrice}","Ticker"));
            while (true)
            {
                lLogger.ALog(0, "I`m listen you");
                var readLine = Console.ReadLine();
                if (readLine != null)
                {
                    var line = readLine.Trim().ToLower();
                    switch (line)
                    {
                        case "start":
                            task = logger.LogIt();
                            lLogger.ALog(0, "Ok. Log active");
                            break;
                        case "stop":
                            logger.Stop();
                            lLogger.ALog(0, "Ok. Log deactive");
                            break;
                        case "status":
                            if (task.Exception != null)
                            {
                                lLogger.ALog(2, task.Exception.ToString(), "Exception");
                                logger.Stop();
                            }
                            lLogger.ALog(0, logger.IsActive ? "Log active" : "Log deactive");
                            break;
                        default:
                            lLogger.ALog(0, "Wrong Input");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Please input something");
                }
            }
        }
    }
}
