using System.Diagnostics;
using System.Threading.Tasks;
using Stocks.UI.Views;
using Xamarin.Forms;

namespace Stocks.UI {
    public abstract class StocksApplicationService {
        protected StocksApplicationService() {
        }

        public abstract Task ShowAlertDialogAsync(DXDialogContentView dialogContentView);
        public abstract Task ShowPopupPageAsync(PopupPage popupPage);
        public abstract Task ClosePopupPageAsync(PopupPage popupPage);

        public void CloseApp() {
            Process currentProcess = Process.GetCurrentProcess();
            currentProcess.CloseMainWindow();
            currentProcess.Close();
        }
    }
}
