using System;
using System.Collections.Generic;
using System.Windows.Input;
using Stocks.Models;

namespace Stocks.ViewModels {
    public class PortfolioTabItemViewModel : NotificationObject {
        IEnumerable<IrrHistoryItem> irrHistory;
        IEnumerable<PortfolioItemStatistics> itemStatistics;

        bool isSelected;
        PortfolioChartViewModel chartViewModel;

        public bool IsSelected {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public PortfolioChartType ChartType { get; }
        public PortfolioChartViewModel ChartViewModel {
            get => chartViewModel;
            private set => SetProperty(ref chartViewModel, value);
        }

        public ICommand OpenDetailCommand { get; }

        public PortfolioTabItemViewModel(PortfolioChartType chartType, Action<PortfolioChartViewModel> openDetailHandler) {
            ChartType = chartType;
            OpenDetailCommand = new DelegateCommand(() => 
                openDetailHandler?.Invoke(PortfolioChartViewModel.Create(chartType, irrHistory, itemStatistics, true)));
        }

        public void Update(IEnumerable<IrrHistoryItem> irrHistory, IEnumerable<PortfolioItemStatistics> itemStatistics) {
            this.irrHistory = irrHistory;
            this.itemStatistics = itemStatistics;
            ChartViewModel = PortfolioChartViewModel.Create(ChartType, irrHistory, itemStatistics);
        }
    }
}
