using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stocks.Models {
    public class PortfolioItemStatistics {
        public string Ticker { get; }
        public double OperationPrice { get; }
        public double ActualPrice { get; }
        public int Count { get; }

        public double OperationValue => OperationPrice * Count;
        public double ActualValue => ActualPrice * Count;
        public double AbsoluteActualValue => Math.Abs(ActualValue);
        public double Profit => ActualPrice - OperationPrice;
        public double ProfitPercent => Profit / OperationPrice;

        public double TotalProfit => Count * Profit;
        public double TotalProfitPercent => TotalProfit / Math.Abs(OperationValue);
        public double AbsoluteProfitPercent => Math.Round(Math.Abs(ProfitPercent), 4);

        public double PartInTotal { get; private set; }

        public PortfolioItemStatistics(string ticker, int count, double operationPrice, double actualPrice) {
            Ticker = ticker;
            Count = count;
            OperationPrice = operationPrice;
            ActualPrice = actualPrice;
        }

        public void CalculatePart(double totalValue) {
            PartInTotal = Math.Abs(Count * ActualPrice / totalValue);
        }
    }

    public class PortfolioStatisticsCalculator {
        public async Task<IList<PortfolioItemStatistics>> Calculate(Portfolio portfolio, ISymbolRepository listSymbolRepository, CancellationToken cancellationToken) {
            IList<string> openedPositions = portfolio.GetOpenedPositions(DateTime.Today);
            IReadOnlyDictionary<string, double> latestPrices = (await listSymbolRepository.GetSymbolsAsync(openedPositions, cancellationToken)).ToDictionary(s => s.Ticker, s => s.LatestPrice);

            List<PortfolioItemStatistics> statistics = new List<PortfolioItemStatistics>(latestPrices.Count);
            double totalValue = 0;
            foreach (var priceMapping in latestPrices) {
                int count = portfolio[priceMapping.Key].ActualCount;
                double inputPrice = portfolio[priceMapping.Key].ActualPrice;
                double actualPrice = priceMapping.Value;

                PortfolioItemStatistics statisticsItem = new PortfolioItemStatistics(
                    priceMapping.Key,
                    count,
                    inputPrice,
                    actualPrice);
                statistics.Add(statisticsItem);
                totalValue += Math.Abs(statisticsItem.ActualValue);
            }
            foreach(PortfolioItemStatistics item in statistics) {
                item.CalculatePart(totalValue);
            }

            return statistics;
        }
    }
}