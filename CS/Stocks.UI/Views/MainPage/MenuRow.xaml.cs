using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public partial class MenuRow : SelectableView {
        public MenuRow() {
            InitializeComponent();
        }

        protected override void OnForegroundChanged() {
            base.OnForegroundChanged();
            Color foreground = ActualForegroundColor;
            icon.ForegroundColor = foreground;
            label.TextColor = foreground;
        }
    }
}
