using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Stocks.Models;
using Stocks.Models.Utils;
using Stocks.Services;
using Stocks.ViewModels.Utils;

namespace Stocks.ViewModels {
    public class PortfolioViewModel : InitializableViewModel<IList<PortfolioItemViewModel>> {
        readonly ILocalStorage localStorage;
        readonly IHistoricalPriceRepository priceRepository;
        readonly ISymbolRepository listSymbolRepository;
        readonly IViewModelFactory viewModelFactory;
        readonly INavigationService navigationService;

        readonly IrrHistoryCalculator irrHistoryCalculator;
        readonly PortfolioStatisticsCalculator statisticsCalculator;

        PortfolioTabItemViewModel selectedTabItem;
        Portfolio portfolio;
        IList<PortfolioTabItemViewModel> charts;

        bool editInProgress;
        double totalValue;
        double totalChange;
        double totalChangePercent;

        public override string Title => "Portfolio";

        public PortfolioTabItemViewModel SelectedChart {
            get => selectedTabItem;
            set => SetProperty(ref selectedTabItem, value, onChanged: (oldV, newV) => {
                if (oldV != null) oldV.IsSelected = false;
                if (newV != null) newV.IsSelected = true;
            });
        }
        public IDialogService DialogService { get; set; }

        public ChangeType ChangeType {
            get { return ChangeTypeUtils.FromDouble(totalChange); }
        }
        public string TotalText => $"${totalValue:n2}";
        public string TotalChangeText => $"{Math.Abs(totalChange):n2} ({Math.Abs(totalChangePercent):p2})";
        public double RowHeight => 95;
        public double HeaderHeight => 25;
        public double RowsHeight => (Content != null) ? HeaderHeight + RowHeight * Content.Count : HeaderHeight;
        public IEnumerable<PortfolioTabItemViewModel> Charts => charts;

        public ICommand AddTransactionOfExistingTickerCommand { get; }
        public ICommand AddTransactionOfNewTickerCommand { get; }
        public ICommand ShowPortfolioItemDetailCommand { get; }
        public ICommand ShowTransactionsLogCommand { get; }
        public ICommand RemovePortfolioItemCommand { get; }

        public PortfolioViewModel(ILocalStorage localStorage, ISymbolRepository listSymbolRepository, IHistoricalPriceRepository priceRepository, IViewModelFactory viewModelFactory, INavigationService navigationService) {
            this.localStorage = localStorage;
            this.priceRepository = priceRepository;
            this.listSymbolRepository = listSymbolRepository;
            this.viewModelFactory = viewModelFactory;
            this.navigationService = navigationService;

            this.irrHistoryCalculator = new IrrHistoryCalculator();
            this.statisticsCalculator = new PortfolioStatisticsCalculator();

            AddTransactionOfNewTickerCommand = new AsyncDelegateCommand(ExecuteAddTransactionOfNewTickerCommand, this);
            AddTransactionOfExistingTickerCommand = new AsyncDelegateCommand<PortfolioItemViewModel>(ExecuteAddTransactionOfExistingTickerCommand, this);
            ShowPortfolioItemDetailCommand = new AsyncDelegateCommand<PortfolioItemViewModel>(ExecuteShowPortfolioItemDetailCommand, this);
            ShowTransactionsLogCommand = new AsyncDelegateCommand(ExecuteShowTransactionsLogCommand, this);
            RemovePortfolioItemCommand = new DelegateCommand<PortfolioItemViewModel>(ExecuteRemovePortfolioItemCommand);

            this.charts = new List<PortfolioTabItemViewModel> {
                new PortfolioTabItemViewModel(PortfolioChartType.Line, OpenChartDetail),
                new PortfolioTabItemViewModel(PortfolioChartType.Donut, OpenChartDetail),
                new PortfolioTabItemViewModel(PortfolioChartType.Bar, OpenChartDetail)
            };
            SelectedChart = charts[0];
        }

        protected override Task<IList<PortfolioItemViewModel>> SendRequestAsync(CancellationToken token) {
            PortfolioStatisticsCache statisticsCache = localStorage.PortfolioStatisticsCache;
            if (statisticsCache.Datestamp < DateTime.Today) {
                return CalculateStatistics(token);
            } else {
                return ReadStatisticsFromCache(statisticsCache, token);
            }
        }

        protected override bool HasContent() => base.HasContent() && Content.Any();

        protected override void OnContentChanged(IList<PortfolioItemViewModel> oldValue, IList<PortfolioItemViewModel> newValue) {
            base.OnContentChanged(oldValue, newValue);
            RaisePropertyChanged(nameof(TotalText));
            RaisePropertyChanged(nameof(TotalChangeText));
            RaisePropertyChanged(nameof(ChangeType));
            RaisePropertyChanged(nameof(RowsHeight));
        }

        void SavePortfolio() {
            localStorage.SavePortfolio();
            localStorage.PortfolioStatisticsCache = PortfolioStatisticsCache.Empty;
        }

