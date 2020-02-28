using System;
using System.Threading;
using System.Threading.Tasks;
using Stocks.Models;

namespace Stocks.ViewModels {
    public class SymbolDetailViewModel : InitializableViewModel<SymbolKeyStats> {
        const string NoValuePlaceholder = "-";

        string ticker;
        ISymbolKeyStatsRepository keyStatsRepository;

        public override string Title => "Detail";
        
        public string Week52Range => Content != null
            ? $"${Content.Week52Low:n} - ${Content.Week52High:n}"
            : NoValuePlaceholder;

        public string MarketCapitalization => Content != null
            ? $"${Content.MarketCapitalization:n}"
            : NoValuePlaceholder;

        public string SharesOutstanding => Content != null
            ? $"${Content.SharesOutstanding:n}"
            : NoValuePlaceholder;

        public string Float => Content != null
            ? $"${Content.Float:n}"
            : NoValuePlaceholder;

        public string Average30DayVolume => Content != null
            ? $"${Content.Average30DayVolume:n}"
            : NoValuePlaceholder;

        public string Average10DayVolume => Content != null
            ? $"${Content.Average10DayVolume:n}"
            : NoValuePlaceholder;

        public string Employees => Content != null
            ? $"{Content.Employees:n0}"
            : NoValuePlaceholder;

        public string Trailing12MonthEarnings => Content != null
            ? $"{Content.Trailing12MonthEarnings:n}"
            : NoValuePlaceholder;

        public string Trailing12MonthDividendRate => Content != null
            ? $"{Content.Trailing12MonthDividendRate:p}"
            : NoValuePlaceholder;

        public string DividentYield => Content != null
            ? $"{Content.DividentYield:p}"
            : NoValuePlaceholder;

        public string NextDividentDate => (Content == null || Content.NextDividentDate == DateTime.MinValue)
            ? NoValuePlaceholder
            : $"{Content.NextDividentDate:d}";

        public string ExDividentDate => (Content == null || Content.ExDividentDate == DateTime.MinValue)
            ? NoValuePlaceholder
            : $"{Content.ExDividentDate:d}";

        public string NextEarningsDate => (Content == null || Content.NextEarningsDate == DateTime.MinValue)
            ? NoValuePlaceholder
            : $"{Content.NextEarningsDate:d}";

        public string PriceToEarningsRatio => Content != null
            ? $"{Content.PriceToEarningsRatio:n3}"
            : NoValuePlaceholder;

        public string Beta => Content != null
            ? $"{Content.Beta:n3}"
            : NoValuePlaceholder;

        public string Day200MovingAverage => Content != null
            ? $"{Content.Day200MovingAverage:n2}"
            : NoValuePlaceholder;

        public string Day50MovingAverage => Content != null
            ? $"{Content.Day50MovingAverage:n2}"
            : NoValuePlaceholder;

        public string MaxChangePercent => Content != null
            ? $"{Content.MaxChangePercent:p2}"
            : NoValuePlaceholder;

        public string Year5ChangePercent => Content != null
            ? $"{Content.Year5ChangePercent:p2}"
            : NoValuePlaceholder;

        public string Year2ChangePercent => Content != null
            ? $"{Content.Year2ChangePercent:p2}"
            : NoValuePlaceholder;

        public string Year1ChangePercent => Content != null
            ? $"{Content.Year1ChangePercent:p2}"
            : NoValuePlaceholder;

        public string YtdChangePercent => Content != null
            ? $"{Content.YtdChangePercent:p2}"
            : NoValuePlaceholder;

        public string Month6ChangePercent => Content != null
            ? $"{Content.Month6ChangePercent:p2}"
            : NoValuePlaceholder;

        public string Month3ChangePercent => Content != null
            ? $"{Content.Month3ChangePercent:p2}"
            : NoValuePlaceholder;

        public string Month1ChangePercent => Content != null
            ? $"{Content.Month1ChangePercent:p2}"
            : NoValuePlaceholder;

        public string Day30ChangePercent => Content != null
            ? $"{Content.Day30ChangePercent:p2}"
            : NoValuePlaceholder;

        public string Day5ChangePercent => Content != null
            ? $"{Content.Day5ChangePercent:p2}"
            : NoValuePlaceholder;

        protected override void OnContentChanged(SymbolKeyStats oldValue, SymbolKeyStats newValue) {
            base.OnContentChanged(oldValue, newValue);
            RaisePropertyChanged(nameof(Week52Range));
            RaisePropertyChanged(nameof(MarketCapitalization));
            RaisePropertyChanged(nameof(SharesOutstanding));
            RaisePropertyChanged(nameof(Float));
            RaisePropertyChanged(nameof(Average30DayVolume));
            RaisePropertyChanged(nameof(Average10DayVolume));
            RaisePropertyChanged(nameof(Employees));
            RaisePropertyChanged(nameof(Trailing12MonthEarnings));
            RaisePropertyChanged(nameof(Trailing12MonthDividendRate));
            RaisePropertyChanged(nameof(DividentYield));
            RaisePropertyChanged(nameof(NextDividentDate));
            RaisePropertyChanged(nameof(ExDividentDate));
            RaisePropertyChanged(nameof(NextEarningsDate));
            RaisePropertyChanged(nameof(PriceToEarningsRatio));
            RaisePropertyChanged(nameof(Beta));
            RaisePropertyChanged(nameof(Day200MovingAverage));
            RaisePropertyChanged(nameof(Day50MovingAverage));
            RaisePropertyChanged(nameof(MaxChangePercent));
            RaisePropertyChanged(nameof(Year5ChangePercent));
            RaisePropertyChanged(nameof(Year2ChangePercent));
            RaisePropertyChanged(nameof(Year1ChangePercent));
            RaisePropertyChanged(nameof(YtdChangePercent));
            RaisePropertyChanged(nameof(Month6ChangePercent));
            RaisePropertyChanged(nameof(Month3ChangePercent));
            RaisePropertyChanged(nameof(Month1ChangePercent));
            RaisePropertyChanged(nameof(Day30ChangePercent));
            RaisePropertyChanged(nameof(Day5ChangePercent));
        }

        public SymbolDetailViewModel(string ticker, ISymbolKeyStatsRepository quoteRepository) : base() {
            if (string.IsNullOrEmpty(ticker)) throw new ArgumentException("The specified ticker must not be null or empty", nameof(ticker));
            this.ticker = ticker;
            this.keyStatsRepository = quoteRepository;
        }

        protected override Task<SymbolKeyStats> SendRequestAsync(CancellationToken token) {
            return keyStatsRepository.GetAsync(ticker, token);            
        }
    }
}
