using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stocks.Models {
    public enum TimeFrame {
        M1 = 1,
        M2 = 2,
        M3 = 3,
        M4 = 4,
        M5 = 5,
        M6 = 6,
        M10 = 7,
        M12 = 8,
        M15 = 9,
        M20 = 10,
        M30 = 11,
        H1 = 12,
        H2 = 13,
        H3 = 14,
        H4 = 15,
        D = 16,
        W = 17,
        MN = 18
    }
    public class DailyPriceRequestParams {
        public IEnumerable<DailyPricesRequest> Params { get; set; }
    }

    public class DailyPricesRequest {
        public string Ticker { get; set; }
        public DateTime StartDate { get; set; }

        public DailyPricesRequest() {

        }

        public DailyPricesRequest(string ticker, DateTime startDate) {
            Ticker = ticker;
            StartDate = startDate;
        }
    }

    public interface IHistoricalPriceRepository {
        Task<IList<HistoricalPrice>> GetPricesAsync(string ticker, TimeFrame timeInterval, CancellationToken cancellationToken);
        Task<HistoricalPrice> GetPriceOnDateAsync(string ticker, DateTime date, CancellationToken cancellationToken);
        Task<IDictionary<string, HistoricalPrice>> GetPricesOnDateAsync(IEnumerable<string> ticker, DateTime date, CancellationToken cancellationToken);
        Task<IList<HistoricalPrice>> GetLastPricesAsync(string ticker, TimeFrame timeInterval, DateTime sinceTimestamp, CancellationToken cancellationToken);
        Task<IDictionary<string, IList<HistoricalClosePrice>>> GetDailyPricesAsync(IEnumerable<DailyPricesRequest> dailyPricesRequests, CancellationToken cancellationToken);
        Task<DateTime> GetLastTradeDateAsync(CancellationToken cancellationToken);
    }
}
