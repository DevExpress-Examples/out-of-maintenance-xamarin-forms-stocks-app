using System;
using System.Collections.Generic;
using System.Linq;
using Stocks.Models;

namespace Stocks.ViewModels {

    public class NewsItemViewModel: BaseViewModel, INavigableViewModel {
        const string timeFormat = "{0:G}";

        NewsItem newsItem;
        
        public override string Title => newsItem.Headline;

        public string Timestamp { get; }
        public string Source => newsItem.Source;
        public string Headline => newsItem.Headline;
        public string Url => newsItem.Url;
        public IEnumerable<string> RelatedTickers { get; }

        public NewsItemViewModel(NewsItem newsItem) {
            this.newsItem = newsItem;
            Timestamp = string.Format(timeFormat, newsItem.Timestamp);
            RelatedTickers = newsItem.RelatedTickers.Split(',').Select(s => s.ToUpper()).ToList();
        }

        public override bool Equals(object obj) {
            if (!(obj is NewsItemViewModel vm)) return false;
            return Source == vm.Source
                && Timestamp == vm.Timestamp
                && Headline == vm.Headline
                && Url == vm.Url
                && RelatedTickers.SequenceEqual(vm.RelatedTickers);
        }

        public override int GetHashCode() {
            var hashCode = -1789386607;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Timestamp);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Source);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Headline);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Url);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<string>>.Default.GetHashCode(RelatedTickers);
            return hashCode;
        }
    }
}
