using System;
using System.ComponentModel;

namespace Stocks.Models {
    public enum SymbolListType {
        [Description("Most Active")]
        MostActive = 0,
        [Description("Gainers")]
        Gainers = 1,
        [Description("Losers")]
        Losers = 2,
        [Description("Volume")]
        Volume = 3,
        [Description("Percent")]
        Percent = 4
    }

    public class Symbol {
        public string Ticker { get; set; }
        public string CompanyName { get; set; }
        public double LatestPrice { get; set; }
        public double Change { get; set; }
        public double ChangePercent { get; set; }
        public DateTime LatestUpdate { get; set; }
    }
}