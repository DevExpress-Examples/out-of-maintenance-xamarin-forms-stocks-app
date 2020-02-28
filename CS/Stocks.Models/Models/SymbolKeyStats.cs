using System;

namespace Stocks.Models {
    public class SymbolKeyStats {
        public string CompanyName { get; set; }
        public double MarketCapitalization { get; set; }
        public double Week52High { get; set; }
        public double Week52Low { get; set; }
        public double SharesOutstanding { get; set; }
        public double Float { get; set; }
        public double Average30DayVolume { get; set; }
        public double Average10DayVolume { get; set; }
        public int Employees { get; set; }
        public double Trailing12MonthEarnings { get; set; }
        public double Trailing12MonthDividendRate { get; set; }
        public double DividentYield { get; set; }
        public DateTime NextDividentDate { get; set; }
        public DateTime ExDividentDate { get; set; }
        public DateTime NextEarningsDate { get; set; }
        public double PriceToEarningsRatio { get; set; }
        public double Beta { get; set; }
        public double Day200MovingAverage { get; set; }
        public double Day50MovingAverage { get; set; }
        public double MaxChangePercent { get; set; }
        public double Year5ChangePercent { get; set; }
        public double Year2ChangePercent { get; set; }
        public double Year1ChangePercent { get; set; }
        public double YtdChangePercent { get; set; }
        public double Month6ChangePercent { get; set; }
        public double Month3ChangePercent { get; set; }
        public double Month1ChangePercent { get; set; }
        public double Day30ChangePercent { get; set; }
        public double Day5ChangePercent { get; set; }
    }
}
