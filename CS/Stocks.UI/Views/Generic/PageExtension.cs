using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public static class PageExtension {
        public static void ShowAsync(this PopupPage popupPage) {
            App.Service?.ShowPopupPageAsync(popupPage);
        }
        public static void CloseAsync(this PopupPage popupPage) {
            App.Service?.ClosePopupPageAsync(popupPage);
        }
        public static async Task DisplayAlertDialog(this ContentPage contentPage, DXDialogContentView element) {
            if (contentPage.IsPlatformEnabled)
                await App.Service.ShowAlertDialogAsync(element);
        }
    }
}
