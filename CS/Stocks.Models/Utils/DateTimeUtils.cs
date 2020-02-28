using System;
namespace Stocks.Models {
    public static class DateTimeUtils {
        public static DateTime Round(this DateTime dateTime, TimeFrame timeFrame) {
            switch (timeFrame) {
                case TimeFrame.M1:
                    return getTimeIntervalForMinuteTimeFrame(dateTime, 1);
                case TimeFrame.M2:
                    return getTimeIntervalForMinuteTimeFrame(dateTime, 2);
                case TimeFrame.M3:
                    return getTimeIntervalForMinuteTimeFrame(dateTime, 3);
                case TimeFrame.M4:
                    return getTimeIntervalForMinuteTimeFrame(dateTime, 4);
                case TimeFrame.M5:
                    return getTimeIntervalForMinuteTimeFrame(dateTime, 5);
                case TimeFrame.M6:
                    return getTimeIntervalForMinuteTimeFrame(dateTime, 6);
                case TimeFrame.M10:
                    return getTimeIntervalForMinuteTimeFrame(dateTime, 10);
                case TimeFrame.M12:
                    return getTimeIntervalForMinuteTimeFrame(dateTime, 12);
                case TimeFrame.M15:
                    return getTimeIntervalForMinuteTimeFrame(dateTime, 15);
                case TimeFrame.M20:
                    return getTimeIntervalForMinuteTimeFrame(dateTime, 20);
                case TimeFrame.M30:
                    return getTimeIntervalForMinuteTimeFrame(dateTime, 30);
                case TimeFrame.H1:
                    return getTimeIntervalForHourTimeFrame(dateTime, 1);
                case TimeFrame.H2:
                    return getTimeIntervalForHourTimeFrame(dateTime, 2);
                case TimeFrame.H3:
                    return getTimeIntervalForHourTimeFrame(dateTime, 3);
                case TimeFrame.H4:
                    return getTimeIntervalForHourTimeFrame(dateTime, 4);
                case TimeFrame.D:
                    return dateTime.Date;
                case TimeFrame.W:
                    return dateTime.AddDays(-(int)dateTime.DayOfWeek).Date;
                case TimeFrame.MN:
                    return new DateTime(dateTime.Year, dateTime.Month, 1);
                default:
                    throw new NotSupportedException();
            }
        }

        public static DateTime Add(this DateTime dateTime, TimeFrame timeFrame, int count = 1) {
            switch (timeFrame) {
                case TimeFrame.M1:
                    return dateTime.AddMinutes(1 * count);
                case TimeFrame.M2:
                    return dateTime.AddMinutes(2 * count);
                case TimeFrame.M3:
                    return dateTime.AddMinutes(3 * count);
                case TimeFrame.M4:
                    return dateTime.AddMinutes(4 * count);
                case TimeFrame.M5:
                    return dateTime.AddMinutes(5 * count);
                case TimeFrame.M6:
                    return dateTime.AddMinutes(6 * count);
                case TimeFrame.M10:
                    return dateTime.AddMinutes(10 * count);
                case TimeFrame.M12:
                    return dateTime.AddMinutes(12 * count);
                case TimeFrame.M15:
                    return dateTime.AddMinutes(15 * count);
                case TimeFrame.M20:
                    return dateTime.AddMinutes(20 * count);
                case TimeFrame.M30:
                    return dateTime.AddMinutes(30 * count);
                case TimeFrame.H1:
                    return dateTime.AddHours(1 * count);
                case TimeFrame.H2:
                    return dateTime.AddHours(2 * count);
                case TimeFrame.H3:
                    return dateTime.AddHours(3 * count);
                case TimeFrame.H4:
                    return dateTime.AddHours(4 * count);
                case TimeFrame.D:
                    return dateTime.AddDays(1 * count);
                case TimeFrame.W:
                    return dateTime.AddDays(7 * count);
                case TimeFrame.MN:
                    return dateTime.AddMonths(1 * count);
                default:
                    throw new NotSupportedException();
            }
        }

        public static bool IsWeekend(this DateTime date) {
            return date.DayOfWeek == DayOfWeek.Sunday
                || date.DayOfWeek == DayOfWeek.Monday;
        }

        // returns 0 if value is in range; -1 if smaller than range's start, 1 if larger than range's end.
        public static int InRange(this DateTime value, DateTime start, DateTime end) {
            if (value > end) return 1;
            if (value < start) return -1;
            return 0;
        }

        static DateTime getTimeIntervalForMinuteTimeFrame(DateTime dateTime, int minutes) {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute / minutes * minutes, 0);
        }

        static DateTime getTimeIntervalForHourTimeFrame(DateTime dateTime, int hours) {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour / hours * hours, 0, 0);
        }
    }
}
