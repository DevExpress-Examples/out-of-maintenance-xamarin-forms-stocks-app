using System;
using Stocks.Models;
using Stocks.ViewModels;

namespace Stocks.Services {
    public interface IViewModelFactory {
        MainViewModel CreateMainViewModel();

        DisclaimerViewModel CreateDisclaimerViewModel();
        EULAViewModel CreateEULAViewModel();
        MarketViewModel CreateMarketViewModel();
        MarketNewsViewModel CreateMarketNewsViewModel();
        WatchlistViewModel CreateWatchlistViewModel();
        PortfolioViewModel CreatePortfolioViewModel();

        SymbolViewModel CreateSymbolDetailViewModel(string ticker);
        SymbolChartViewModel CreateSymbolChartViewModel(string ticker);
        SymbolHistoricalPricesViewModel CreateSymbolHistoricalPricesViewModel(string ticker);
        SymbolHistoricalPricesHeaderViewModel CreateSymbolHistoricalPricesHeaderViewModel(string ticker);
        SymbolDetailViewModel CreateInfoViewModel(string ticker);
        SymbolNewsViewModel CreateNewsListViewModel(string ticker);

        NewsItemViewModel CreateNewsItemViewModel(NewsItem newsItem);

        MarketSymbolSearchViewModel CreateMarketPageSymbolSearchViewModel();
        WatchListSymbolSearchViewModel CreateWatchListSymbolSearchViewModel();

        EditTransactionViewModel CreateEditTransactionViewModel(Portfolio portfolio);
        EditTransactionViewModel CreateEditTransactionViewModel(string ticker, Portfolio portfolio);
        EditTransactionViewModel CreateEditTransactionViewModel(Transaction transaction, Portfolio portfolio);

        TickerSearchViewModel CreateTickerSearchViewModel(Action<string> selectTickerHandle);
        PortfolioTransactionsViewModel CreatePortfolioTransactionsViewModel();
    }
}
