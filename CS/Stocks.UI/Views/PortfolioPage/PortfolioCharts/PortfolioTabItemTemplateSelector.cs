using System;
using Stocks.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stocks.UI.Utils {
    public class PortfolioTabItemTemplateSelector : DataTemplateSelector {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) {
            if (!(item is PortfolioTabItemViewModel chartContainer))
                return null;

            switch (chartContainer.ChartType) {
                case PortfolioChartType.Line: return LineChartTemplate;
                case PortfolioChartType.Donut: return DonutChartTemplate;
                case PortfolioChartType.Bar: return BarChartTemplate;
                default: return null;
            }
        }

        public DataTemplate LineChartTemplate { get; set; }
        public DataTemplate DonutChartTemplate { get; set; }
        public DataTemplate BarChartTemplate { get; set; }
    }

    class PortfolioTabItemTemplateSelectorExtension : IMarkupExtension<PortfolioTabItemTemplateSelector> {
        public DataTemplate LineChartTemplate { get; set; }
        public DataTemplate DonutChartTemplate { get; set; }
        public DataTemplate BarChartTemplate { get; set; }

        public PortfolioTabItemTemplateSelector ProvideValue(IServiceProvider serviceProvider) =>
            new PortfolioTabItemTemplateSelector() { LineChartTemplate = LineChartTemplate, DonutChartTemplate = DonutChartTemplate, BarChartTemplate = BarChartTemplate };
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }
}