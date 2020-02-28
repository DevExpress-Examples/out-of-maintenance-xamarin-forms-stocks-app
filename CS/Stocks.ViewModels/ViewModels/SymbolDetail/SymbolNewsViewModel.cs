using System.Collections.Generic;
using Stocks.Models;
using Stocks.Services;

namespace Stocks.ViewModels {
    public class SymbolNewsViewModel: NewsListViewModel {
        string ticker;
        
        public override string Title => "News";
        
        public SymbolNewsViewModel(string ticker, INewsRepository newsRepository, INavigationService navigationService): base(newsRepository, navigationService) {
            this.ticker = ticker;
        }

        protected override IEnumerable<string> GetTargetTickers() => new List<string> { ticker };
    }
}
