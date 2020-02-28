using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public partial class PortfolioView : ContentView {
        Thickness safeAreaIndents;
        public Thickness SafeAreaIndents {
            get => safeAreaIndents;
            set {
                if (value == safeAreaIndents) return;
                safeAreaIndents = value;
                OnSafeAreaIndentsChanged();
            } }

        public PortfolioView() {
            InitializeComponent();
        }

        void OnSafeAreaIndentsChanged() {
            // There is a problem with SafeInsets - when you rotate device it does not return the right inset correctly.
            Thickness contentInsets = new Thickness(SafeAreaIndents.Left, 0);
            header.Padding = contentInsets;
            tabControl.Margin = contentInsets;
            grid.Margin = new Thickness(SafeAreaIndents.Left, 0, SafeAreaIndents.Left, SafeAreaIndents.Bottom);
        }

        void OnScrolled(object sender, ScrolledEventArgs e) {
            if (e.ScrollY < 0) {
                filler.HeightRequest = Math.Abs(e.ScrollY);
            }
        }
    }
}
