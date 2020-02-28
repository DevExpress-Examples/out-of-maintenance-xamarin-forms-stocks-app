using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stocks.Models {
    public class AvailableCashCalculator {
        double leverage;

        public AvailableCashCalculator(double leverage = 1.0) {
            this.leverage = leverage;
        }

        public async Task<double> Calculate(Portfolio portfolio, IHistoricalPriceRepository historicalPriceRepository, DateTime date, CancellationToken cancellationToken) {
            double cash = portfolio.GetCashValue(date);

            IReadOnlyList<KeyValuePair<string, PortfolioItemBalance>> shortBalances = portfolio.GetShortPositionBalances(date);
            if (shortBalances.Count == 0) return cash;

            IEnumerable<string> shortTickers = shortBalances.Select(s => s.Key);
            //TODO: replace to request list of historical prices by ticker for period
            IDictionary<string, HistoricalPrice> actualPrices = await historicalPriceRepository.GetPricesOnDateAsync(shortTickers, date, cancellationToken);
            double shortMarketValue = shortBalances.Aggregate(0.0, (result, shortBalance) => result += actualPrices[shortBalance.Key].Close * shortBalance.Value.Count);
            
            return cash - shortMarketValue * (leverage + 1) / leverage;
        }
    }
}
