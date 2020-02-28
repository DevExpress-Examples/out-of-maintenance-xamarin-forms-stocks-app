using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stocks.Models.Utils;
using Stocks.Services;

namespace Stocks.ViewModels {
    public class MainViewModel: BaseViewModel, IErrorHandler {
        ILocalStorage localStorage;
        IViewModelFactory viewModelFactory;
        INavigationService navigationService;

        INavigableViewModel selectedChild;
        
        public IDialogService DialogService { get; set; }

        public MarketViewModel Market { get; }
        public MarketNewsViewModel WatchlistNews { get; }
        public WatchlistViewModel Watchlist { get; }
        public PortfolioViewModel Portfolio { get; }

        public IEnumerable<INavigableViewModel> Children { get; }

        public INavigableViewModel SelectedChild {
            get => selectedChild;
            set => SetProperty(
                ref selectedChild, value,
                onChanged: (oldValue, newValue) => {
                    Task.Run(() => {
                        navigationService.Replace(oldValue, newValue);
                    }).InvokeAndForgetSafeAsync(this);
                }
            );
        }

        public MainViewModel(IViewModelFactory viewModelFactory, ILocalStorage localStorage, INavigationService navigationService) {
            this.viewModelFactory = viewModelFactory;
            this.localStorage = localStorage;
            this.navigationService = navigationService;

            Market = viewModelFactory.CreateMarketViewModel();
            WatchlistNews = viewModelFactory.CreateMarketNewsViewModel();
            Watchlist = viewModelFactory.CreateWatchlistViewModel();
            Portfolio = viewModelFactory.CreatePortfolioViewModel();

            Children = new List<INavigableViewModel> { Market, WatchlistNews, Watchlist, Portfolio };
        }

        public void Initialize() {
            if (localStorage.Tickers.Any())
                SelectedChild = Watchlist;
            else
                SelectedChild = Market;
        }

        public void ShowLicenseAgreement() {
            if (localStorage.ShouldShowEULA)
                DialogService?.ShowFullScreenDialog(viewModelFactory.CreateEULAViewModel()).InvokeAndForgetSafeAsync(this);
            else
                DialogService?.ShowDialog(viewModelFactory.CreateDisclaimerViewModel()).InvokeAndForgetSafeAsync(this);
        }

        protected override void OnPresent() {
            base.OnPresent();
            SelectedChild?.Present();
        }

        protected override void OnHide() {
            base.OnHide();
            SelectedChild?.Hide();
        }

        public void HandleError(Exception exception) {
            throw exception;
        }
    }
}
