using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public partial class SymbolDetailItem : ContentView {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(SymbolDetailItem), String.Empty);
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(object), typeof(SymbolDetailItem), null);

        public string Title {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public object Value {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public SymbolDetailItem() {
            InitializeComponent();
        }
    }
}
