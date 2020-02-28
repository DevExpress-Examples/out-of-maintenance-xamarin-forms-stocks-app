using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Stocks.Models;

namespace Stocks.UI.Services.Mocks {
    public class SymbolsRepository : ISymbolRepository, ISymbolKeyStatsRepository, IHistoricalPriceRepository, INewsRepository {
        const string SymbolsFileName = "symbols.json";
        const string SymbolKeyStatsFileName = "key-stats.json";
        const string NewsFileName = "news.json";

        const int ItemsPerType = 10;

        Random rnd = new Random();
        JsonReader reader;
        List<Symbol> symbols;
        Dictionary<string, SymbolKeyStats> symbolKeyStats;
        List<NewsItem> news;
        Dictionary<string, HistoricalPriceData> historicalPrices;

        Dictionary<SymbolListType, IEnumerable<Symbol>> maglCached;

        public SymbolsRepository() {
            reader = new JsonReader();
        }

        #region ISymbolRepository implementation
        async Task<IEnumerable<Symbol>> ISymbolRepository.FindByFragmentAsync(string fragment, CancellationToken cancellationToken) {
            List<Symbol> items = await GetSymbolsAsync(cancellationToken);
            return items.Where(i => i.Ticker.Contains(fragment)).ToList();
        }

        async Task<IDictionary<SymbolListType, IEnumerable<Symbol>>> ISymbolRepository.GetMarketConditionsAsync(CancellationToken cancellationToken) {
            if (maglCached == null) {
                List<Symbol> items = await GetSymbolsAsync(cancellationToken);
                List<Symbol> gainers = GetRandomItems(items, ItemsPerType);
                foreach (Symbol item in gainers) {
                    item.ChangePercent = 0.3 * rnd.NextDouble();
                    item.Change = item.ChangePercent * item.LatestPrice / (1 + item.ChangePercent);
                    item.LatestUpdate = DateTime.Now;
                }
                List<Symbol> losers = GetRandomItems(items, ItemsPerType);
                foreach (Symbol item in losers) {
                    item.ChangePercent = -0.3 * rnd.NextDouble();
                    item.Change = item.ChangePercent * item.LatestPrice / (1 + item.ChangePercent);
                    item.LatestUpdate = DateTime.Now;
                }
                List<Symbol> mostActive = GetRandomItems(items, ItemsPerType);
                foreach (Symbol item in mostActive) {
                    item.ChangePercent = 0.2 * rnd.NextDouble() - 0.1;
                    item.Change = item.ChangePercent * item.LatestPrice / (1 + item.ChangePercent);
                    item.LatestUpdate = DateTime.Now;
                }

                maglCached = new Dictionary<SymbolListType, IEnumerable<Symbol>> {
                    { SymbolListType.MostActive, mostActive },
                    { SymbolListType.Gainers, gainers },
                    { SymbolListType.Losers, losers },
                };
            }
            return maglCached;
        }

        async Task<Symbol> ISymbolRepository.GetSymbolAsync(string ticker, CancellationToken cancellationToken) {
            List<Symbol> items = await GetSymbolsAsync(cancellationToken);
            return items.FirstOrDefault(i => i.Ticker == ticker);
        }

        async Task<IEnumerable<Symbol>> ISymbolRepository.GetSymbolsAsync(IEnumerable<string> tickers, CancellationToken cancellationToken) {
            List<Symbol> items = await GetSymbolsAsync(cancellationToken);
            return items.Where(i => tickers.Contains(i.Ticker)).ToList();
        }



        async Task<List<Symbol>> GetSymbolsAsync(CancellationToken token) {
            if (symbols == null) {
                Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(SymbolsFileName);
                symbols = await reader.GetAsync<List<Symbol>>(resourceStream, token);
            }
            return symbols;
        }

        List<Symbol> GetRandomItems(List<Symbol> items, int count) {
            if (count > items.Count) {
                throw new ArgumentException("Number of elements in 'items' is less than required by 'count'.");
            }
            List<Symbol> copy = new List<Symbol>(items);
            List<Symbol> result = new List<Symbol>(count);
            for (int i = 0; i < count; ++i) {
                int index = rnd.Next(0, copy.Count);
                result.Add(copy[index]);
                copy.RemoveAt(index);
            }
            return result;
        }
        #endregion

        #region ISymbolKeyStatsRepository implementation
        async Task<SymbolKeyStats> ISymbolKeyStatsRepository.GetAsync(string ticker, CancellationToken cancellationToken) {
            Dictionary<string, SymbolKeyStats> items = await GetSymbolKeyStatsAsync(cancellationToken);
            return items[ticker];
        }

