using System;
using System.Collections.Generic;
using Stocks.Models;

namespace Stocks.Services {
    public class PortfolioStatisticsCache {
        public static PortfolioStatisticsCache Empty { get; } = new PortfolioStatisticsCache(DateTime.MinValue, new List<IrrHistoryItem>(), new List<PortfolioItemStatistics>());

        public DateTime Datestamp { get; }
        public IList<IrrHistoryItem> IrrHistory { get; }
        public IList<PortfolioItemStatistics> ItemStatistics { get; }

        public PortfolioStatisticsCache(DateTime datestamp, IList<IrrHistoryItem> irrHistory, IList<PortfolioItemStatistics> itemStatistics) {
            Datestamp = datestamp;
            IrrHistory = irrHistory;
            ItemStatistics = itemStatistics;
        }
    }
}
