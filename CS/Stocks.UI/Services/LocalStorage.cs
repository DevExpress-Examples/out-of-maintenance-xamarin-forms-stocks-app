using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Stocks.Models;
using Stocks.Services;
using Xamarin.Essentials;

namespace Stocks.UI.Services {
    public class LocalStorage : ILocalStorage {
        private const string TickersKey = "STOCKS_WATCHLIST_TICKERS";
        private const string PortfolioKey = "STOCKS_PORTFOLIO";
        private const string PortfolioStatisticsCacheKey = "STOCKS_PORTFOLIO_STATISTICS_CACHE";
        private const string TimeFrameKey = "STOCKS_HISTORICALPRICES_TIMEFRAME";
        private const string IsCloseOnlyShownKey = "STOCKS_HISTORICALPRICES_ISCLOSEONLYSHOWN";
        private const string EULAKey = "STOCKS_EULA";
        private const string ApiKey = "STOCKS_APPLICATION_KEY";

        private const string EmptyTickersValue = "[]";

        List<string> tickers;
        Portfolio portfolio;
        PortfolioStatisticsCache portfolioStatisticsCache;
        JsonSerializerSettings jsonSettings;

        public string this[string key] {
            get => Preferences.Get(key, string.Empty);
            set => Preferences.Set(key, value);
        }

        public IEnumerable<string> Tickers {
            get {
                if (tickers == null) {
                    LoadTickers();
                }
                return tickers;
            }
        }

        public Portfolio Portfolio {
            get {
                if (portfolio == null) {
                    string savedValue = Preferences.Get(PortfolioKey, string.Empty);
                    portfolio = string.IsNullOrEmpty(savedValue)
                        ? new Portfolio()
                        : JsonConvert.DeserializeObject<Portfolio>(savedValue, jsonSettings);
                }
                return portfolio;
            }
        }

        public PortfolioStatisticsCache PortfolioStatisticsCache {
            get {
                if (portfolioStatisticsCache == null) {
                    string savedValue = Preferences.Get(PortfolioStatisticsCacheKey, string.Empty);
                    if (string.IsNullOrEmpty(savedValue)) {
                        portfolioStatisticsCache = PortfolioStatisticsCache.Empty;
                    } else {
                        portfolioStatisticsCache = JsonConvert.DeserializeObject<PortfolioStatisticsCache>(savedValue, jsonSettings);
                    }
                }
                return portfolioStatisticsCache;
            }
            set {
                if (portfolioStatisticsCache == value) return;
                portfolioStatisticsCache = value;
                string serializedCache = JsonConvert.SerializeObject(value, jsonSettings);
                Preferences.Set(PortfolioStatisticsCacheKey, serializedCache);
            }
        }


        public TimeFrame TimeFrame {
            get => (TimeFrame)Preferences.Get(TimeFrameKey, (int)TimeFrame.M1);
            set => Preferences.Set(TimeFrameKey, (int)value);
        }

        public bool ShouldShowEULA {
            get => Preferences.Get(EULAKey, true);
            set => Preferences.Set(EULAKey, value);
        }

        public bool IsCloseOnlyShown {
            get => Preferences.Get(IsCloseOnlyShownKey, true);
            set => Preferences.Set(IsCloseOnlyShownKey, value);
        }

        public string ApplicationKey {
            get {
                if (!Preferences.ContainsKey(ApiKey)) {
                    Preferences.Set(ApiKey, GenerateApiKey());
                }
                return Preferences.Get(ApiKey, GenerateApiKey());
            }
        }
        

        public LocalStorage() {
            jsonSettings = new JsonSerializerSettings();

            jsonSettings.Converters.Add(new PortfolioConverter());
            jsonSettings.Converters.Add(new PortfolioItemConverter());
            jsonSettings.Converters.Add(new TransactionConverter());

            jsonSettings.Converters.Add(new PortfolioStatisticsCacheConverter());
            jsonSettings.Converters.Add(new IrrHistoryItemConverter());
            jsonSettings.Converters.Add(new PortfolioItemStatisticsConverter());
        }

        public void AddTicker(string ticker) {
            if (tickers == null) {
                LoadTickers();
            }
            if (tickers.Contains(ticker)) {
                return;
            }
            tickers.Add(ticker);
            Update();
        }

        public void RemoveTicker(string ticker) {
            tickers.Remove(ticker);
            Update();
        }

        string GenerateApiKey() {
            return $"mobile_{Guid.NewGuid().ToString().Replace("-", "")}";
        }
        void LoadTickers() {
            string savedValue = Preferences.Get(TickersKey, EmptyTickersValue);
            if (string.IsNullOrEmpty(savedValue)) {
                savedValue = EmptyTickersValue;
            }
            tickers = JsonConvert.DeserializeObject<List<string>>(savedValue);
        }

        void Update() {
            if (tickers == null || !tickers.Any()) {
                Preferences.Set(TickersKey, EmptyTickersValue);
                return;
            }
            Preferences.Set(TickersKey, JsonConvert.SerializeObject(tickers));
        }

        public void SavePortfolio() {
            string serializedPortfolio = JsonConvert.SerializeObject(portfolio, jsonSettings);
            Preferences.Set(PortfolioKey, serializedPortfolio);
        }
    }
}
