using System;
using System.Collections;
using System.Collections.Specialized;
using DevExpress.XamarinForms.Core.Internal;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public class InlineItemsView: StackLayout {
        public static readonly BindableProperty ItemsSourceProperty = BindingUtils.Instance.CreateBindableProperty<InlineItemsView, IEnumerable>(v => v.ItemsSource, propertyChanged: OnItemsChanged);
        public static readonly BindableProperty ItemTemplateProperty = BindingUtils.Instance.CreateBindableProperty<InlineItemsView, DataTemplate>(v => v.ItemTemplate, propertyChanged: OnTemplateChanged);

        public IEnumerable ItemsSource {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        public DataTemplate ItemTemplate {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public InlineItemsView() {
            Orientation = StackOrientation.Vertical;
        }

        void ReloadItemViews() {
            Children.Clear();
            // Maybe there are some constants for default size requests.
            this.WidthRequest = -1;
            this.HeightRequest = -1;
            if (ItemsSource == null) return;
            DataTemplate template = ItemTemplate;
            if (template != null) {
                if (template is DataTemplateSelector templateSelector) {
                    foreach (object item in ItemsSource) {
                        var content = templateSelector.SelectTemplate(item, this)?.CreateContent();
                        if (content is View view) {
                            view.BindingContext = item;
                            Children.Add(view);
                        }
                    }
                } else {
                    foreach (object item in ItemsSource) {
                        var content = template.CreateContent();
                        if (content is View view) {
                            view.BindingContext = item;
                            Children.Add(view);
                        }
                    }
                }
                
            } else {
                foreach (object item in ItemsSource) {
                    Children.Add(new Label { Text = item.ToString() });
                }
            }
        }

        void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
            ReloadItemViews();
        }

        static void OnItemsChanged(BindableObject obj, object oldValue, object newValue) {
            if (!(obj is InlineItemsView view)) return;
            if (oldValue is INotifyCollectionChanged changableOldCollection) {
                changableOldCollection.CollectionChanged -= view.OnItemsCollectionChanged;
            }
            if (newValue is INotifyCollectionChanged changableNewCollection) {
                changableNewCollection.CollectionChanged += view.OnItemsCollectionChanged;
            }
            view.ReloadItemViews();
        }

        static void OnTemplateChanged(BindableObject obj, object oldValue, object newValue) {
            if (!(obj is InlineItemsView view)) return;
            view.ReloadItemViews();
        }
    }
}
