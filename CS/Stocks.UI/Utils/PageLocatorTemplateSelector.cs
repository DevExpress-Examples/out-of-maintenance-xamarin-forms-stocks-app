using System;
using Stocks.UI.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stocks.UI.Utils {
    public class PageLocatorTemplateSelector : DataTemplateSelector {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) {
            return new DataTemplate(() => {
                Page page = PageLocator.Instance.GetPage(item);
                if (page != null) {
                    page.BindingContext = item;
                }
                return page;
            });
        }
    }
    public class PageLocatorTemplateSelectorExtension : IMarkupExtension<PageLocatorTemplateSelector> {
        public PageLocatorTemplateSelector ProvideValue(IServiceProvider serviceProvider) => new PageLocatorTemplateSelector();
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }
}

