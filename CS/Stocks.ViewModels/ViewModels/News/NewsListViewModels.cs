using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Stocks.Models;
using Stocks.Services;
using Stocks.ViewModels.Utils;

namespace Stocks.ViewModels {
    public abstract class NewsListViewModel: InitializableViewModel<IList<NewsItemViewModel>> {
        INavigationService navigationService;
        INewsRepository newsRepository { get; }

        public ICommand OpenNewsItemCommand { get; }

        public NewsListViewModel(INewsRepository newsRepository, INavigationService navigationService) {
            this.newsRepository = newsRepository;
            this.navigationService = navigationService;
            OpenNewsItemCommand = new AsyncDelegateCommand<NewsItemViewModel>(OpenNews, this);
        }

        protected override async Task<IList<NewsItemViewModel>> SendRequestAsync(CancellationToken token) {
            IEnumerable<string> tickers = GetTargetTickers();
            if (tickers.Any()) {
                return (await newsRepository.GetAsync(tickers, INewsRepository.DefaultNewsCount, token))
                    .Select(n => new NewsItemViewModel(n))
                    .ToList();
            }
            return new List<NewsItemViewModel>();
        }
        protected override bool HasContent() => base.HasContent() && Content.Any();

        protected abstract IEnumerable<string> GetTargetTickers();

        async Task OpenNews(NewsItemViewModel newsItem) {
            await navigationService.NavigateTo(newsItem);
        }
    }

    public class MarketNewsViewModel : NewsListViewModel {
        ILocalStorage localStorage;

        public override string Title => "News";

        public MarketNewsViewModel(ILocalStorage localStorage, INewsRepository newsRepository, INavigationService navigationService): base(newsRepository, navigationService) {
            this.localStorage = localStorage;
        }

        protected override IEnumerable<string> GetTargetTickers() {
            IEnumerable<string> watchlistTickers = localStorage.Tickers;
            IEnumerable<string> portfolioTickers = localStorage.Portfolio.Tickers;
            return watchlistTickers.Union(portfolioTickers).ToList();
        }
    }
}
