using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Stocks.Models;
using Stocks.Services;
using Stocks.ViewModels.Utils;

namespace Stocks.ViewModels {
    public class PortfolioTransactionsViewModel : InitializableViewModel<IList<TransactionViewModel>> {
        readonly IViewModelFactory viewModelFactory;
        readonly ILocalStorage localStorage;
        readonly Portfolio portfolio;

        bool isGroupedByDate;
        bool isGroupedByTicker;
        bool isGroupedByType;

        public override string Title => "Manage Transactions";
        public bool IsGroupedByDate { get => isGroupedByDate; set => SetProperty(ref isGroupedByDate, value); }
        public bool IsGroupedByTicker { get => isGroupedByTicker; set => SetProperty(ref isGroupedByTicker, value); }
        public bool IsGroupedByType { get => isGroupedByType; set => SetProperty(ref isGroupedByType, value); }
        public bool IsEmpty => Content == null || !Content.Any();

        public IDialogService DialogService { get; set; }

        public ICommand AddTransactionCommand { get; }
        public ICommand EditTransactionCommand { get; }
        public ICommand RemoveTransactionCommand { get; }

        public PortfolioTransactionsViewModel(ILocalStorage localStorage, IViewModelFactory viewModelFactory) {
            this.localStorage = localStorage;
            this.viewModelFactory = viewModelFactory;
            this.portfolio = localStorage.Portfolio;

            AddTransactionCommand = new AsyncDelegateCommand(ExecuteAddTransactionCommand, this);
            EditTransactionCommand = new AsyncDelegateCommand<TransactionViewModel>(ExecuteEditTransactionCommand, this);
            RemoveTransactionCommand = new DelegateCommand<TransactionViewModel>(ExecuteRemoveTransactionCommand);
        }
        
        protected override Task<IList<TransactionViewModel>> SendRequestAsync(CancellationToken token) {
            return GetRows();
        }
        protected override bool HasContent() => base.HasContent() && Content.Any();

        protected override void OnContentChanged(IList<TransactionViewModel> oldValue, IList<TransactionViewModel> newValue) {
            base.OnContentChanged(oldValue, newValue);
            RaisePropertyChanged(nameof(IsEmpty));
        }

        async Task ExecuteAddTransactionCommand() {
            EditTransactionViewModel editTransactionViewModel = viewModelFactory.CreateEditTransactionViewModel(portfolio);
            await DialogService.ShowFullScreenDialog(editTransactionViewModel);

            if (!editTransactionViewModel.IsSubmitted) return;
            Transaction transaction = editTransactionViewModel.ToTransaction();
            portfolio.AddTransaction(transaction);
            SavePortfolio();
            LoadContent();
        }

        async Task ExecuteEditTransactionCommand(TransactionViewModel transaction) {
            EditTransactionViewModel editTransactionViewModel = viewModelFactory.CreateEditTransactionViewModel(transaction.Transaction, portfolio);
            await DialogService.ShowFullScreenDialog(editTransactionViewModel);

            if (!editTransactionViewModel.IsSubmitted) return;
            Transaction newTransaction = editTransactionViewModel.ToTransaction();
            portfolio.ReplaceTransaction(transaction.Transaction, newTransaction);
            SavePortfolio();
            LoadContent();
        }

        void ExecuteRemoveTransactionCommand(TransactionViewModel transaction) {
            portfolio.RemoveTransaction(transaction.Transaction);
            SavePortfolio();
            LoadContent();
        }

        void SavePortfolio() {
            localStorage.SavePortfolio();
            localStorage.PortfolioStatisticsCache = PortfolioStatisticsCache.Empty;
        }

        Task<IList<TransactionViewModel>> GetRows() {
            IList<TransactionViewModel> result = portfolio.Transactions
                .Select(t => new TransactionViewModel(t))
                .ToList();
            return Task.FromResult(result);
        }
    }
}
