using Android.Content;

namespace Stocks.UI.Android {
    public static class StocksApp {
        internal static Context Context { get; private set; }

        public static void Init(Context context) {
            Context = context;
        }
    }
}
