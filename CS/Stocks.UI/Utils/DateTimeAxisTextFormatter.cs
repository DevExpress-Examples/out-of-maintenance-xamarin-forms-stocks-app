using System;
using System.Globalization;
using DevExpress.XamarinForms.Charts;
using Stocks.Models;

namespace Stocks.UI.Utils {
    public class DateTimeAxisTextFormatter : IAxisLabelTextFormatter {
        string DateTimeFormat { get; }

        public DateTimeAxisTextFormatter(TimeFrame timeFrame) {
            DateTimeFormatInfo dtfi = CultureInfo.CurrentUICulture.DateTimeFormat;
            switch (timeFrame) {
                case TimeFrame.M1:
                case TimeFrame.M2:
                case TimeFrame.M3:
                case TimeFrame.M4:
                case TimeFrame.M5:
                case TimeFrame.M6:
                case TimeFrame.M10:
                case TimeFrame.M12:
                case TimeFrame.M15:
                case TimeFrame.M20:
                case TimeFrame.M30:
                case TimeFrame.H1:
                case TimeFrame.H2:
                case TimeFrame.H3:
                case TimeFrame.H4:
                    DateTimeFormat = "{0:t}";
                    break;
                case TimeFrame.D:
                case TimeFrame.W:
                    DateTimeFormat = "{0:" + dtfi.MonthDayPattern.Replace("MMMM", "MMM") + "}";
                    break;
                case TimeFrame.MN:
                    DateTimeFormat = "{0:" + dtfi.YearMonthPattern.Replace("MMMM", "MMM") + "}";
                    break;
                default:
                    throw new NotSupportedException($"The converter does not support the specified time frame ({timeFrame}) value.");
            }
        }

        public string Format(object value) {
            return string.Format(DateTimeFormat, value);
        }
    }
}
