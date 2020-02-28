namespace Stocks.UI.Views {
    public partial class PortfolioDataHeader : ValueDifferenceView {
        public PortfolioDataHeader() {
            InitializeComponent();
        }

        protected override void OnActualForegroundColorChanged() {
            base.OnActualForegroundColorChanged();
            totalChangeLabel.TextColor = ActualForegroundColor;
        }
    }
}
