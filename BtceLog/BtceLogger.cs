using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AnoviHelpers;
using BtceApi;
using Newtonsoft.Json;


namespace BtceLog
{
    public class BtceLogger
    {
        private static volatile BtceLogger instance;
        private static object syncRoot = new Object();
        private Logger lLogger = Logger.Instance;
        private const string BtceUrl = "https://btc-e.com/api/2/";
        private Task currenTask;
        private const double epsilon = 0.00001;
        public DataTickersDataContext db;
        public bool IsActive { get; private set; }
        public event Action<Ticker> EventTickerAdd; 

        private const string connectionString =
            "Server=tcp:tradehistoryserver.database.windows.net,1433;Database=BtceTradeHistory;User ID=L0dom@tradehistoryserver;Password=1@3$qWeR;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private BtceLogger()
        {

        }
        public static BtceLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BtceLogger();
                    }
                }

                return instance;
            }
        }

        private string TickerUrl(string tradeType) => Path.Combine(BtceUrl, tradeType, "ticker");
        private string TickerUrl(TradeType tradeType) => TickerUrl(tradeType.ToApiFormat());
        private string FeeUrl(string tradeType) => Path.Combine(BtceUrl, tradeType, "fee");
        private string FeeUrl(TradeType tradeType) => TickerUrl(tradeType.ToApiFormat());
        public DateTime EkbDataTimeNow => DateTime.Now.ToUniversalTime().AddHours(5);

        private string Get(string url)
        {
            lLogger.ALog(3, "Try Get");
            try
            {
                //TODO Something wrong with get responce on ssl/tls connection
                WebRequest req = WebRequest.Create(url);
                WebResponse resp = req.GetResponse();
                Stream stream = resp.GetResponseStream();
                string Out;
                using (StreamReader sr = new StreamReader(stream))
                {
                    Out = sr.ReadToEnd();
                }
                return Out;
            }
            catch (Exception e)
            {
                lLogger.ALog(2, e.Message, "Func:Get");
                throw e;
            }
        }
        
        private Ticker GetTicker(TradeType tradeType)
        {
            var jsonString = Get(TickerUrl(tradeType));
            dynamic json = JsonConvert.DeserializeObject(jsonString);
            return new Ticker
            {
                TickerType = (byte)tradeType,
                WritingDate = EkbDataTimeNow,
                LastPrice = json.ticker.last,
                BuyPrice = json.ticker.buy,
                SellPrice = json.ticker.sell
            };
        }

        private async Task<Ticker> GetTickerAsync(TradeType tradeType) => await Task.Run(() => GetTicker(tradeType));

        private List<Ticker> GetAllTickers()
        {
            var tasks = Enum.GetValues(typeof(TradeType))
                .Cast<TradeType>()
                .Select(GetTickerAsync)
                .ToArray();
            Task.WaitAll(tasks);
            return tasks
                .Select(x => x.IsCompleted ? x.Result : new Ticker())
                .ToList();
        }

        private void Log(List<Ticker> tickers)
        {
            if (!tickers.Any()) return;
            if (EventTickerAdd != null)
                tickers.ForEach(x=> EventTickerAdd(x));

            lLogger.ALog(3, "Try send result to db");
            try
            {
                db.Tickers.InsertAllOnSubmit(tickers);
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                lLogger.ALog(2, e.Message, "Func:Log");
            }
            lLogger.ALog(3, "Complete send result to db");
        }

        private async void ClearOld()
        {
            while (IsActive)
            {
                var old = db.Tickers.Where(x => x.WritingDate.AddMonths(6) < EkbDataTimeNow);
                db.Tickers.DeleteAllOnSubmit(old);
                db.SubmitChanges();
                await Task.Delay(new TimeSpan(10,0,0,0));
            }
        }
        private void WorkThread()
        {
            lLogger.ALog(3, "WorkThread start work");
            var lastTickers = GetAllTickers();
            Log(lastTickers);
            while (IsActive)
            {
                var tickers = GetAllTickers();
                var newTickers = tickers
                    .Where(x =>
                    {
                        var lastTickerOfCurrentType = lastTickers.FirstOrDefault(y => y.TickerType == x.TickerType);
                        if (lastTickerOfCurrentType == null)
                            return true;
                        return !BaseTickerEqual(x, lastTickerOfCurrentType);
                    })
                    .ToList();
                Log(newTickers);
                lastTickers = tickers;
            }
            lLogger.ALog(3, "WorkThread end work");
        }

        public static bool BaseTickerEqual(Ticker x, Ticker y)
        {
            return x.TickerType == y.TickerType &&
                Math.Abs(x.LastPrice - y.LastPrice) < epsilon &&
                Math.Abs(x.BuyPrice - y.BuyPrice) < epsilon &&
                Math.Abs(x.SellPrice - y.SellPrice) < epsilon;
        }
        public Task LogIt()
        {
            lLogger.ALog(3, "Try connect to data base");
            db = new DataTickersDataContext(connectionString);
            lLogger.ALog(3, "Complete connect to data base");
            IsActive = true;
            return currenTask ?? (currenTask = Task.Factory.StartNew(WorkThread));
        }

        public void Stop()
        {
            db.Dispose();
            IsActive = false;
        }
    }
    
}
