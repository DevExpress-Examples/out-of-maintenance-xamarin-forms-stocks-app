using System;
using Stocks.ViewModels;

namespace Stocks.UI.Views {
    public partial class DisclaimerPage : PopupPage {
        public DisclaimerPage() {
            InitializeComponent();
        }

        void OnOkClicked(object sender, EventArgs eventArgs) {
            this.CloseAsync();
        }
    }
}
