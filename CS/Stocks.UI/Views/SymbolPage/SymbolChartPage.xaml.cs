using System;
using Stocks.ViewModels;

namespace Stocks.UI.Views {
    public partial class SymbolChartPage : InitializablePage {
        public override bool ShouldHandleDeviceLock => false;

        public SymbolChartPage() {
            InitializeComponent();
        }
        
        async void OnClicked(object sender, EventArgs args) {
            await this.DisplayAlertDialog(new HistoricalDataControlPanel() { BindingContext = ((SymbolChartViewModel)BindingContext).HistoricalValues });
        }
    }
}
