using System.Threading.Tasks;
using System.Windows.Input;
using Stocks.ViewModels.Utils;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public class PortfolioDetailedChartPage : ContentPage {
        public ICommand CloseCommand { get; }

        public PortfolioDetailedChartPage() {
            CloseCommand = new AsyncDelegateCommand(ExecuteCloseCommand);
        }

        async Task ExecuteCloseCommand() {
            await Navigation.PopModalAsync();
        }
    }
}

