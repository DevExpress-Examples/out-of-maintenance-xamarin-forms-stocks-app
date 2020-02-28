using DevExpress.XamarinForms.DataGrid;
using Stocks.Models;
using Stocks.ViewModels;
using Xamarin.Forms;

namespace Stocks.UI.Utils {
    public class PortfolioItemTemplateSelector : DataTemplateSelector {
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate CashItemTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) {
            if (!(item is CellData cellData) || !(cellData.Item is PortfolioItemViewModel itemViewModel)) return null;
            return itemViewModel.Ticker == Portfolio.CashKey
                ? CashItemTemplate
                : ItemTemplate;
        }
    }
}

