using System;

namespace Stocks.Models {
    public class HistoricalClosePrice {
        public DateTime Timestamp { get; set; }
        public double Close { get; set; }

        public HistoricalClosePrice() {
        }
        public HistoricalClosePrice(DateTime timestamp, double close) {
            Timestamp = timestamp;
            Close = close;
        }
    }

    public class HistoricalPrice : HistoricalClosePrice {
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public long Volume { get; set; }

        public HistoricalPrice() {
        }
        public HistoricalPrice(DateTime timestamp, double open, double high, double low, double close, long volume) : base(timestamp, close) {
            Timestamp = timestamp;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }
    }
}
