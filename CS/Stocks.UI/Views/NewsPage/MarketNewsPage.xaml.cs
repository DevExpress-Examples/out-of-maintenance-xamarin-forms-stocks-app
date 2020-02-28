using System.ComponentModel;
using Xamarin.Forms;
using Stocks.ViewModels;

namespace Stocks.UI.Views {
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MarketNewsPage : InitializablePage {
        MarketNewsViewModel ViewModel => (MarketNewsViewModel)BindingContext;

        public MarketNewsPage() {
            InitializeComponent();
        }
    }
}