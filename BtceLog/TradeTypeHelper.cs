using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtceApi
{
    public static class TradeTypeHelper
    {
        public static string ToApiFormat(this TradeType tradeType)
        {
            var baseString =  tradeType.ToString();
            return (baseString.Substring(0, 3) + "_" + baseString.Substring(3, 3)).ToLower();
        }
        public static TradeType Parse(string text)
        {
            var strBuild = new StringBuilder(text.Replace("_", "").Replace("/", "").ToLower());
            if (strBuild.Length != 6) throw new ArgumentException();
            return (TradeType) Enum.Parse(typeof(TradeType), strBuild.ToString(), true);
        }
    }
}
