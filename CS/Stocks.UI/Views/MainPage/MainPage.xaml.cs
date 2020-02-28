using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DevExpress.XamarinForms.Navigation;
using Stocks.UI.Services;
using Stocks.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using XFPage = Xamarin.Forms.Page;

namespace Stocks.UI.Views {
    public partial class MainPage : DrawerPage, INavigationHost {
        readonly TopLevelPagesNavigationStack navigationStack = new TopLevelPagesNavigationStack();

        const string DrawerContentItemStyleBaseName = "DrawerContentItemStyleBase";
        const string DrawerContentItemStyleName = "DrawerContentItemStyle";

        MainViewModel ViewModel => (MainViewModel)BindingContext;

        public MainPage() {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
        }

        public async Task NavigateTo(XFPage page) {
            await Device.InvokeOnMainThreadAsync(async () => {
                IsDrawerOpened = false;
                await Task.Delay(250);
                MainContent = page;
            });
            navigationStack.Push(MainContent);
        }

        public async Task ReplaceWith(XFPage page) {
            await Device.InvokeOnMainThreadAsync(async () => {
                IsDrawerOpened = false;
                await Task.Delay(250);
                MainContent = page;
            });
            navigationStack.Push(MainContent);
        }

        protected override void OnAppearing() {
            base.OnAppearing();
            ViewModel?.Present();
            MessagingCenter.Instance.Subscribe<object>(this, Constants.ToggleDrawerMessage, (sender) => {
                IsDrawerOpened = !IsDrawerOpened;
            });
            MessagingCenter.Instance.Subscribe<object>(this, Constants.DeviceLocked, (sender) => {
                ViewModel?.Hide();
            });
            MessagingCenter.Instance.Subscribe<object>(this, Constants.DeviceUnlocked, (sender) => {
                ViewModel?.Present();
            });
        }

        protected override void OnDisappearing() {
            base.OnDisappearing();
            ViewModel?.Hide();
            MessagingCenter.Instance.Unsubscribe<object>(this, Constants.ToggleDrawerMessage);
            MessagingCenter.Instance.Unsubscribe<object>(this, Constants.DeviceLocked);
            MessagingCenter.Instance.Unsubscribe<object>(this, Constants.DeviceUnlocked);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            base.OnPropertyChanged(propertyName);
            if (propertyName == Constants.SafeAreaInsetsPropertyName) {
                ApplySafeInsets();
            }
        }

        protected override bool OnBackButtonPressed() {
            if (base.OnBackButtonPressed())
                return true;

            if (IsDrawerOpened) {
                IsDrawerOpened = false;
                return true;
            }

            navigationStack.Pop();
            XFPage prevPage = navigationStack.Peek();
            if (prevPage == null)
                return false;

            SelectChild(prevPage.BindingContext);
            return true;
        }

        void SelectChild(object viewModel) {
            if (!(viewModel is INavigableViewModel newSelectedChild)) return;
            ViewModel.SelectedChild = newSelectedChild;
        }

        void ApplySafeInsets() {
            Thickness safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
            header.Padding = new Thickness(safeInsets.Left, safeInsets.Top, 0, 0);

            Resources.Remove(DrawerContentItemStyleName);
            Style drawerContentItemStyle = new Style(typeof(MenuRow));
            drawerContentItemStyle.BasedOn = (Style)Resources[DrawerContentItemStyleBaseName];
            drawerContentItemStyle.Setters.Add(
                new Setter {
                    Property = PaddingProperty,
                    Value = new Thickness(safeInsets.Left, 0, 0, 0)
                });
            Resources.Add(DrawerContentItemStyleName, drawerContentItemStyle);
        }

        Task<XFPage> INavigationHost.NavigateBack() {
            return Task.FromException<XFPage>(new NotImplementedException());
        }

        XFPage INavigationHost.CurrentPage => MainContent;
    }
}
