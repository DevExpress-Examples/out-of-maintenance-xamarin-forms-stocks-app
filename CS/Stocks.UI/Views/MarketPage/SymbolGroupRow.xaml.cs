using Xamarin.Forms;

namespace Stocks.UI.Views {
    public partial class SymbolGroupRow : ContentView {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(SymbolGroupRow));
        public static readonly BindableProperty GroupImageSourceProperty = BindableProperty.Create(nameof(GroupImageSource), typeof(string), typeof(SymbolGroupRow));
        public static readonly BindableProperty ShowGroupImageProperty = BindableProperty.Create(nameof(ShowGroupImage), typeof(bool), typeof(SymbolGroupRow));

        public string Text {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string GroupImageSource {
            get => (string)GetValue(GroupImageSourceProperty);
            set => SetValue(GroupImageSourceProperty, value);
        }

        public bool ShowGroupImage {
            get => (bool)GetValue(ShowGroupImageProperty);
            set => SetValue(ShowGroupImageProperty, value);
        }

        public SymbolGroupRow() {
            InitializeComponent();
        }
    }
}
