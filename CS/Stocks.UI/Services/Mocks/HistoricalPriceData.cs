using System;
using System.Collections.Generic;
using System.Linq;
using Stocks.Models;

namespace Stocks.UI.Services.Mocks {
    public class HistoricalPriceSlice {
        public TimeFrame TimeFrame { get; }
        public IList<HistoricalPrice> Prices { get; }

        public HistoricalPriceSlice(TimeFrame frame, IEnumerable<HistoricalPrice> prices) {
            TimeFrame = frame;
            Prices = new List<HistoricalPrice>(prices);
        }
    }

    public class HistoricalPriceData {
        Random rnd = new Random();        
        const int defaultValueCount = 170;
        Symbol refValue;

        Dictionary<TimeFrame, IList<HistoricalPrice>> framedData = new Dictionary<TimeFrame, IList<HistoricalPrice>>();

        public IList<HistoricalPrice> this[TimeFrame frame] {
            get {
                IList<HistoricalPrice> prices = null;
                if (!framedData.TryGetValue(frame, out prices)) {
                    prices = GeneratePrices(frame);
                    framedData.Add(frame, prices);
                }
                return prices;
            }
        }

        public HistoricalPriceData(Symbol symbol) {
            if (symbol == null) {
                throw new ArgumentNullException(nameof(symbol));
            }
            refValue = symbol;
        }

        public HistoricalPrice GetPrice(TimeFrame frame, DateTime timestamp) {
            IList<HistoricalPrice> prices = this[frame];
            DateTime normalizedTimestamp = timestamp.Round(frame);
            GenerateValuesIfNotStored(prices, frame, normalizedTimestamp);
            return GetPrice(prices, normalizedTimestamp);
        }

        public IList<HistoricalPrice> GetPricesSince(TimeFrame frame, DateTime timestamp) {
            IList<HistoricalPrice> prices = this[frame];
            DateTime normalizedStart = timestamp.Round(frame);
            DateTime normalizedEnd = DateTime.Now.Round(frame);
            UpdateLastPriceIfNeeded(prices, frame, timestamp);
            GenerateValuesIfNotStored(prices, frame, normalizedStart);
            GenerateValuesIfNotStored(prices, frame, normalizedEnd);
            return GetPrices(prices, normalizedStart, normalizedEnd);
        }

        IList<HistoricalPrice> GeneratePrices(TimeFrame frame) {
            var result = new List<HistoricalPrice>();

            DateTime timestamp = DateTime.Now.Round(frame);
            double close = refValue.LatestPrice;
            double open = refValue.LatestPrice - refValue.Change;
            result.Add(new HistoricalPrice(timestamp, open, GenerateHigh(open, close), GenerateLow(open, close), close, GenerateVolume()));

            for (int i = 1; i < defaultValueCount; ++i) {    
                close = open;
                open = GenerateOpenClose(close);
                timestamp = timestamp.Add(frame, -1);
                result.Add(new HistoricalPrice(timestamp, open, GenerateHigh(open, close), GenerateLow(open, close), close, GenerateVolume()));
            }
            result.Reverse();

            return result;
        }

        void GenerateValuesIfNotStored(IList<HistoricalPrice> prices, TimeFrame frame, DateTime timestamp) {
            DateTime start = prices.First().Timestamp;
            DateTime end = prices.Last().Timestamp;
            if (timestamp > end) {
                AddValuesToEnd(prices, frame, timestamp);
            } else if (timestamp < start) {
                AddValuesToStart(prices, frame, timestamp);
            }
        }

        void AddValuesToStart(IList<HistoricalPrice> prices, TimeFrame frame, DateTime target) {
            HistoricalPrice startPrice = prices.First();
            DateTime timestamp = startPrice.Timestamp;
            double close = startPrice.Open;
            while (timestamp > target) {
                timestamp = timestamp.Add(frame, -1);
                double open = GenerateOpenClose(close);
                prices.Insert(0, new HistoricalPrice(timestamp, open, GenerateHigh(open, close), GenerateLow(open, close), close, GenerateVolume()));
                close = open;
            }
        }

        void AddValuesToEnd(IList<HistoricalPrice> prices, TimeFrame frame, DateTime target) {
            HistoricalPrice startPrice = prices.Last();
            DateTime timestamp = startPrice.Timestamp;
            double open = startPrice.Open;
            while (timestamp > target) {
                timestamp = timestamp.Add(frame, 1);
                double close = GenerateOpenClose(open);
                prices.Insert(0, new HistoricalPrice(timestamp, open, GenerateHigh(open, close), GenerateLow(open, close), close, GenerateVolume()));
                open = close;
            }
        }

        void UpdateLastPriceIfNeeded(IList<HistoricalPrice> prices, TimeFrame frame, DateTime timestamp) {
            DateTime roundTS = timestamp.Round(frame);
            HistoricalPrice lastPrice = prices.Last();
            if (lastPrice.Timestamp == roundTS) {
                lastPrice.Close = GenerateOpenClose(lastPrice.Open);
                lastPrice.High = Math.Max(lastPrice.High, GenerateHigh(lastPrice.Open, lastPrice.Close));
                lastPrice.Low = Math.Min(lastPrice.Low, GenerateLow(lastPrice.Open, lastPrice.Close));
            }
        }

        HistoricalPrice GetPrice(IList<HistoricalPrice> prices, DateTime timestamp) {
            return prices.First(p => p.Timestamp == timestamp);
        }
        IList<HistoricalPrice> GetPrices(IList<HistoricalPrice> prices, DateTime start, DateTime end) {
            return prices.Where(p => p.Timestamp >= start && p.Timestamp <= end).ToList();
        }

        double GenerateHigh(double open, double close) {
            return Math.Max(open, close) * (1 + 0.3 * rnd.NextDouble());
        }
        double GenerateLow(double open, double close) {
            return Math.Min(open, close) * (1 - 0.3 * rnd.NextDouble());
        }
        double GenerateOpenClose(double value) {
            return value * (0.7 + 0.6 * rnd.NextDouble());
        }
        long GenerateVolume() {
            return rnd.Next(100_000, 1_000_000);
        }
    }
}
