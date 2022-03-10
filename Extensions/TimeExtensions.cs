using System;
using UnityEngine;

namespace CoreDev.Extensions
{
    public static class TimeExtensions
    {
        public static DateTime FromEpochTime(this Int64 epochTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(epochTime);
        }

        public static Int64 ToEpochTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(Math.Floor((date.ToUniversalTime() - epoch).TotalSeconds));
        }

        public static DateTime FromEpochTimeMilliseconds(this Int64 epochTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(epochTime);
        }

        public static Int64 ToEpochTimeMilliseconds(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(Math.Floor((date.ToUniversalTime() - epoch).TotalMilliseconds));
        }

        public static string SecToHrMinSecString(this int seconds)
        {
            return SecToHrMinSecString((double)seconds);
        }

        public static string SecToHrMinSecString(this float seconds)
        {
            return SecToHrMinSecString((double)seconds);
        }

        public static string SecToHrMinSecString(this double seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            int hour = timeSpan.Hours;
            int minute = timeSpan.Minutes;
            int sec = timeSpan.Seconds;

            string time = string.Empty;
            if (hour > 0) { time += hour + "hr "; }
            if (minute > 0) { time += minute + "min "; }
            if (sec > 0 || string.IsNullOrEmpty(time)) { time += sec + "sec"; }
            return time.Trim();
        }
    }
}
