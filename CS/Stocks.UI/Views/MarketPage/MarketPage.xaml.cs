using Stocks.UI.Services;
using Stocks.ViewModels;

namespace Stocks.UI.Views {
    public partial class MarketPage: InitializablePage {
        public MarketPage() {
            InitializeComponent();
        }

        protected override void OnAppearing() {
            base.OnAppearing();
            if (BindingContext is MarketViewModel viewModel) {
                viewModel.Inject(new DialogService(this, "TransactionDialog"));
            }
        }
    }
}