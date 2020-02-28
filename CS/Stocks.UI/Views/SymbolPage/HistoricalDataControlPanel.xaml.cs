using System;

namespace Stocks.UI.Views {
    public partial class HistoricalDataControlPanel : DXDialogContentView {
        public HistoricalDataControlPanel() {
            InitializeComponent();
        }

        void OnClicked(object sender, EventArgs args) {
            NotifyDialogClosed();
        }
    }
}
