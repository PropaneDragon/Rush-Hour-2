using System;

namespace RushHour2.Core.Settings
{
    public static class UserModSettings
    {
        public static bool Enabled = true;
        public static bool LogToFile = true;
        public static float TimeSpeedMultiplier = 0.25f;

        public static TimeSpan StartTime_Schools = TimeSpan.FromHours(8.5);
        public static TimeSpan StartTime_University = TimeSpan.FromHours(10);
        public static TimeSpan StartTime_Work = TimeSpan.FromHours(9);

        public static TimeSpan Duration_Schools = TimeSpan.FromHours(6.5);
        public static TimeSpan Duration_University = TimeSpan.FromHours(6);
        public static TimeSpan Duration_Work = TimeSpan.FromHours(8);

        public static bool TimeIsBetween(DateTime currentTime, TimeSpan startTime, TimeSpan duration)
        {
            return !TimeIsBefore(currentTime, startTime) && !TimeIsAfter(currentTime, startTime, duration);
        }

        public static bool TimeIsBefore(DateTime currentTime, TimeSpan startTime)
        {
            var begins = currentTime.Date.Add(startTime);

            return currentTime < begins;
        }

        public static bool TimeIsAfter(DateTime currentTime, TimeSpan startTime, TimeSpan duration)
        {
            var begins = currentTime.Date.Add(startTime);
            var ends = begins.Add(duration);

            return currentTime > ends;
        }
    }
}
