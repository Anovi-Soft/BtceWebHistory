using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BtceApi;
using Newtonsoft.Json;

namespace BtceLog
{
    public class BtceLog
    {
        private static volatile BtceLog instance;
        private static object syncRoot = new Object();
        private const string BtceUrl = "https://btc-e.com/api/2/";
        private Task currenTask;
        private DataTickersDataContext db;
        private const double epsilon = 0.00001;
        public bool IsActive { get; private set; }

        private const string connectionString =
            "Server=tcp:tradehistoryserver.database.windows.net,1433;Database=BtceTradeHistory;User ID=L0dom@tradehistoryserver;Password=1@3$qWeR;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private BtceLog()
        {
            db = new DataTickersDataContext(connectionString);
        }
        public static BtceLog Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BtceLog();
                    }
                }

                return instance;
            }
        }

        private string TickerUrl(string tradeType) => Path.Combine(BtceUrl, tradeType, "ticker");
        private string TickerUrl(TradeType tradeType) => TickerUrl(tradeType.ToApiFormat());
        private string FeeUrl(string tradeType) => Path.Combine(BtceUrl, tradeType, "fee");
        private string FeeUrl(TradeType tradeType) => TickerUrl(tradeType.ToApiFormat());

        private string Get(string url)
        {
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

        private Ticker GetTicker(TradeType tradeType)
        {
            var jsonString = Get(TickerUrl(tradeType));
            dynamic json = JsonConvert.DeserializeObject(jsonString);
            return new Ticker
            {
                TickerType = tradeType.ToApiFormat(),
                WritingDate = DateTime.Now,
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
            db.Tickers.InsertAllOnSubmit(tickers);
            db.SubmitChanges();
        }

        private async void ClearOld()
        {
            while (IsActive)
            {
                var old = db.Tickers.Where(x => x.WritingDate.AddMonths(6) < DateTime.Now);
                db.Tickers.DeleteAllOnSubmit(old);
                db.SubmitChanges();
                await Task.Delay(new TimeSpan(10,0,0,0));
            }
        }
        private void WorkThread()
        {
            //ClearOld();
            var lastTickers = GetAllTickers();
            Log(lastTickers);
            db.SubmitChanges();
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
            IsActive = true;
            return currenTask ?? (currenTask = Task.Factory.StartNew(WorkThread));
        }

        public void Stop()
        {
            IsActive = false;
        }
    }
    
}
