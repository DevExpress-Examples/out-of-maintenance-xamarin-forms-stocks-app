using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Stocks.Models;
using Stocks.Models.Utils;
using Stocks.Services;
using Stocks.ViewModels.Utils;

namespace Stocks.ViewModels {
    public class SymbolViewModel : BaseViewModel, IErrorHandler {
        ILocalStorage localStorage;
        IViewModelFactory viewModelFactory;
        INavigationService navigationService;

        string ticker;
        bool editInProgress;
        INavigableViewModel selectedChild;
        DelegateCommand addToWatchlistCommand;

        public override string Title => ticker;
        
        public SymbolChartViewModel Chart { get; }
        public SymbolDetailViewModel Detail { get; }
        public SymbolNewsViewModel News { get; }
        public IEnumerable<INavigableViewModel> Children { get; }
        public INavigableViewModel SelectedChild {
            get => selectedChild;
            set => SetProperty(ref selectedChild, value, onChanged: (oldV, v) => {
                Task.Run(() => {
                    if (oldV != null) oldV.Hide();
                    if (v != null) v.Present();
                }).InvokeAndForgetSafeAsync(this);
            });
        }
        public bool IsInWatchlist => localStorage.Tickers.Contains(ticker);

        public IDialogService DialogService { get; set; }

        public ICommand AddToWatchlistCommand => addToWatchlistCommand;
        public ICommand AddToPortfolioCommand { get; }


        public SymbolViewModel(string ticker, IViewModelFactory viewModelFactory, ILocalStorage localStorage, INavigationService navigationService) {
            this.ticker = ticker;
            this.localStorage = localStorage;
            this.viewModelFactory = viewModelFactory;
            this.navigationService = navigationService;

            Chart = viewModelFactory.CreateSymbolChartViewModel(ticker);
            Detail = viewModelFactory.CreateInfoViewModel(ticker);
            News = viewModelFactory.CreateNewsListViewModel(ticker);
            Children = new List<INavigableViewModel> { Chart, Detail, News };
            SelectedChild = Chart;

            addToWatchlistCommand = new DelegateCommand(ExecuteAddToWatchlistCommand, CanExecuteAddToWatchlistCommand);
            AddToPortfolioCommand = new AsyncDelegateCommand(ShowAddTickerTransactionDialog, this);
        }

        protected override void OnPresent() {
            base.OnPresent();
            SelectedChild?.Present();
        }

        protected override void OnHide() {
            base.OnHide();
            SelectedChild?.Hide();
        }

        void ExecuteAddToWatchlistCommand() {
            localStorage.AddTicker(ticker);
            RaisePropertyChanged(nameof(IsInWatchlist));
            addToWatchlistCommand.RaiseCanExecuteChanged();
        }
        bool CanExecuteAddToWatchlistCommand() => !IsInWatchlist;

        async Task ShowAddTickerTransactionDialog() {
            if (editInProgress) return;
            editInProgress = true;
            Portfolio portfolio = localStorage.Portfolio;
            EditTransactionViewModel dialogViewModel = viewModelFactory.CreateEditTransactionViewModel(ticker, portfolio);
            await DialogService.ShowFullScreenDialog(dialogViewModel);

            if (dialogViewModel.IsSubmitted) {
                Transaction transaction = dialogViewModel.ToTransaction();
                portfolio.AddTransaction(transaction);
                SavePortfolio();
            }

            editInProgress = false;
        }

        public void HandleError(Exception exception) {
            throw new Exception("Error on opening 'Add Transaction' dialog.", exception);
        }

        void SavePortfolio() {
            localStorage.SavePortfolio();
            localStorage.PortfolioStatisticsCache = PortfolioStatisticsCache.Empty;
        }
    }
}
