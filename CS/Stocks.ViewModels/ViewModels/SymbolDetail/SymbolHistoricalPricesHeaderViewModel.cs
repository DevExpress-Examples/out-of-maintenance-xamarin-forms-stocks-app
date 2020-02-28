using System;
using System.Threading;
using System.Threading.Tasks;
using Stocks.Models;
using Stocks.Services;
using Xamarin.Forms;

namespace Stocks.ViewModels {
    public class SymbolHistoricalPricesHeaderViewModel : UpdatableViewModel<Symbol> {
        const string LatestPriceTextFormatString = "Latest update: {0:g}";
        const string ChangeTextFormatString = "{0:n2} ({1:p2})";

        string ticker;
        ISymbolRepository listSymbolRepository;


        public string CompanyName => Content?.CompanyName ?? string.Empty;
        public double LatestPrice => Content?.LatestPrice ?? 0;
        public ChangeType ChangeType => (Content != null)
            ? ChangeTypeUtils.FromDouble(Content.Change)
            : ChangeType.None;
        public string ChangeText => Content != null
            ? string.Format(ChangeTextFormatString, Math.Abs(Content.Change), Math.Abs(Content.ChangePercent))
            : string.Empty;
        public string LatestUpdateText => Content != null
            ? string.Format(LatestPriceTextFormatString, Content.LatestUpdate)
            : string.Empty;

        public SymbolHistoricalPricesHeaderViewModel(string ticker, ISymbolRepository listSymbolRepository): base() {
            this.ticker = ticker;
            this.listSymbolRepository = listSymbolRepository;
        }

        protected override async Task<Symbol> SendRequestAsync(CancellationToken token) {
            return await listSymbolRepository.GetSymbolAsync(ticker, token);
        }

        protected override void OnContentChanged(Symbol oldValue, Symbol newValue) {
            base.OnContentChanged(oldValue, newValue);
            RaisePropertyChanged(nameof(CompanyName));
            RaisePropertyChanged(nameof(LatestPrice));
            RaisePropertyChanged(nameof(ChangeType));
            RaisePropertyChanged(nameof(ChangeText));
            RaisePropertyChanged(nameof(LatestUpdateText));
        }
    }
}
