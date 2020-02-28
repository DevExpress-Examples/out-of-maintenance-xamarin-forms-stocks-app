using System;
using System.Threading.Tasks;
using DevExpress.Data;
using Stocks.Models.Utils;
using Stocks.UI.Services;
using Stocks.ViewModels;

namespace Stocks.UI.Views {
    public partial class WatchlistPage : InitializablePage, IErrorHandler {
        const string SortTitle = "Sort by";
        const string CancelAction = "Cancel";
        
        const string NoneAction = "None";
        const string ByTickerAction = "Ticker";
        const string ByTickerDescendingAction = "Ticker (Descending)";
        const string ByProfitabilityAction = "Profitability";
        const string ByProfitabilityDescendingAction = "Profitability (Descending)";

        public WatchlistPage() {
            InitializeComponent();
        }

        public void HandleError(Exception exception) {
            // RK: In general, the error cannot be here.
        }

        protected override void OnAppearing() {
            base.OnAppearing();
            if (BindingContext is WatchlistViewModel viewModel) {
                viewModel.Inject(new DialogService(this, "TransactionDialog"));
            }
        }

        void OnSortClicked(object sender, EventArgs args) {
            SortSymbols().InvokeAndForgetSafeAsync(this);
        }

        async Task SortSymbols() {
            string action = await DisplayActionSheet(SortTitle, CancelAction, null, NoneAction, ByTickerAction, ByTickerDescendingAction, ByProfitabilityAction, ByProfitabilityDescendingAction);
            switch (action) {
                case NoneAction:
                    contentColumn.SortOrder = ColumnSortOrder.None;
                    break;
                case ByTickerAction:
                    contentColumn.SortOrder = ColumnSortOrder.None;
                    contentColumn.FieldName = "Ticker";
                    contentColumn.SortOrder = ColumnSortOrder.Ascending;
                    break;
                case ByTickerDescendingAction:
                    contentColumn.SortOrder = ColumnSortOrder.None;
                    contentColumn.FieldName = "Ticker";
                    contentColumn.SortOrder = ColumnSortOrder.Descending;
                    break;
                case ByProfitabilityAction:
                    contentColumn.SortOrder = ColumnSortOrder.None;
                    contentColumn.FieldName = "Change";
                    contentColumn.SortOrder = ColumnSortOrder.Ascending;
                    break;
                case ByProfitabilityDescendingAction:
                    contentColumn.SortOrder = ColumnSortOrder.None;
                    contentColumn.FieldName = "Change";
                    contentColumn.SortOrder = ColumnSortOrder.Descending;
                    break;
                case CancelAction: break;
                default:
                    throw new Exception("Cannot handle the specified action.");
            }
        }
    }
}
