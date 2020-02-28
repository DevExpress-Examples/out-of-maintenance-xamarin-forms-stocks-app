using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Stocks.ViewModels {
    public static class ListUtils {
        public static int LastIndex<T>(this IReadOnlyList<T> collection) {
            return collection.Count - 1;
        }
        public static int LastIndex<T>(this IList<T> collection) {
            return collection.Count - 1;
        }
        public static void RemoveFirst<T>(this IList<T> collection) {
            collection.RemoveAt(0);
        }
        public static void RemoveLast<T>(this IList<T> collection) {
            collection.RemoveAt(collection.LastIndex());
        }
    }
}
