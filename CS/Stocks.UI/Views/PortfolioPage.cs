using System;

using Xamarin.Forms;

namespace Stocks.UI.Views {
    public class PortfolioPage : ContentPage {
        public PortfolioPage() {
            Content = new StackLayout {
                Children = {
                    new Label { Text = "Portfolio Page" }
                }
            };
        }
    }
}

