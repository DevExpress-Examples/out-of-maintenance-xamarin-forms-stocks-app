using System;
using System.Threading.Tasks;
using Stocks.UI.Services;
using Stocks.ViewModels;
using Xamarin.Forms;

namespace Stocks.UI.Views {
    public class NavigationHostPage : NavigationPage, INavigationHost {
        public NavigationHostPage() : base() {  }
        public NavigationHostPage(Page rootPage) : base(rootPage) {
            Popped += OnPopped;
        }

        private void OnPopped(object sender, NavigationEventArgs e) {
            var poppedVm = e.Page.BindingContext as INavigableViewModel;
            poppedVm?.Hide();

            var currentVm = this.CurrentPage.BindingContext as INavigableViewModel;
            currentVm.Present();
        }

        public Task NavigateTo(Page page) {
            BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"];
            return this.Navigation.PushAsync(page);
        }
        public Task ReplaceWith(Page page) {
            throw new NotSupportedException();
        }
        public Task<Page> NavigateBack() => this.Navigation.PopAsync();
    }
}

