namespace Stocks.Models {
    public enum SecurityType {
        ADR,
        REIT,
        SecondaryIssue,
        LimitedPartnerships,
        CommonStock,
        ETF,
        Warrant,
        OpenEndedFund,
        ClosedEndedFund,
        ClosedEndFund,
        PreferredStock
    }

    public class SecurityPaper {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public SecurityType Type { get; }
        public string Region { get; set; }
        public string Exchange { get; set; }
    }
}
