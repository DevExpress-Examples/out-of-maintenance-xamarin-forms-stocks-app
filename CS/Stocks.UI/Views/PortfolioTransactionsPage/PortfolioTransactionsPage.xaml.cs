using System;
using Stocks.UI.Services;
using Stocks.ViewModels;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public partial class PortfolioTransactionsPage : InitializablePage {
        const string DIALOG_TITLE = "Group by";
        const string ACTION_CANCEL = "Cancel";
        const string ACTION_GROUP_BY_NONE = "None";
        const string ACTION_GROUP_BY_DATE = "Date";
        const string ACTION_GROUP_BY_TICKER = "Ticker";
        const string ACTION_GROUP_BY_TYPE = "Type";
        static readonly string[] ACTIONS = { ACTION_GROUP_BY_NONE, ACTION_GROUP_BY_DATE, ACTION_GROUP_BY_TICKER, ACTION_GROUP_BY_TYPE };

        PortfolioTransactionsViewModel ViewModel => (PortfolioTransactionsViewModel)BindingContext;

        public PortfolioTransactionsPage() {
            InitializeComponent();
        }

        protected override void OnAppearing() {
            base.OnAppearing();
            ViewModel.DialogService = new DialogService(this, "TransactionDialog");
        }

        async void OnSetUpGroupClicked(object sender, EventArgs args) {
            string action = await DisplayActionSheet(DIALOG_TITLE, ACTION_CANCEL, null, ACTIONS);
            if (string.IsNullOrEmpty(action))
                return;

            switch (action) {
                case ACTION_GROUP_BY_NONE:
                    ViewModel.IsGroupedByDate = false;
                    ViewModel.IsGroupedByTicker = false;
                    ViewModel.IsGroupedByType = false;
                    return;
                case ACTION_GROUP_BY_DATE:
                    ViewModel.IsGroupedByDate = true;
                    return;
                case ACTION_GROUP_BY_TICKER:
                    ViewModel.IsGroupedByTicker = true;
                    return;
                case ACTION_GROUP_BY_TYPE:
                    ViewModel.IsGroupedByType = true;
                    return;
                default:
                    return;
            }
        }
    }
}
