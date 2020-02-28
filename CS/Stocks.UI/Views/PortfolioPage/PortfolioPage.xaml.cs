using System.Runtime.CompilerServices;
using Stocks.UI.Services;
using Stocks.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Stocks.UI.Views {
    public partial class PortfolioPage : InitializablePage {
        PortfolioViewModel ViewModel => (PortfolioViewModel)BindingContext;

        public PortfolioPage() {
            InitializeComponent();
        } 
        
        protected override void OnAppearing() {
            base.OnAppearing();
            ViewModel.DialogService = new DialogService(this, "TransactionDialog");
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            base.OnPropertyChanged(propertyName);
            if (propertyName == Constants.SafeAreaInsetsPropertyName) {
                ApplySafeInsets();
            }
        }

        void ApplySafeInsets() {
            Thickness safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
            portfolioView.SafeAreaIndents = safeInsets;
        }
    }
}