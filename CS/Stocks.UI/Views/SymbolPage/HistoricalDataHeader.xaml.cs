namespace Stocks.UI.Views {
    public partial class HistoricalDataHeader : ValueDifferenceView {
        public HistoricalDataHeader() {
            InitializeComponent();
        }

        protected override void OnActualForegroundColorChanged() {
            base.OnActualForegroundColorChanged();
            subtitleLabel.TextColor = ActualForegroundColor;
        }
    }
}
