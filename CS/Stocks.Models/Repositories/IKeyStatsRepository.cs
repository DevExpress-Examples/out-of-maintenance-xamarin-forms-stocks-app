using System.Threading;
using System.Threading.Tasks;

namespace Stocks.Models {
    public interface ISymbolKeyStatsRepository {
        Task<SymbolKeyStats> GetAsync(string ticker, CancellationToken cancellationToken);
    }
}
