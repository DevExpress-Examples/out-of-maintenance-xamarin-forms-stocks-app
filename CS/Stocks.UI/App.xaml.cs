using System.Reflection;
using Unity;
using Xamarin.Forms;
using DevExpress.XamarinForms.Core.Themes;
using DevExpress.XamarinForms.DemoEditors;
using Stocks.Models;
using Stocks.Services;
using Stocks.UI.Services;
using Stocks.UI.Views;
using Stocks.ViewModels;
using Xamarin.Forms.Xaml;
using Mocks = Stocks.UI.Services.Mocks;

namespace Stocks.UI {
    public partial class App : Application {
        public static StocksApplicationService Service => DependencyService.Get<StocksApplicationService>();

        MainViewModel mainViewModel;

        public App() {
            InitializeComponent();

            IconView.ResourceAssembly = Assembly.GetAssembly(typeof(App));

            NavigationService navigationService = new NavigationService();
            NavigationService transactionDialogNavigationService = new NavigationService();

            RegisterPages(transactionDialogNavigationService);

            IUnityContainer container = new UnityContainer();
            container.RegisterType<ILocalStorage, LocalStorage>(TypeLifetime.PerContainer);


            ILocalStorage storage = container.Resolve<ILocalStorage>();
            Mocks.SymbolsRepository symbolRepository = new Mocks.SymbolsRepository();
            container.RegisterInstance<INewsRepository>(symbolRepository);
            container.RegisterInstance<ISymbolRepository>(symbolRepository);
            container.RegisterInstance<IHistoricalPriceRepository>(symbolRepository);
            container.RegisterInstance<ISymbolKeyStatsRepository>(symbolRepository);

            container.RegisterType<IViewModelFactory, ViewModelFactory>(TypeLifetime.PerContainer);
            container.RegisterInstance<INavigationService>(navigationService);
            container.RegisterInstance<INavigationService>("TransactionDialog", transactionDialogNavigationService);

            ViewModelFactory vmFactory = container.Resolve<ViewModelFactory>();

            ThemeManager.ThemeName = Theme.Dark;
            mainViewModel = vmFactory.CreateMainViewModel();
            Page mainPage = navigationService.GetMainPage(mainViewModel);
            mainViewModel.DialogService = new DialogService(mainPage, "EULADialog");
            MainPage = mainPage; 
            mainViewModel.Initialize();
        }

        protected override void OnStart() {
            mainViewModel.ShowLicenseAgreement();
        }

        protected override void OnSleep() {
            MessagingCenter.Instance.Send<object>(this, Constants.DeviceLocked);
        }

        protected override void OnResume() {
            MessagingCenter.Instance.Send<object>(this, Constants.DeviceUnlocked);
        }

        void RegisterPages(NavigationService transactionDialogNavigationService) {
            var pageLocator = PageLocator.Instance;

            pageLocator.Register<MainViewModel, MainPage>();

            pageLocator.Register<MarketViewModel>((vm) => new NavigationHostPage(new MarketPage()) { BindingContext = vm });
            pageLocator.Register<MarketNewsViewModel>((vm) => new NavigationHostPage(new MarketNewsPage()) { BindingContext = vm });
            pageLocator.Register<WatchlistViewModel>((vm) => new NavigationHostPage(new WatchlistPage()) { BindingContext = vm });
            pageLocator.Register<PortfolioViewModel>((vm) => new NavigationHostPage(new PortfolioPage()) { BindingContext = vm });

            pageLocator.Register<SymbolViewModel, SymbolPage>();
            pageLocator.Register<SymbolChartViewModel, SymbolChartPage>();
            pageLocator.Register<SymbolDetailViewModel, SymbolDetailPage>();
            pageLocator.Register<SymbolNewsViewModel, SymbolNewsPage>();
            pageLocator.Register<PortfolioTransactionsViewModel, PortfolioTransactionsPage>();
            pageLocator.Register<NewsItemViewModel, NewsItemPage>();

            pageLocator.Register<EditTransactionViewModel>((vm) => transactionDialogNavigationService.GetMainPage((EditTransactionViewModel)vm), "TransactionDialog");
            pageLocator.Register<EditTransactionViewModel>((vm) => new NavigationHostPage(new EditTransactionPage()) { BindingContext = vm });

            pageLocator.Register<PortfolioLineChartViewModel, PortfolioLineChartPage>();
            pageLocator.Register<PortfolioDonutChartViewModel, PortfolioDonutChartPage>();
            pageLocator.Register<PortfolioBarChartViewModel, PortfolioBarChartPage>();
            pageLocator.Register<MarketSymbolSearchViewModel, MarketSearchSymbolPage>();
            pageLocator.Register<WatchListSymbolSearchViewModel, WatchListSearchSymbolPage>();
            pageLocator.Register<TickerSearchViewModel, TickerSearchPage>();
            pageLocator.Register<EULAViewModel, EULANavigationPage>();
            pageLocator.Register<DisclaimerViewModel, DisclaimerPage>();
        }
    }

    static class Constants {
        public const string SafeAreaInsetsPropertyName = "SafeAreaInsets";

        public const string ToggleDrawerMessage = "ToggleDrawer";
        public const string DeviceLocked = "DeviceLocked";
        public const string DeviceUnlocked = "DeviceUnlocked";
    }
}
