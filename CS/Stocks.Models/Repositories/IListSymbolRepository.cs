using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stocks.Models {
    public interface ISymbolRepository  {
        Task<IDictionary<SymbolListType, IEnumerable<Symbol>>> GetMarketConditionsAsync(CancellationToken cancellationToken);
        Task<Symbol> GetSymbolAsync(string ticker, CancellationToken cancellationToken);
        Task<IEnumerable<Symbol>> GetSymbolsAsync(IEnumerable<string> tickers, CancellationToken cancellationToken);
        Task<IEnumerable<Symbol>> FindByFragmentAsync(string fragment, CancellationToken cancellationToken);
    }
}
