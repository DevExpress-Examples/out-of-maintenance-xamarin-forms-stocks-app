using System;
using System.Windows.Input;
using Stocks.Models;

namespace Stocks.ViewModels {
    public class SearchListSymbolItem: ListSymbolWrapper {
        bool isInWatchlist;

        public bool IsInWatchlist {
            get => isInWatchlist;
            set => SetProperty(ref isInWatchlist, value);
        }

        public ICommand OpenSymbolDetailCommand { get; }

        public SearchListSymbolItem(Symbol listSymbol, bool isInWatchlist, Action<Symbol> openSymbolDetailHandler) : base(listSymbol) {
            IsInWatchlist = isInWatchlist;
            OpenSymbolDetailCommand = new DelegateCommand(() => {
                if (openSymbolDetailHandler != null) 
                    openSymbolDetailHandler(listSymbol);
            });
        }

        public override string ToString() {
            return $"{Ticker} ({CompanyName}, isInWatchlist: {IsInWatchlist})";
        }
    }
}