        async Task<Dictionary<string, SymbolKeyStats>> GetSymbolKeyStatsAsync(CancellationToken token) {
            if (symbolKeyStats == null) {
                Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(SymbolKeyStatsFileName);
                symbolKeyStats = await reader.GetAsync<Dictionary<string, SymbolKeyStats>>(resourceStream, token);
            }
            return symbolKeyStats;
        }
        #endregion

        #region IHistoricalPriceRepository implementation
        async Task<IList<HistoricalPrice>> IHistoricalPriceRepository.GetPricesAsync(string ticker, TimeFrame timeInterval, CancellationToken cancellationToken) {
            var prices = await GetHistoricalPrices(cancellationToken);
            return prices[ticker][timeInterval];
        }

        async Task<HistoricalPrice> IHistoricalPriceRepository.GetPriceOnDateAsync(string ticker, DateTime date, CancellationToken cancellationToken) {
            var prices = await GetHistoricalPrices(cancellationToken);
            return prices[ticker].GetPrice(TimeFrame.D, date);
        }

        async Task<IDictionary<string, HistoricalPrice>> IHistoricalPriceRepository.GetPricesOnDateAsync(IEnumerable<string> tickers, DateTime date, CancellationToken cancellationToken) {
            var prices = await GetHistoricalPrices(cancellationToken);
            IDictionary<string, HistoricalPrice> result = new Dictionary<string, HistoricalPrice>();
            foreach (string ticker in tickers) {
                result.Add(ticker, prices[ticker].GetPrice(TimeFrame.D, date));
            }
            return result;
        }

        async Task<IList<HistoricalPrice>> IHistoricalPriceRepository.GetLastPricesAsync(string ticker, TimeFrame timeInterval, DateTime sinceTimestamp, CancellationToken cancellationToken) {
            var prices = await GetHistoricalPrices(cancellationToken);
            return prices[ticker].GetPricesSince(timeInterval, sinceTimestamp);
        }

        async Task<IDictionary<string, IList<HistoricalClosePrice>>> IHistoricalPriceRepository.GetDailyPricesAsync(IEnumerable<DailyPricesRequest> dailyPricesRequests, CancellationToken cancellationToken) {
            var prices = await GetHistoricalPrices(cancellationToken);
            IDictionary<string, IList<HistoricalClosePrice>> result = new Dictionary<string, IList<HistoricalClosePrice>>();
            foreach (DailyPricesRequest request in dailyPricesRequests) {
                result.Add(request.Ticker, prices[request.Ticker]
                    .GetPricesSince(TimeFrame.D, request.StartDate)
                    .Select(p => (HistoricalClosePrice)p)
                    .ToList());
            }
            return result;
        }

        async Task<IDictionary<string, HistoricalPriceData>> GetHistoricalPrices(CancellationToken token) {
            if (historicalPrices == null) {
                List<Symbol> items = await GetSymbolsAsync(token);
                historicalPrices = new Dictionary<string, HistoricalPriceData>(items.Count);
                foreach (Symbol item in items) {
                    historicalPrices.Add(item.Ticker, new HistoricalPriceData(item));
                }
            }
            return historicalPrices;
        }

        Task<DateTime> IHistoricalPriceRepository.GetLastTradeDateAsync(CancellationToken cancellationToken) {
            DateTime date = DateTime.Today;
            do {
                date = date.AddDays(-1);
            } while (date.IsWeekend());
            return Task.FromResult(date);
        }
        #endregion

        #region
        async Task<IEnumerable<NewsItem>> INewsRepository.GetAsync(string ticker, int count, CancellationToken cancellationToken) {
            List<NewsItem> news = await GetNewsAsync(cancellationToken);
            return news.Where(n => n.RelatedTickers.Contains(ticker));
        }

        async Task<IEnumerable<NewsItem>> INewsRepository.GetAsync(IEnumerable<string> tickers, int perTickerCount, CancellationToken cancellationToken) {
            List<NewsItem> news = await GetNewsAsync(cancellationToken);
            return news.Where(n => n.RelatedTickers.ContainsAny(tickers));
        }

        async Task<List<NewsItem>> GetNewsAsync(CancellationToken token) {
            if (news == null) {
                Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(NewsFileName);
                news = await reader.GetAsync<List<NewsItem>>(resourceStream, token);
            }
            return news;
        }

        #endregion
    }

    static class StringUtils {
        public static bool ContainsAny(this string main, IEnumerable<string> values) {
            foreach(string v in values) {
                if (main.Contains(v)) return true;
            }
            return false;
        }
    }
}
