using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stocks.Models;
using Stocks.Services;

namespace Stocks.ViewModels {
    public class MarketViewModel: DetailedListViewModel<ListSymbolItem> {
        ISymbolRepository repository;

        public override string Title => "Market";
        
        public MarketViewModel(ISymbolRepository repository, IViewModelFactory factory, INavigationService navigationService) : base(factory, navigationService) {
            this.repository = repository;
        }

        protected override async Task<IList<ListSymbolItem>> SendRequestAsync(CancellationToken token) {
            IDictionary<SymbolListType, IEnumerable<Symbol>> newSymbols = await repository.GetMarketConditionsAsync(token);
            List<ListSymbolItem> newItems = new List<ListSymbolItem>();
            foreach (SymbolListType type in newSymbols.Keys) {
                foreach (Symbol listSymbol in newSymbols[type]) {
                    newItems.Add(new ListSymbolItem(listSymbol, type));
                }
            }
            return newItems;
        }

        protected override SymbolSearchViewModel CreateSymbolSearchViewModel() {
            return Factory.CreateMarketPageSymbolSearchViewModel();
        }
    }
}