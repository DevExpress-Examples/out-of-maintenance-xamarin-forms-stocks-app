using System;
using Stocks.Models;

namespace Stocks.ViewModels {
    public enum ChangeType {
        Falling,
        None,
        Rising
    }

    public static class ChangeTypeUtils {
        public static ChangeType FromDouble(double change) {
            if (change.IsZero()) return ChangeType.None;
            return (change > 0)
                ? ChangeType.Rising
                : ChangeType.Falling;
        }
    }
}
