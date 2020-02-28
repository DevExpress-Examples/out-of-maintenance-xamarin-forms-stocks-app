using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stocks.Models;

namespace Stocks.ViewModels {
    public enum PortfolioChartType { Line, Donut, Bar }

    public abstract class PortfolioChartViewModel : NotificationObject {
        public abstract PortfolioChartType ChartType { get; }
        public bool ShouldShowDetail { get; }

        protected PortfolioChartViewModel(bool shouldShowDetail) {
            ShouldShowDetail = shouldShowDetail;
        }

        public static PortfolioChartViewModel Create(PortfolioChartType chartType, IEnumerable<IrrHistoryItem> irrHistory, IEnumerable<PortfolioItemStatistics> itemStatistics, bool isDetailedView = false) {
            switch (chartType) {
                case PortfolioChartType.Line:
                    return PortfolioLineChartViewModel.Create(irrHistory, isDetailedView);
                case PortfolioChartType.Donut:
                    return PortfolioDonutChartViewModel.Create(itemStatistics, isDetailedView);
                case PortfolioChartType.Bar:
                    return PortfolioBarChartViewModel.Create(itemStatistics, isDetailedView);
                default: return null;
            }
        }
    }

    public class PortfolioLineChartViewModel : PortfolioChartViewModel {
        public IEnumerable<IrrHistoryItem> Data { get; }
        
        public override PortfolioChartType ChartType => PortfolioChartType.Line;

        PortfolioLineChartViewModel(IEnumerable<IrrHistoryItem> data, bool shouldShowDetail) : base(shouldShowDetail) {
            Data = data;
        }

        public static PortfolioLineChartViewModel Create(IEnumerable<IrrHistoryItem> data, bool isDetailedView = false) {
            return new PortfolioLineChartViewModel(data, !isDetailedView && data.Any());
        }
    }

    public class PortfolioDonutChartViewModel : PortfolioChartViewModel {
        public IEnumerable<PortfolioItemStatistics> Data { get; }
        public override PortfolioChartType ChartType => PortfolioChartType.Donut;

        PortfolioDonutChartViewModel(IEnumerable<PortfolioItemStatistics> data, bool shouldShowDetail) : base(shouldShowDetail) {
            Data = data;
        }

        public static PortfolioDonutChartViewModel Create(IEnumerable<PortfolioItemStatistics> data, bool isDetailedView = false) {
            List<PortfolioItemStatistics> donutChartData = data.OrderByDescending(x => x.AbsoluteActualValue).ToList();
            List<PortfolioItemStatistics> donutChartVisualData;

            if (isDetailedView || !donutChartData.Any())
                donutChartVisualData = donutChartData;
            else {
                int maxVisibleSegmentsCount = 10;
                double minVisibleSegmentSize = 0.03;

                List<PortfolioItemStatistics> others = donutChartData.Where((x, i) => x.PartInTotal <= minVisibleSegmentSize || i > maxVisibleSegmentsCount).ToList();
                if (others.Count > 1) {
                    double othersSum = others.Sum(x => x.AbsoluteActualValue);
                    donutChartVisualData = donutChartData.Take(donutChartData.Count - others.Count).ToList();
                    donutChartVisualData.Add(new PortfolioItemStatistics("Others", 1, othersSum, othersSum));
                    donutChartVisualData = donutChartVisualData.OrderByDescending(x => x.AbsoluteActualValue).ToList();
                } else
                    donutChartVisualData = donutChartData;
            }
            return new PortfolioDonutChartViewModel(donutChartVisualData, !isDetailedView && donutChartVisualData.Any());
        }
    }

    public class PortfolioBarChartViewModel : PortfolioChartViewModel {
        public IEnumerable<PortfolioItemStatistics> Gainers { get; }
        public IEnumerable<PortfolioItemStatistics> Losers { get; }
        public double SideMargin { get; }
        public string VisualMax { get; }
        public string VisualMin { get; }
        
        public override PortfolioChartType ChartType => PortfolioChartType.Bar;

        PortfolioBarChartViewModel(IEnumerable<PortfolioItemStatistics> losers, IEnumerable<PortfolioItemStatistics> gainers, double sideMargin, string visualMin, string visualMax, bool shouldShowDetail) :
            base(shouldShowDetail) {
            Losers = losers;
            Gainers = gainers;
            SideMargin = sideMargin;
            VisualMin = visualMin;
            VisualMax = visualMax;
        }

        public static PortfolioBarChartViewModel Create(IEnumerable<PortfolioItemStatistics> data, bool isDetailedView = false) {
            List<PortfolioItemStatistics> barChartData = data.ToList();
            barChartData.Sort(new PortfolioItemStatisticsComparer());
            int dataCount = barChartData.Count;

            IEnumerable<PortfolioItemStatistics> visualData;
            int maxVisibleBarCount;
            bool shouldUseVisualRange;
            bool shouldShowDetail;

            if (isDetailedView) {
                maxVisibleBarCount = 30;
                visualData = barChartData;
                shouldUseVisualRange = dataCount > maxVisibleBarCount;
                shouldShowDetail = false;
            } else {
                maxVisibleBarCount = 10;
                shouldShowDetail = dataCount > maxVisibleBarCount;
                visualData = shouldShowDetail ? barChartData.Take(maxVisibleBarCount / 2).Union(barChartData.Skip(dataCount - maxVisibleBarCount / 2)) : barChartData;
                shouldUseVisualRange = false;
            }
            IEnumerable<PortfolioItemStatistics> fallingData = visualData.Where(x => x.TotalProfit < 0).ToList();
            IEnumerable<PortfolioItemStatistics> risingData = visualData.Where(x => x.TotalProfit >= 0).ToList();
            double sideMargin = dataCount < maxVisibleBarCount ? 0.5 + (maxVisibleBarCount - dataCount) / 2 : 0.5;
            string visualMin = shouldUseVisualRange ? barChartData[dataCount - maxVisibleBarCount].Ticker : null;
            string visualMax = shouldUseVisualRange ? barChartData[dataCount - 1].Ticker : null;
            return new PortfolioBarChartViewModel(fallingData, risingData, sideMargin, visualMin, visualMax, shouldShowDetail && !isDetailedView && (fallingData.Any() || risingData.Any()));
        }

        class PortfolioItemStatisticsComparer : IComparer<PortfolioItemStatistics> {
            public int Compare(PortfolioItemStatistics x, PortfolioItemStatistics y) {
                if (x.TotalProfitPercent > y.TotalProfitPercent)
                    return 1;
                if (x.TotalProfitPercent < y.TotalProfitPercent)
                    return -1;
                if (x.ActualValue >= y.ActualValue)
                    return 1;
                return -1;
            }
        }

    }
}