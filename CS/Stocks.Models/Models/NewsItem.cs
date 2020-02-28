using System;

namespace Stocks.Models {
    public class NewsItem {
        public string Headline { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public DateTime Timestamp { get; set; }
        public bool HasPaywall { get; set; }
        public string RelatedTickers { get; set; }
    }
}
