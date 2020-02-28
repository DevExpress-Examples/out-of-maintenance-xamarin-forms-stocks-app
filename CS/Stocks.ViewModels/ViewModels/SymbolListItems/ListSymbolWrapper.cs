using System;
using Stocks.Models;

namespace Stocks.ViewModels {
    public class ListSymbolWrapper: NotificationObject, ISymbol {
        public string Ticker => Model.Ticker;
        public string CompanyName => Model.CompanyName;
        public double LatestPrice => Model.LatestPrice;
        public double Change => Model.Change;
        public double ChangePercent => Model.ChangePercent;
        public ChangeType ChangeType {
            get {
                if (Model == null) return ChangeType.None;
                return ChangeTypeUtils.FromDouble(Model.Change);
            }
        }
        public string LatestPriceText => $"{Model.LatestPrice:n2}";
        public string ChangeText => $"{Math.Abs(Model.Change):n2} ({Math.Abs(Model.ChangePercent):p2})";
        public Symbol Model { get; }

        public ListSymbolWrapper(Symbol listSymbol) {
            Model = listSymbol;
        }
    }
}
