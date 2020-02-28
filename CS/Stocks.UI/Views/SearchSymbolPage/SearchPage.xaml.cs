using System;
using Stocks.ViewModels;

namespace Stocks.UI.Views {
    public partial class SearchPage : InitializablePage {
        ISearchViewModel ViewModel => BindingContext as ISearchViewModel;

        public SearchPage() {
            InitializeComponent();
        }

        protected override void OnAppearing() {
            base.OnAppearing();
            searchBar.FocusSearchField();
        }

        void OnDataRequested(object sender, EventArgs args) {
            ViewModel?.SearchImmediately();
        }
    }
}
