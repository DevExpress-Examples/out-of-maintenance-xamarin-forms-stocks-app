using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public partial class TabHeader : SelectableView {
        public TabHeader() {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged() {
            base.OnBindingContextChanged();
        }

        protected override void OnForegroundChanged() {
            base.OnForegroundChanged();
            Color foreground = ActualForegroundColor;
            icon.ForegroundColor = foreground;
            label.TextColor = foreground;
        }
    }
}
