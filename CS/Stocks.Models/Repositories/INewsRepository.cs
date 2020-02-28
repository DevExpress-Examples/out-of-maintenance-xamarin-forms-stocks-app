using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stocks.Models;

namespace Stocks.Models {
    public interface INewsRepository {
        public const int DefaultNewsCount = 10;

        Task<IEnumerable<NewsItem>> GetAsync(string ticker, int count, CancellationToken cancellationToken);
        Task<IEnumerable<NewsItem>> GetAsync(IEnumerable<string> tickers, int perTickerCount, CancellationToken cancellationToken);
    }
}
