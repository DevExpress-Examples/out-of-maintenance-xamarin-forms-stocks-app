using System;

using Xamarin.Forms;

namespace Stocks.ViewModels {
    public class DialogViewModel : NotificationObject {
        public bool IsSubmitted { get; protected set; } = false;
    }
}

