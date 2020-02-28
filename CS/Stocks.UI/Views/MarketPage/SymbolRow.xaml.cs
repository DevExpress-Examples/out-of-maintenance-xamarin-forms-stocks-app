using Xamarin.Forms;

namespace Stocks.UI.Views {
    public partial class SymbolRow : ValueDifferenceView {
        public SymbolRow() {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged() {
            base.OnBindingContextChanged();
        }

        protected override void OnActualForegroundColorChanged() {
            base.OnActualForegroundColorChanged();
            Color foreground = ActualForegroundColor;
            changeIcon.ForegroundColor = foreground;
            changeLabel.TextColor = foreground;
        }
    }
}
