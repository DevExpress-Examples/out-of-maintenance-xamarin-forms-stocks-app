<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/243744496/20.1.1%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T867470)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
## Mobile UI for Xamarin.Forms: Stock Market

This example demonstrates how to implement the Stock Market application with sample data and the following pages:

- [Market](./CS/Stocks.UI/Views/MarketPage) - Provides a snapshot of market conditions (most active stocks, biggest gains/losses, etc).
- [Stock Detail](./CS/Stocks.UI/Views/SymbolPage) - Provides detailed stock information.
- [Watchlist News](./CS/Stocks.UI/Views/NewsPage) - Displays news for the stocks in the app’s watchlist.
- [Watchlist](./CS/Stocks.UI/Views/WatchlistPage) - Displays a list of stocks added to the watchlist.
- [Search](./CS/Stocks.UI/Views/SearchSymbolPage) - Allows users to add stock symbols to their watchlist.
- [Portfolio](./CS/Stocks.UI/Views/PortfolioPage) - Allows users to enter hypothetical buy/sell orders to measure their stock market acumen.

This application uses the following DevExpress Xamarin.Forms controls:

- [DrawerPage](https://docs.devexpress.com/MobileControls/401159/xamarin-forms/navigation-controls/drawer-page/index) - Implements navigation in the application.
- [TabPage](https://docs.devexpress.com/MobileControls/401160/xamarin-forms/navigation-controls/tab-page/index) and [TabView](https://docs.devexpress.com/MobileControls/401161/xamarin-forms/navigation-controls/tab-view/index) - Implements navigation between pages or views embedded in the page.
- [DataGridView](https://docs.devexpress.com/MobileControls/DevExpress.XamarinForms.DataGrid.DataGridView) - Displays information on app pages. The following grid features are used: [template column](https://docs.devexpress.com/MobileControls/DevExpress.XamarinForms.DataGrid.TemplateColumn), data [sorting](https://docs.devexpress.com/MobileControls/400552/xamarin-forms/data-grid/getting-started/lesson-5-sort-data) and [grouping](https://docs.devexpress.com/MobileControls/400550/xamarin-forms/data-grid/getting-started/lesson-3-group-data), and [swipe actions](https://docs.devexpress.com/MobileControls/401053/xamarin-forms/data-grid/examples/swipe-actions). 
- [Charts](http://docs.devexpress.com/MobileControls/400422/xamarin-forms/charts/index) - Visualize financial data as Open-High-Low-Close series, bars, lines, donuts, etc.

To run the application:
1. [Obtain your NuGet feed URL](http://docs.devexpress.com/GeneralInformation/116042/installation/install-devexpress-controls-using-nuget-packages/obtain-your-nuget-feed-url).
2. Register the DevExpress NuGet feed as a package source.
3. Restore all NuGet packages for the solution.
