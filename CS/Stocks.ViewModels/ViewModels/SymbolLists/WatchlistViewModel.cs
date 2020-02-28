using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Stocks.Models;
using Stocks.Services;
using Stocks.ViewModels.Utils;

namespace Stocks.ViewModels {
    public class WatchlistViewModel: DetailedListViewModel<ListSymbolWrapper> {
        ILocalStorage localStorage;
        ISymbolRepository listSymbolRepository;
        
        public override string Title => "Watchlist";

        public ICommand RemoveTickerCommand { get; }
        public ICommand AddToPortfolioCommand { get; }

        public WatchlistViewModel(ILocalStorage localStorage, ISymbolRepository listSymbolRepository, IViewModelFactory factory, INavigationService navigationService) : base(factory, navigationService) {
            this.localStorage = localStorage;
            this.listSymbolRepository = listSymbolRepository;
            AddToPortfolioCommand = new AsyncDelegateCommand<ListSymbolWrapper>(ShowAddTickerTransactionDialog, this);
            RemoveTickerCommand = new DelegateCommand<ListSymbolWrapper>(ExecuteRemoveTickerCommand);
        }

        protected override async Task<IList<ListSymbolWrapper>> SendRequestAsync(CancellationToken token) {
            IEnumerable<string> tickers = localStorage.Tickers;
            IEnumerable<Symbol> symbols = await listSymbolRepository.GetSymbolsAsync(tickers, token);
            return symbols.Select(s => new ListSymbolWrapper(s)).ToList() ;
        }

        protected override SymbolSearchViewModel CreateSymbolSearchViewModel() {
            return Factory.CreateWatchListSymbolSearchViewModel();
        }

        void ExecuteRemoveTickerCommand(ListSymbolWrapper symbol) {
            localStorage.RemoveTicker(symbol.Ticker);
            Content.Remove(symbol);
            RequestUpdate();
        }

        async Task ShowAddTickerTransactionDialog(ListSymbolWrapper symbol) {
            string ticker = symbol.Ticker;
            if (IsInEditProcess) return;
            IsInEditProcess = true;
            Portfolio portfolio = localStorage.Portfolio;
            EditTransactionViewModel dialogViewModel = Factory.CreateEditTransactionViewModel(ticker, portfolio);
            await DialogService.ShowFullScreenDialog(dialogViewModel);

            if (dialogViewModel.IsSubmitted) {
                Transaction transaction = dialogViewModel.ToTransaction();
                portfolio.AddTransaction(transaction);
                localStorage.SavePortfolio();
            }

            IsInEditProcess = false;
        }

        protected override void OnPresent() {
            if (localStorage.Tickers != null && localStorage.Tickers.Any()) {
                RequestUpdate();
            } else {
                SetHasContentState(null);
            }
        }
    }
}