using System;

namespace foundation
{
    public static class DateTimeExtension
    {
        public static int ZoneMinuteOffset = 0;

        public static string ServerTimeDisplay(this long serverTimeMilliseconds, string format)
        {
            DateTime dtStart = new DateTime(1970, 1, 1);
            long value = serverTimeMilliseconds + ZoneMinuteOffset * DateUtils.ONE_MINUTE_MILLISECOND;
            dtStart = dtStart.Add(new TimeSpan(value * 10000));

            return string.Format(format, dtStart);
        }

        public static string ServerTimeDisplay(this DateTime dateTime, string format)
        {
            DateTime dtStart = dateTime;
            long value =ZoneMinuteOffset * DateUtils.ONE_MINUTE_MILLISECOND;
            dtStart = dtStart.Add(new TimeSpan(value * 10000));

            return string.Format(format, dtStart);
        }
    }
}
