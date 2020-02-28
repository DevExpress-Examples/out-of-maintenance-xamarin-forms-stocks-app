using System;
using Unity;
using Stocks.Models;
using Stocks.ViewModels;
using Stocks.Services;
using Unity.Resolution;

namespace Stocks.UI.Services {
    public class ViewModelFactory : IViewModelFactory {
        IUnityContainer container;

        INavigationService NavigationService => container.Resolve<INavigationService>("TransactionDialog");
        IViewModelFactory Factory => container.Resolve<IViewModelFactory>();
        IHistoricalPriceRepository HistoricalPriceRepository => container.Resolve<IHistoricalPriceRepository>();

        public ViewModelFactory(IUnityContainer container) {
            this.container = container;
        }

        public MainViewModel CreateMainViewModel() => container.Resolve<MainViewModel>();

        public DisclaimerViewModel CreateDisclaimerViewModel() => container.Resolve<DisclaimerViewModel>();
        public EULAViewModel CreateEULAViewModel() => container.Resolve<EULAViewModel>();
        public MarketViewModel CreateMarketViewModel() => container.Resolve<MarketViewModel>();
        public MarketNewsViewModel CreateMarketNewsViewModel() => container.Resolve<MarketNewsViewModel>();
        public WatchlistViewModel CreateWatchlistViewModel() => container.Resolve<WatchlistViewModel>();
        public PortfolioViewModel CreatePortfolioViewModel() => container.Resolve<PortfolioViewModel>();

        public SymbolViewModel CreateSymbolDetailViewModel(string ticker) => container.Resolve<SymbolViewModel>(new ParameterOverride(typeof(string), "ticker", ticker));
        public SymbolChartViewModel CreateSymbolChartViewModel(string ticker) => container.Resolve<SymbolChartViewModel>(new ParameterOverride(typeof(string), "ticker", ticker));
        public SymbolHistoricalPricesViewModel CreateSymbolHistoricalPricesViewModel(string ticker) => container.Resolve<SymbolHistoricalPricesViewModel>(new ParameterOverride(typeof(string), "ticker", ticker));
        public SymbolHistoricalPricesHeaderViewModel CreateSymbolHistoricalPricesHeaderViewModel(string ticker) => container.Resolve<SymbolHistoricalPricesHeaderViewModel>(new ParameterOverride(typeof(string), "ticker", ticker));
        public SymbolDetailViewModel CreateInfoViewModel(string ticker) => container.Resolve<SymbolDetailViewModel>(new ParameterOverride(typeof(string), "ticker", ticker));
        public SymbolNewsViewModel CreateNewsListViewModel(string ticker) => container.Resolve<SymbolNewsViewModel>(new ParameterOverride(typeof(string), "ticker", ticker));

        public NewsItemViewModel CreateNewsItemViewModel(NewsItem newsItem) => container.Resolve<NewsItemViewModel>(new ParameterOverride(typeof(NewsItem), "newsItem", newsItem));

        public MarketSymbolSearchViewModel CreateMarketPageSymbolSearchViewModel() => container.Resolve<MarketSymbolSearchViewModel>();
        public WatchListSymbolSearchViewModel CreateWatchListSymbolSearchViewModel() => container.Resolve<WatchListSymbolSearchViewModel>();

        public EditTransactionViewModel CreateEditTransactionViewModel(Portfolio portfolio) => new EditTransactionViewModel(HistoricalPriceRepository, Factory, NavigationService, portfolio);
        public EditTransactionViewModel CreateEditTransactionViewModel(string ticker, Portfolio portfolio) => new EditTransactionViewModel(ticker, HistoricalPriceRepository, Factory, NavigationService, portfolio);
        public EditTransactionViewModel CreateEditTransactionViewModel(Transaction transaction, Portfolio portfolio) => new EditTransactionViewModel(transaction.Ticker, transaction.Date, transaction.Count, transaction.Price, HistoricalPriceRepository, Factory, NavigationService, portfolio);

        public TickerSearchViewModel CreateTickerSearchViewModel(Action<string> tickerSelectedHandle) {
            return new TickerSearchViewModel(tickerSelectedHandle, container.Resolve<ISymbolRepository>(), NavigationService);
        }

        public PortfolioTransactionsViewModel CreatePortfolioTransactionsViewModel() => container.Resolve<PortfolioTransactionsViewModel>();
    }
}
