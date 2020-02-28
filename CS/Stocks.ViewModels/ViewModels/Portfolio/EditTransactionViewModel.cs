using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Stocks.Models;
using Stocks.Models.Utils;
using Stocks.Services;
using Stocks.ViewModels.Utils;

namespace Stocks.ViewModels {
    public class EditTransactionViewModel : DialogViewModel, IErrorHandler, INavigableViewModel {
        public const string AddTransactionTitle = "Add Transaction";
        public const string EditTransactionTitle = "Edit Transaction";

        public const string NonWorkingDayMsg = "You selected a non-working day. This stock cannot be purchased on non-working days.";
        public const string BiddingIsNotStartedYetMsg = "Bidding is not open. You cannot place a bid at this time.";

        readonly object locker = new object();
        Portfolio portfolio;
        IHistoricalPriceRepository historicalPriceRepository;
        INavigationService navigationService;
        IViewModelFactory viewModelFactory;
        CancellationTokenSource cancellationTokenSource;

        bool isSelected;
        bool isError;
        bool editInProgress;
        string ticker;
        DateTime date;
        int count;
        double price;
        TransactionType transactionType;
        bool isPriceLoading;
        bool useMarketPrice;
        string warningMessage;

        public string Title { get; }
        public bool IsPresented {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public bool IsError {
            get => isError;
            set => SetProperty(ref isError, value);
        }

        public string Ticker {
            get => ticker;
            set => SetProperty(ref ticker, value, onChanged: (oldV, newV) => {
                RaisePropertyChanged(nameof(CanApplyTransaction));
                RaisePropertyChanged(nameof(CurrentPosition));
                UpdatePrice();
            });
        }
        public DateTime Date {
            get => date;
            set => SetProperty(ref date, value, onChanged: (oldV, newV) => {
                RaisePropertyChanged(nameof(CanApplyTransaction));
                RaisePropertyChanged(nameof(CurrentPosition));
                UpdatePrice();
            });
        }
        public string Count {
            get => count.ToString();
            set {
                //numeric keyboard allows you to input not only numbers but also the other symbols ( +, -, .)
                string newValue = string.IsNullOrEmpty(value) ? value : new string(value.ToCharArray().Where(x => char.IsDigit(x)).ToArray());
                int newCount;
                if (!int.TryParse(newValue, out newCount)) {
                    newCount = 0;
                }
                //DO NOT USE SetProperty method! (field "count" may be equal "newCount" value, but string property "Count" value may be not equal new string property "Count" value) 
                count = newCount;
                RaisePropertyChanged(nameof(Count));
                RaisePropertyChanged(nameof(TotalValue));
                RaisePropertyChanged(nameof(CanApplyTransaction));
            }
        }

        public string Price {
            get => price.ToString();
            set {
                double newValue;
                if (!double.TryParse(value, out newValue)) {
                    newValue = 0.0;
                }
                SetPrice(newValue, false);
            }
        }

        public bool IsPriceLoading {
            get => isPriceLoading;
            private set => SetProperty(ref isPriceLoading, value);
        }

        public TransactionType TransactionType {
            get => transactionType;
            set => SetProperty(ref transactionType, value);
        }

        public string TotalValue => $"${price * count:n}";
        public string CurrentPosition {
            get {
                if (this.portfolio != null) {
                    PortfolioItemBalance currentBalance = this.portfolio.GetBalance(Ticker, Date, ToTransaction());
                    if (currentBalance != null)
                        return $"{currentBalance.Count}";
                }
                return $"{(int)0}";
            }
        }

        public string WarningMessage {
            get => warningMessage;
            private set => SetProperty(ref warningMessage, value, onChanged: (oldV, newV) => RaisePropertyChanged(nameof(CanApplyTransaction)));
        }

        public bool CanApplyTransaction =>
               !string.IsNullOrEmpty(ticker)
            && (Date != DateTime.MinValue || Date != DateTime.MaxValue)
            && count != 0
            && price.IsNotZero()
            && string.IsNullOrEmpty(WarningMessage);

        public ICommand StartTickerSearchCommand { get; }
        public ICommand ChangeTransactionTypeCommand { get; }
        public ICommand ApplyTransactionCommand { get; }

        public EditTransactionViewModel(IHistoricalPriceRepository historicalPriceRepository, IViewModelFactory viewModelFactory, INavigationService navigationService, Portfolio portfolio)
            : this(string.Empty, DateTime.Today, 0, 0, historicalPriceRepository, viewModelFactory, navigationService, portfolio, AddTransactionTitle) {
            this.useMarketPrice = true;
        }

        public EditTransactionViewModel(string ticker, IHistoricalPriceRepository historicalPriceRepository, IViewModelFactory viewModelFactory, INavigationService navigationService, Portfolio portfolio)
            : this(ticker, DateTime.Today, 0, 0, historicalPriceRepository, viewModelFactory, navigationService, portfolio, AddTransactionTitle) {
            if (string.IsNullOrEmpty(ticker)) throw new ArgumentException("The specified parameter must not be null or empty.", nameof(ticker));
            this.useMarketPrice = true;
        }

        public EditTransactionViewModel(string ticker, DateTime date, int count, double price, IHistoricalPriceRepository historicalPriceRepository, IViewModelFactory viewModelFactory, INavigationService navigationService, Portfolio portfolio)
            : this(ticker, date, count, price, historicalPriceRepository, viewModelFactory, navigationService, portfolio, EditTransactionTitle) {
            if (string.IsNullOrEmpty(ticker)) throw new ArgumentException("The specified parameter must be non-null and not empty.", nameof(ticker));
            if (date == DateTime.MinValue || date == DateTime.MaxValue) throw new ArgumentException("The specified parameter must not be DateTime.MinValue or DateTime.MaxValue.", nameof(date));
            if (count == 0) throw new ArgumentException("The specified parameter must be unequal to 0.", nameof(count));
            if (price.IsZero()) throw new ArgumentException("The specified parameter's absolute value must be more than 0.0000001.", nameof(price));
        }

        EditTransactionViewModel(string ticker, DateTime date, int count, double price, IHistoricalPriceRepository historicalPriceRepository, IViewModelFactory viewModelFactory, INavigationService navigationService, Portfolio portfolio, string title) {
            this.portfolio = portfolio;
            this.navigationService = navigationService;
            this.historicalPriceRepository = historicalPriceRepository;
            this.viewModelFactory = viewModelFactory;

            this.ticker = ticker;
            this.date = date;
            this.count = count == 0 ? 1 : Math.Abs(count);
            this.transactionType = this.count >= 0 ? TransactionType.Buy : TransactionType.Sell;
            this.price = price;

            Title = title;

            StartTickerSearchCommand = new AsyncDelegateCommand(ExecuteStartTickerSearchCommand, null, this);
            ChangeTransactionTypeCommand = new DelegateCommand<TransactionType>(ExecuteChangeTransactionTypeCommand);
            ApplyTransactionCommand = new DelegateCommand(ExecuteApplyTransactionCommand);

            UpdatePriceAndDate();
        }

        public Transaction ToTransaction() {
            return new Transaction(ticker, Date, (TransactionType == TransactionType.Buy ? 1 : -1) * count, price);
        }

        public void HandleError(Exception exception) {
            IsError = true;
            WarningMessage = exception.Message;
        }

        protected void CancelUpdate() {
            if (cancellationTokenSource == null) return;
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }

        async Task ExecuteStartTickerSearchCommand() {
            if (editInProgress) return;
            editInProgress = true;
            TickerSearchViewModel searchViewModel = viewModelFactory.CreateTickerSearchViewModel(SelectTickerCallback);
            await navigationService.NavigateTo(searchViewModel);
            editInProgress = false;
        }

        void ExecuteChangeTransactionTypeCommand(TransactionType ot) {
            TransactionType = ot;
        }

        void ExecuteApplyTransactionCommand() {
            IsSubmitted = true;
        }
        void UpdatePriceAndDate() {
            UpdatePriceAndDateAsync().InvokeAndForgetSafeAsync(this);
        }

        void UpdatePrice() {
            UpdatePriceAsync().InvokeAndForgetSafeAsync(this);
        }

        async Task UpdatePriceAndDateAsync() {
            CancelUpdate();
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            useMarketPrice = price.IsNotZero() ? await CheckUseMarketPrice(cancellationToken) : true;
            if (await UpdateDateAsync(cancellationToken) && useMarketPrice) {
                await UpdatePriceAsync(cancellationToken);
            }
        }

        Task UpdatePriceAsync() {
            CancelUpdate();
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            return UpdatePriceAsync(cancellationToken);
        }

        async Task<bool> UpdateDateAsync(CancellationToken token) {
            if (Date == DateTime.Today) {
                Date = await historicalPriceRepository.GetLastTradeDateAsync(token);
                return Date == DateTime.Today;
            }
            return true;
        }

        async Task UpdatePriceAsync(CancellationToken token) {
            if (!useMarketPrice || string.IsNullOrEmpty(Ticker) || Date == DateTime.MinValue || Date == DateTime.MaxValue) return;

            token.Register(() => {
                IsPriceLoading = false;
            });

            IsPriceLoading = useMarketPrice;
            HistoricalPrice hp = await historicalPriceRepository.GetPriceOnDateAsync(Ticker, Date, token);
            IsError = false;
            IsPriceLoading = false;
            if (hp != null) {
                SetPrice(hp.Close);
                WarningMessage = null;
            } else {
                SetPrice(0.0);
                WarningMessage = (Date == DateTime.Today) ? BiddingIsNotStartedYetMsg : NonWorkingDayMsg;
            }
        }

        void SetPrice(double value, bool isMarketPrice = true) {
            SetProperty(ref price, value, nameof(Price), onChanged: (oldV, newV) => {
                this.useMarketPrice = this.useMarketPrice && isMarketPrice;

                RaisePropertyChanged(nameof(TotalValue));
                RaisePropertyChanged(nameof(CanApplyTransaction));
            });
        }

        void SelectTickerCallback(string t) {
            Ticker = t;
        }

        async Task<bool> CheckUseMarketPrice(CancellationToken token) {
            IsPriceLoading = true;
            HistoricalPrice hp = await historicalPriceRepository.GetPriceOnDateAsync(Ticker, Date, token);
            IsPriceLoading = false;

            if (hp != null) {
                return hp.Close.ApproximatelyEquals(price);
            }
            return true;
        }

        public void Present() {
            lock (locker) {
                IsPresented = true;
            }
        }

        public void Hide() {
            lock (locker) {
                IsPresented = false;
            }
        }
    }
}
