using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Stocks.Models;
using Stocks.Services;

namespace Stocks.ViewModels {
    public class SymbolHistoricalPricesViewModel : UpdatableViewModel<IList<HistoricalPrice>>, IDisposable {
        const int ShownPointCount = 40;

        ILocalStorage localStorage;
        IHistoricalPriceRepository historicalPriceRepository;
        string ticker;
        TimeFrame timeFrame;
        bool isCloseOnlyShown = true;
        bool isUpdateLocked = false;
        DateTime rangeStart;
        DateTime rangeEnd;

        public TimeFrame TimeFrame {
            get => timeFrame;
            private set => SetProperty(ref timeFrame, value, onChanged: (_, v) => {
                localStorage.TimeFrame = TimeFrame;
            });
        }
        public bool IsCloseOnlyShown {
            get => isCloseOnlyShown;
            set => SetProperty(ref isCloseOnlyShown, value, onChanged: (_, v) => {
                localStorage.IsCloseOnlyShown = IsCloseOnlyShown;
            });
        }
        public bool IsUpdateLocked {
            get => isUpdateLocked;
            set => SetProperty(ref isUpdateLocked, value);
        }
        public DateTime RangeStart {
            get => rangeStart;
            private set => SetProperty(ref rangeStart, value);
        }
        public DateTime RangeEnd {
            get => rangeEnd;
            private set => SetProperty(ref rangeEnd, value);
        }

        public ICommand SetTimeFrameCommand { get; }

        public SymbolHistoricalPricesViewModel(ILocalStorage localStorage, string ticker, IHistoricalPriceRepository historicalPriceRepository) : base() {
            if (string.IsNullOrEmpty(ticker)) throw new ArgumentException("The specified parameter must not be null or empty.", nameof(ticker));
            this.localStorage = localStorage;
            this.ticker = ticker;
            this.historicalPriceRepository = historicalPriceRepository;
            timeFrame = localStorage.TimeFrame;
            isCloseOnlyShown = localStorage.IsCloseOnlyShown;

            SetTimeFrameCommand = new DelegateCommand<TimeFrame>(ExecuteSetTimeIntervalCommand);
        }

        protected override Task<IList<HistoricalPrice>> SendRequestAsync(CancellationToken token) {
            if ((Content == null) || !Content.Any()) {
                return ReloadHistoricalValues(token);
            }
            return UpdateLastValues(token);
        }
        protected override void SetHasContentState(IList<HistoricalPrice> c) {
            IsUpdateLocked = true;
            if (c == null) {
                base.SetHasContentState(null);
            } else {
                if ((Content == null) || !Content.Any()) {
                    base.SetHasContentState(new ObservableCollection<HistoricalPrice>(c));
                    RaisePropertyChanged(nameof(TimeFrame));
                    if (Content.Any()) {
                        UpdateRangeToStickToEnd();
                    }
                } else {
                    if (c != null && c.Any()) {
                        MergeValues(c);
                    }
                    base.SetHasContentState(Content);
                }
            }
            IsUpdateLocked = false;
        }

        void ExecuteSetTimeIntervalCommand(TimeFrame newValue) {
            timeFrame = newValue;
            localStorage.TimeFrame = TimeFrame;
            SetHasContentState(null);
            RequestUpdate();
        }

        Task<IList<HistoricalPrice>> ReloadHistoricalValues(CancellationToken token) {
            return historicalPriceRepository.GetPricesAsync(ticker, TimeFrame, token);
        }

        Task<IList<HistoricalPrice>> UpdateLastValues(CancellationToken token) {
            return historicalPriceRepository.GetLastPricesAsync(ticker, TimeFrame, Content.Last().Timestamp, token);
        }

        void MergeValues(IList<HistoricalPrice> newPrices) {
            int addStartIndex = 0;
            if (Equals(Content.Last().Timestamp, newPrices.First().Timestamp)) {
                Content[Content.LastIndex()] = newPrices.First();
                addStartIndex = 1;
            }
            for (int i = addStartIndex; i < newPrices.Count; ++i) {
                Content.Add(newPrices[i]);
            }
        }

        void UpdateRangeToStickToEnd() {
            int endIndex = Content.LastIndex();
            int startIndex = Math.Max(endIndex - ShownPointCount, 0);
            RangeStart = Content[startIndex].Timestamp;
            RangeEnd = Content[endIndex].Timestamp;
        }
    }
}
