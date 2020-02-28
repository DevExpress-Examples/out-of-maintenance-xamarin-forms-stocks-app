using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stocks.Models {
    public class IrrHistoryItem {
        public DateTime Date { get; }
        public double InternalRateOfReturn { get; }

        public IrrHistoryItem(DateTime date, double incomePercent) {
            Date = date;
            InternalRateOfReturn = incomePercent;
        }
    }

    // IRR = Internal Rate of Return
    public class IrrHistoryCalculator {
        const int HistoryLimintInYears = 1;

        public async Task<IList<IrrHistoryItem>> Calculate(Portfolio portfolio, IHistoricalPriceRepository historicalPriceRepository, CancellationToken cancellationToken) {
            DateTime startDate = GetStartDate(portfolio);
            if (startDate == DateTime.MaxValue) return new List<IrrHistoryItem>();

            DateTime endDate = DateTime.Today;
            // Cannot calculate XIRR for the same date as the portfolio starts (AddDays(1)).
            DateTime adjustedStartDate = startDate.AddDays(1);//ClipStartDate(startDate, endDate);

            XirrCalculator xirrCalculator = new XirrCalculator();
            IDictionary<string, IList<HistoricalClosePrice>> pricesHistoryBySymbol = await historicalPriceRepository.GetDailyPricesAsync(FindStartDatesOfSymbols(portfolio, adjustedStartDate, endDate), cancellationToken);
            Dictionary<DateTime, Dictionary<string, double>> pricesByDate = GroupPricesByDateSymbol(pricesHistoryBySymbol);

            List<IrrHistoryItem> result = new List<IrrHistoryItem>(pricesByDate.Keys.Count);
            double prevYearlyRate = 0.1;
            foreach (DateTime date in pricesByDate.Keys) {
                IList<CashFlow> flows = portfolio.GetCashFlows(date, pricesByDate[date]);
                double? yearlyRate = xirrCalculator.Calculate(flows, prevYearlyRate);
                if (yearlyRate.HasValue) {
                    prevYearlyRate = yearlyRate.Value;
                    double realRate = Math.Pow((1 + yearlyRate.Value), (date - startDate).TotalDays / 365.0) - 1;
                    if (!double.IsNaN(realRate)) {
                        result.Add(new IrrHistoryItem(date, realRate));
                    }
                }
            }
            var month = result.Where(r => r.Date.Month == 8 && r.Date.Year == 2018);
            return result;
        }

        DateTime GetStartDate(Portfolio portfolio) {
            DateTime portfolioStartDate = DateTime.MaxValue;
            foreach (string ticker in portfolio.Tickers) {
                if (portfolio[ticker].Transactions.Count == 0) continue;
                DateTime tickerStartDate = portfolio[ticker].Transactions.Select(t => t.Date).Min();
                if (portfolioStartDate > tickerStartDate) {
                    portfolioStartDate = tickerStartDate;
                }
            }
            return portfolioStartDate;
        }

        //DateTime ClipStartDate(DateTime startDate, DateTime endDate) {
        //    DateTime yearAgo = endDate.AddYears(-HistoryLimintInYears);
        //    // Cannot calculate XIRR for the same date as the portfolio starts.
        //    return (startDate < yearAgo) ? yearAgo : startDate.AddDays(1);
        //}

        IEnumerable<DailyPricesRequest> FindStartDatesOfSymbols(Portfolio portfolio, DateTime startDate, DateTime endDate) {
            int daysToCalculate = (int)(endDate - startDate).TotalDays;
            Dictionary<string, DateTime> symbolStartTradingDates = new Dictionary<string, DateTime>();

            for (int i = 0; i < daysToCalculate; ++i) {
                DateTime date = startDate.AddDays(i);
                var symbols = portfolio.GetOpenedPositions(date);
                foreach (string symbol in symbols) {
                    if (!symbolStartTradingDates.ContainsKey(symbol)) {
                        symbolStartTradingDates.Add(symbol, date);
                    }
                }
            }

            return symbolStartTradingDates
                .Select(p => new DailyPricesRequest(p.Key, p.Value))
                .ToList();
        }

        Dictionary<DateTime, Dictionary<string, double>> GroupPricesByDateSymbol(IDictionary<string, IList<HistoricalClosePrice>> symbolToPricesMapping) {
            Dictionary<DateTime, Dictionary<string, double>> result = new Dictionary<DateTime, Dictionary<string, double>>();
            foreach (KeyValuePair<string, IList<HistoricalClosePrice>> mapping in symbolToPricesMapping) {
                foreach (HistoricalClosePrice price in mapping.Value) {
                    Dictionary<string, double> symbolToPriceMappings;
                    if (!result.TryGetValue(price.Timestamp, out symbolToPriceMappings)) {
                        symbolToPriceMappings = new Dictionary<string, double>();
                        result.Add(price.Timestamp, symbolToPriceMappings);
                    }
                    symbolToPriceMappings.Add(mapping.Key, price.Close);
                }
            }
            return result;
        }
    }
}
