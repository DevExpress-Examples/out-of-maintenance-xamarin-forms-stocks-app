using System.ComponentModel;
using Xamarin.Forms;
using DevExpress.XamarinForms.Navigation;
using Stocks.UI.Services;
using Stocks.ViewModels;

namespace Stocks.UI.Views {
    [DesignTimeVisible(false)]
    public partial class SymbolPage : TabPage {
        SymbolViewModel ViewModel => BindingContext as SymbolViewModel;

        public SymbolPage() {
            InitializeComponent();
        }

        protected override void OnAppearing() {
            base.OnAppearing();
            ViewModel.DialogService = new DialogService(this, "TransactionDialog");
            // The XF calls OnDisappearing on Android before MC sends the DeviceUnlocked message to the instance.
            // So the object has to call VM.OnPresent() by himself.
            ViewModel?.Present();
            MessagingCenter.Instance.Subscribe<object>(this, Constants.DeviceLocked, (sender) => {
                ViewModel?.Hide();
            });
            MessagingCenter.Instance.Subscribe<object>(this, Constants.DeviceUnlocked, (sender) => {
                ViewModel?.Present();
            });
        }

        protected override void OnDisappearing() {
            base.OnDisappearing();
            // The XF on Android calls OnDisappearing on Android before MC sends the DeviceLocked message to the instance.
            // So the object has to call VM.OnHide() by himself.
            ViewModel?.Hide();
            MessagingCenter.Instance.Unsubscribe<object>(this, Constants.DeviceLocked);
            MessagingCenter.Instance.Unsubscribe<object>(this, Constants.DeviceUnlocked);
        }
    }
}