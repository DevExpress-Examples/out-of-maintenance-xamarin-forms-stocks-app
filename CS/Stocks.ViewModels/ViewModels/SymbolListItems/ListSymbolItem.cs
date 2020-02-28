using Stocks.Models;

namespace Stocks.ViewModels {
    public class ListSymbolItem : ListSymbolWrapper {
        public SymbolListType ListType { get; }
        
        public ListSymbolItem(Symbol listSymbol, SymbolListType type) : base (listSymbol) {
            ListType = type;
        }
    }
}
