using System;
using System.Collections.Generic;
using Stocks.UI.Utils;
using Stocks.ViewModels;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public partial class EditTransactionPage : ContentPage {
        EditTransactionViewModel ViewModel => (EditTransactionViewModel)BindingContext;

        public EditTransactionPage() {
            InitializeComponent();
        }

        void OnDateRowTapped(object sender, EventArgs args) {
            dateEditor.Focus();
        }
        void OnPriceRowTapped(object sender, EventArgs args) {
            priceEditor.SetCursorInEnd();
        }
        void OnCountRowTapped(object sender, EventArgs args) {
            countEditor.SetCursorInEnd();
        }

        async void OnTransactionEditFinished(object sender, EventArgs args) {
            await Navigation.PopModalAsync();
        }
    }
}
