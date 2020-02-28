using System;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public class DXDialogContentView : ContentView {
        public event EventHandler DialogClosed;

        public void NotifyDialogClosed() {
            DialogClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
