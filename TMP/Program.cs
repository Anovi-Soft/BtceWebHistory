using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BtceApi;
using BtceLog;

namespace TMP
{
    class Program
    {
        static Task task;
        static BtceLogger logger = BtceLogger.Instance;
        static void Main(string[] args)
        {
            logger.EventTickerAdd += ticker => Task.Run(()=>
                Console.WriteLine($"[{ticker.WritingDate:G}]\t[{((TradeType)ticker.TickerType).ToApiFormat()}]\tLast:{ticker.LastPrice}\tSell:{ticker.SellPrice}\tBuy:{ticker.BuyPrice}"));
            while (true)
            {
                var readLine = Console.ReadLine();
                if (readLine != null)
                {
                    var line = readLine.Trim().ToLower();
                    switch (line)
                    {
                        case "start":
                            task = logger.LogIt();
                            break;
                        case "stop":
                            logger.Stop();
                            break;
                        case "status":
                            Console.WriteLine(logger.IsActive ? "Log active" : "Log deactive");
                            break;
                        default:
                            Console.WriteLine("Wrong Input");
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
