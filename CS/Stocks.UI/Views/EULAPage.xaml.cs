using System;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public partial class EULAPage : ContentPage {
        public EULAPage() {
            InitializeComponent();
        }

        async void OnAcceptClicked(object sender, EventArgs eventArgs) {
            await Navigation.PopModalAsync();
        }
        void OnDeclineClicked(object sender, EventArgs eventArgs) {
            App.Service?.CloseApp();
        }
    }

    public class EULANavigationPage : NavigationPage {
        public EULANavigationPage() : base(new EULAPage()) {
        }

        protected override bool OnBackButtonPressed() {
            return false;
        }
    }
}
