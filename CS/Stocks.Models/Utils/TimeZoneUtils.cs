using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Stocks.Models {
    public static class TimeZoneUtils {
        const string IEX_TIMEZONE_ID_WIN = "Eastern Standard Time";
        const string IEX_TIMEZONE_ID_IANA = "America/New_York";

        static Dictionary<string, TimeZoneInfo> timeZones;

        public static TimeZoneInfo IEX { get; }
        public static TimeZoneInfo Local => TimeZoneInfo.Local;
        public static TimeZoneInfo Utc => TimeZoneInfo.Utc;

        static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        static TimeZoneUtils() {
            timeZones = new Dictionary<string, TimeZoneInfo>();
            IEX = FindSystemTimeZoneById(IsWindows ? IEX_TIMEZONE_ID_WIN : IEX_TIMEZONE_ID_IANA);
        }

        static bool ShouldConvertTime(TimeFrame timeFrame) {
            switch (timeFrame) {
                case TimeFrame.D:
                case TimeFrame.W:
                case TimeFrame.MN:
                    return false;
                default:
                    return true;
            }
        }
        public static DateTime FromLocalToIEXIfNeed(TimeFrame timeFrame, DateTime dateTime) {
            return ShouldConvertTime(timeFrame) ? FromLocalToIEX(dateTime) : dateTime;
        }
        public static DateTime FromIEXToLocalIfNeed(TimeFrame timeFrame, DateTime dateTime) {
            return ShouldConvertTime(timeFrame) ? FromIEXToLocal(dateTime) : dateTime;
        }
        public static DateTime FromLocalToIEX(DateTime dateTime) {
            return ConvertTime(dateTime, Local, IEX);
        }
        public static DateTime FromUtcToIEX(DateTime dateTime) {
            return ConvertTime(dateTime, Utc, IEX);
        }
        public static DateTime FromIEXToLocal(DateTime dateTime) {
            return ConvertTime(dateTime, IEX, Local);
        }

        static TimeZoneInfo FindSystemTimeZoneById(string timeZoneId) {
            try {
                TimeZoneInfo timeZoneInfo;
                if (timeZones != null && timeZones.TryGetValue(timeZoneId, out timeZoneInfo))
                    return timeZoneInfo;
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (Exception e) {
                if (timeZoneId == Local.Id)
                    return Local;
                throw e;
            }
        }

        static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo targetTimeZone) {
            if (sourceTimeZone == null || targetTimeZone == null)
                return dateTime;
            if (sourceTimeZone.Equals(targetTimeZone))
                return dateTime;
            return TimeZoneInfo.ConvertTime(NormalizeDate(dateTime), sourceTimeZone, targetTimeZone);
        }

        static DateTime NormalizeDate(DateTime dateTime) {
            if (dateTime.Kind != DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            return dateTime;
        }
    }
}
