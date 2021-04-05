using System;

namespace Media_Player.Extensions
{
    public static class TimeSpanExt
    {
        public static TimeSpan Add(this TimeSpan timeSpan, int hours = 0, int minutes = 0, int seconds = 0)
        {
            return timeSpan.Add(new TimeSpan(hours, minutes, seconds));
        }

        public static TimeSpan Subtract(this TimeSpan timeSpan, int hours = 0, int minutes = 0, int seconds = 0)
        {
            return timeSpan.Subtract(new TimeSpan(hours, minutes, seconds));
        }
    }
}
