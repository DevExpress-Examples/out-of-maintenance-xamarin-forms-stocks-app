using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocks.Models {
    public enum EnvironmentType { Local, Development, Production }

    public static class EnvironmentHolder {
        public static bool UseMockData { get; } = true;
    }
}
