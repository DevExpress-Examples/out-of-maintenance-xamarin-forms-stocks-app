using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Stocks.Models;
using Stocks.Models.Utils;
using Stocks.Services;
using Stocks.ViewModels.Utils;

namespace Stocks.ViewModels {
    public abstract class ItemSearchViewModel<T> : InitializableViewModel<IList<T>>, ISearchViewModel, IDisposable {
        const string UnableToLocateTickerPlaceholderText = "Unable to locate ticker symbol.";
        const int SearchWaitInterval = 750;
        const int SearchPeriod = 0;

        bool isDisposed;
        Timer searchTimer;
        string fragment;
        string searchResult;
        string previousFragment;

        protected ISymbolRepository ListSymbolRepository { get; }

        public string Fragment {
            get => fragment;
            set => SetProperty(ref fragment, value, onChanged: (_, __) => OnFragmentChanged());
        }
        public string SearchResult {
            get => searchResult;
            set => SetProperty(ref searchResult, value);
        }

        public ICommand SelectSymbolCommand { get; }

        protected INavigationService NavigationService { get; }

        protected ItemSearchViewModel(ISymbolRepository listSymbolRepository, INavigationService navigationService) : base(InitializableViewModelState.Initialized) {
            NavigationService = navigationService;
            ListSymbolRepository = listSymbolRepository;
            SelectSymbolCommand = new AsyncDelegateCommand<T>(ExecuteSelectSymbolCommand, null, this);
        }

        public void Dispose() {
            Dispose(true);
        }

        public void SearchImmediately() {
            StopSearchTimer();
            SearchTimerCallback(null);
        }

        protected void Dispose(bool disposing) {
            if (isDisposed) return;
            StopSearchTimer();
            isDisposed = true;
        }

        void OnFragmentChanged() {
            StopSearchTimer();
            searchTimer = new Timer(
                callback: SearchTimerCallback,
                state: null,
                dueTime: SearchWaitInterval,
                period: SearchPeriod);
        }

        void StopSearchTimer() {
            if (searchTimer == null) return;
            searchTimer.Dispose();
            searchTimer = null;
        }

        protected abstract Task ExecuteSelectSymbolCommand(T selectedItem);
        protected abstract Task<IList<T>> FindSymbols(CancellationToken token);

        protected override Task<IList<T>> SendRequestAsync(CancellationToken token) {
            return FindSymbols(token);
        }

        protected override bool HasContent() => base.HasContent() && Content.Any();

        protected override void SetHasContentState(IList<T> c) {
            if (c != null) {
                previousFragment = Fragment;
                base.SetHasContentState(new BindingList<T>(c));
            } else {
                base.SetHasContentState(c);
            }
        }

        protected override void OnContentChanged(IList<T> oldValue, IList<T> newValue) {
            base.OnContentChanged(oldValue, newValue);
            UpdateSearchResult();
        }

        void SearchTimerCallback(object state) {
            if (Equals(previousFragment, Fragment)) return;
            SetHasContentState(null);
            LoadContent();
        }

        void UpdateSearchResult() {
            Device.BeginInvokeOnMainThread(() => SearchResult = !string.IsNullOrEmpty(Fragment) && !HasContent() ? UnableToLocateTickerPlaceholderText : string.Empty);
        }

        protected override void OnPresent() { }
    }

    public abstract class SymbolSearchViewModel : ItemSearchViewModel<SearchListSymbolItem> {
        bool isInEditProcess;
        ILocalStorage localStorage;
        IViewModelFactory factory;

        protected SymbolSearchViewModel(IViewModelFactory factory, ILocalStorage localStorage, ISymbolRepository listSymbolRepository, INavigationService navigationService) : base(listSymbolRepository, navigationService) {
            this.factory = factory;
            this.localStorage = localStorage;
        }

        protected async override Task<IList<SearchListSymbolItem>> FindSymbols(CancellationToken token) {
            IEnumerable<Symbol> symbols = await ListSymbolRepository.FindByFragmentAsync(Fragment, token);
            return symbols
                .Select(s => new SearchListSymbolItem(s, IsTickerInWatchList(s.Ticker), OpenSymbolDetail))
                .ToList();
        }

        protected bool IsTickerInWatchList(string ticker) {
            return localStorage.Tickers.Contains(ticker);
        }
        protected void AddToWatchlist(SearchListSymbolItem listSymbol) {
            localStorage.AddTicker(listSymbol.Ticker);
            listSymbol.IsInWatchlist = true;
        }
        protected void RemoveFromWatchlist(SearchListSymbolItem listSymbol) {
            localStorage.RemoveTicker(listSymbol.Ticker);
            listSymbol.IsInWatchlist = false;
        }

        protected void OpenSymbolDetail(Symbol listSymbol) {
            OpenSymbolDetailAsync(listSymbol).InvokeAndForgetSafeAsync(this);
        }

        protected async Task OpenSymbolDetailAsync(Symbol listSymbol) {
            if (isInEditProcess) return;
            isInEditProcess = true;
            SymbolViewModel symbolViewModel = factory?.CreateSymbolDetailViewModel(listSymbol.Ticker);
            await NavigationService.NavigateTo(symbolViewModel);
            isInEditProcess = false;
        }
    }

    public class WatchListSymbolSearchViewModel : SymbolSearchViewModel {
        public WatchListSymbolSearchViewModel(IViewModelFactory factory, ILocalStorage localStorage, ISymbolRepository listSymbolRepository, INavigationService navigationService)
            : base(factory, localStorage, listSymbolRepository, navigationService) {
        }

        protected override Task ExecuteSelectSymbolCommand(SearchListSymbolItem selectedItem) {
            if (IsTickerInWatchList(selectedItem.Ticker))
                RemoveFromWatchlist(selectedItem);
            else
                AddToWatchlist(selectedItem);
            return Task.CompletedTask;
        }
    }

    public class MarketSymbolSearchViewModel : SymbolSearchViewModel {
        public MarketSymbolSearchViewModel(IViewModelFactory factory, ILocalStorage localStorage, ISymbolRepository listSymbolRepository, INavigationService navigationService)
            : base(factory, localStorage, listSymbolRepository, navigationService) {
        }

        protected async override Task ExecuteSelectSymbolCommand(SearchListSymbolItem selectedItem) {
            await OpenSymbolDetailAsync(selectedItem.Model);
        }
    }

    public class TickerSearchViewModel : ItemSearchViewModel<ListSymbolWrapper> {
        readonly Action<string> tickerSelectedHandle;

        public TickerSearchViewModel(Action<string> tickerSelectedHandle, ISymbolRepository listSymbolRepository, INavigationService navigationService) : base(listSymbolRepository, navigationService) {
            this.tickerSelectedHandle = tickerSelectedHandle;
        }

        protected async override Task ExecuteSelectSymbolCommand(ListSymbolWrapper selectedItem) {
            this.tickerSelectedHandle?.Invoke(selectedItem.Ticker);
            await NavigationService.NavigateBack();
        }

        protected override async Task<IList<ListSymbolWrapper>> FindSymbols(CancellationToken token) {
            IEnumerable<Symbol> listSymbols = await ListSymbolRepository.FindByFragmentAsync(Fragment, token);
            return listSymbols
                .Select(s => new ListSymbolWrapper(s))
                .ToList();
        }
    }
}

