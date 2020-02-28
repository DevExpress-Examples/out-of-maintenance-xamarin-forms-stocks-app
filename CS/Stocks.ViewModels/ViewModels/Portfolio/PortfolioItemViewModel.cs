using System;
using Stocks.Models;

namespace Stocks.ViewModels {
    public enum PositionType {
        Long,
        Short
    };

    public class PortfolioItemViewModel : NotificationObject {
        PortfolioItemStatistics statistics;

        public string Ticker => statistics.Ticker;
        public int Count => Math.Abs(statistics.Count);
        public PositionType Type => statistics.Count >= 0 ? PositionType.Long : PositionType.Short;

        public string OperationPriceText => $"${statistics.OperationPrice:n2}";
        public ChangeType PriceChangeType => statistics == null
            ? ChangeType.None
            : ChangeTypeUtils.FromDouble(statistics.ActualPrice - statistics.OperationPrice);
        public string ActualPriceText => $"${statistics.ActualPrice:n2}";
        public string ActualNetValue => $"${Math.Abs(statistics.ActualValue):n2}";

        public ChangeType TotalChangeType => statistics == null
            ? ChangeType.None
            : ChangeTypeUtils.FromDouble(statistics.TotalProfit); 
        public string TotalChangeText => $"${Math.Abs(statistics.TotalProfit):n2} ({Math.Abs(statistics.ProfitPercent):p2})";

        public string PartInTotal => $"{statistics.PartInTotal:p2}";

        public PortfolioItemViewModel(PortfolioItemStatistics statistics) {
            this.statistics = statistics;
        }
    }
}
