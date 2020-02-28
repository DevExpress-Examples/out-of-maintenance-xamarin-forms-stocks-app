using System.Collections.Generic;
using Stocks.Models;

namespace Stocks.Services {
    public interface ILocalStorage {
        string this[string key] { get; set; }

        bool ShouldShowEULA{ get; set; }
        IEnumerable<string> Tickers { get; }
        Portfolio Portfolio { get; }
        PortfolioStatisticsCache PortfolioStatisticsCache { get; set; }
        TimeFrame TimeFrame { get; set; }
        bool IsCloseOnlyShown { get; set; }
        string ApplicationKey { get; }

        void SavePortfolio();
        void AddTicker(string ticker);
        void RemoveTicker(string ticker);
    }
}