        async Task ExecuteAddTransactionOfNewTickerCommand() {
            await ShowAddTickerTransactionDialog();
        }

        async Task ExecuteAddTransactionOfExistingTickerCommand(PortfolioItemViewModel row) { 
            await ShowAddTickerTransactionDialog(row.Ticker);
        }

        void ExecuteRemovePortfolioItemCommand(PortfolioItemViewModel row) {
            portfolio.RemoveAllTransactions(row.Ticker);
            SavePortfolio();
            LoadContent();
        }
        async Task ExecuteShowTransactionsLogCommand() {
            if (editInProgress) return;
            editInProgress = true;
            PortfolioTransactionsViewModel itemDetail = viewModelFactory.CreatePortfolioTransactionsViewModel();
            await navigationService.NavigateTo(itemDetail);
            editInProgress = false;
        }

        async Task ExecuteShowPortfolioItemDetailCommand(PortfolioItemViewModel row) {
            if (editInProgress) return;
            editInProgress = true;
            SymbolViewModel symbolViewModel = viewModelFactory.CreateSymbolDetailViewModel(row.Ticker);
            await navigationService.NavigateTo(symbolViewModel);
            editInProgress = false;
        }

        async Task ShowAddTickerTransactionDialog(string ticker = null) {
            if (editInProgress) return;
            editInProgress = true;
            EditTransactionViewModel dialogViewModel = string.IsNullOrEmpty(ticker)
                ? viewModelFactory.CreateEditTransactionViewModel(portfolio)
                : viewModelFactory.CreateEditTransactionViewModel(ticker, portfolio);
            await DialogService.ShowFullScreenDialog(dialogViewModel);

            editInProgress = false;
            if (!dialogViewModel.IsSubmitted) return;
            Transaction transaction = dialogViewModel.ToTransaction();
            portfolio.AddTransaction(transaction);
            SavePortfolio();

            LoadContent();
        }

        void OpenChartDetail(PortfolioChartViewModel vm) {
            OpenChartDetailAsync(vm).InvokeAndForgetSafeAsync(this);
        }

        async Task OpenChartDetailAsync(PortfolioChartViewModel vm) {
            if (editInProgress) return;
            editInProgress = true;
            await DialogService.ShowFullScreenDialog(vm);
            editInProgress = false;
        }

        async Task<IList<PortfolioItemViewModel>> CalculateStatistics(CancellationToken token) {
            Task<IList<IrrHistoryItem>> irrHistoryTask = irrHistoryCalculator.Calculate(portfolio, priceRepository, token);
            Task<IList<PortfolioItemStatistics>> itemStatisticsTask = statisticsCalculator.Calculate(portfolio, listSymbolRepository, token);
            await Task.WhenAll(irrHistoryTask, itemStatisticsTask);

            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            IList<IrrHistoryItem> irrHistory = irrHistoryTask.Result;
            IList<PortfolioItemStatistics> itemStatistics = itemStatisticsTask.Result;
            localStorage.PortfolioStatisticsCache = new PortfolioStatisticsCache(DateTime.Today, irrHistory, itemStatistics);
            return await UpdateStatistics(irrHistory, itemStatistics);
        }

        Task<IList<PortfolioItemViewModel>> ReadStatisticsFromCache(PortfolioStatisticsCache statisticsCache, CancellationToken token) {
            return UpdateStatistics(statisticsCache.IrrHistory, statisticsCache.ItemStatistics);
        }

        Task<IList<PortfolioItemViewModel>> UpdateStatistics(IList<IrrHistoryItem> irrHistory, IList<PortfolioItemStatistics> itemStatistics) {
            double totalOperationValue = 0;
            totalValue = totalOperationValue;
            totalChange = totalChangePercent = 0;
            foreach (PortfolioItemStatistics statistic in itemStatistics) {
                totalValue += statistic.ActualValue;
                totalOperationValue += statistic.OperationValue;
            }
            totalChange = totalValue - totalOperationValue;
            totalChangePercent = totalOperationValue.IsZero() ? 0 : totalChange / totalOperationValue;

            IList<PortfolioItemViewModel> itemRows = CreateRows(itemStatistics);
            UpdateCharts(irrHistory, itemStatistics);
            localStorage.PortfolioStatisticsCache = new PortfolioStatisticsCache(DateTime.Today, irrHistory, itemStatistics);

            return Task.FromResult(itemRows);
        }

        IList<PortfolioItemViewModel> CreateRows(IList<PortfolioItemStatistics> itemStatistics) {
            return itemStatistics
                .OrderByDescending(s => s.AbsoluteActualValue)
                .Select(s => {
                    return new PortfolioItemViewModel(s);
                }).ToList();
        }

        void UpdateCharts(IList<IrrHistoryItem> irrHistory, IList<PortfolioItemStatistics> itemStatistics) {
            foreach (PortfolioTabItemViewModel tabVM in charts) {
                tabVM.Update(irrHistory, itemStatistics);
            }
        }

        protected override void OnPresent() {
            portfolio = localStorage.Portfolio;
            LoadContent();
        }
    }
}