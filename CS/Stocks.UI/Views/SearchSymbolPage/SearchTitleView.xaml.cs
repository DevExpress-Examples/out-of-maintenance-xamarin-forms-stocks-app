using System;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public partial class SearchTitleView : ContentView {
        public static readonly BindableProperty SearchTextProperty = BindableProperty.Create(nameof(SearchText), typeof(string), typeof(SearchTitleView), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSearchTextPropertyChanged);

        public string SearchText {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public event EventHandler ClearClicked;
        public event EventHandler<TextChangedEventArgs> SearchTextChanged;
        public event EventHandler DataRequested;

        public SearchTitleView() {
            InitializeComponent();
        }

        private void OnSearchFieldCompleted(object sender, EventArgs e) {
            RaiseDataRequested();
        }

        private void OnSearchFieldTextChanged(object sender, TextChangedEventArgs e) {
            SearchText = searchField.Text;
            SearchTextChanged?.Invoke(this, e);
        }

        private void OnSearchFieldFocused(object sender, FocusEventArgs e) {
            if (e.IsFocused) return;
            RaiseDataRequested();
        }

        public void OnAppearing() {
            FocusSearchField();
        }

        public void FocusSearchField() {
            searchField.Focus();
        }

        void OnClearItemClicked(object sender, EventArgs args) {
            SearchText = string.Empty;
            FocusSearchField();
            EventHandler handler = ClearClicked;
            handler?.Invoke(this, EventArgs.Empty);
        }

        void OnSearchTextChanged() {
            searchField.Text = SearchText;
        }

        void RaiseDataRequested() {
            EventHandler handler = DataRequested;
            handler?.Invoke(this, EventArgs.Empty);
        }

        static void OnSearchTextPropertyChanged(BindableObject bindableObject, object oldValue, object newValue) {
            if (!(bindableObject is SearchTitleView titleView)) return;
            titleView.OnSearchTextChanged();
        }
    }
}
