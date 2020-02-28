using System;
namespace Stocks.Models {
    public static class MathUtils {
        public const double Epsilon = 1e-9;

        public static bool ApproximatelyEquals(this double first, double second) {
            return Math.Abs(first - second) < Epsilon;
        }
        public static bool IsZero(this double value) {
            return Math.Abs(value) < Epsilon;
        }
        public static bool IsNotZero(this double value) {
            return Math.Abs(value) > Epsilon;
        }
    }
}
