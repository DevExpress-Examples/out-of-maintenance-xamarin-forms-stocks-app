using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public partial class EmptyNewsListView : ContentView {
        public static readonly BindableProperty MessageProperty = BindableProperty.Create(nameof(Message), typeof(String), typeof(EmptyNewsListView), null, propertyChanged: OnMessagePropertyChanged);

        public string Message {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public EmptyNewsListView() {
            InitializeComponent();
        }

        void OnMessageChanged() {
            messageLabel.Text = Message;
        }

        static void OnMessagePropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is EmptyNewsListView emptyNewsListView)) return;
            emptyNewsListView.OnMessageChanged();
        }
    }
}
