using System;
using Xamarin.Forms;

namespace Stocks.ViewModels {
    static class PaletteLoader {
        public static Color[] LoadPalette(params string[] values) {
            Color[] colors = new Color[values.Length];
            for (int i = 0; i < values.Length; i++)
                colors[i] = Color.FromHex(values[i]);
            return colors;
        }
    }

    public static class Palettes {
        static readonly Color[] extended = PaletteLoader.LoadPalette("#FF42A5F5", "#FFFF5252", "#FF4CAF50", "#FFFFAB40", "#FFBDBDBD",
                                                                     "#FF536DFE", "#FF009688", "#FFE91E63", "#FFFF6E40", "#FF9C27B0");
        public static Color[] Extended => extended;
    }
}
