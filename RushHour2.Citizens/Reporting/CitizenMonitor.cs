using RushHour2.Core.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RushHour2.Citizens.Reporting
{
    public static class CitizenMonitor
    {
        public enum Activity
        {
            AtHome,
            AtWork,
            Visiting,
            GoingToVisit,
            GoingHome,
            GoingToWork
        }

        private static object _logLockObject = new object();
        private static Dictionary<uint, Activity> _activitiesLog = new Dictionary<uint, Activity>();

        public static void LogActivity(uint citizenId, Activity activity)
        {
            lock (_logLockObject)
            {
                _activitiesLog[citizenId] = activity;
            }
        }

        public static void WriteLog(LoggingWrapper.LogArea logArea)
        {
            lock (_logLockObject)
            {
                var values = Enum.GetValues(typeof(Activity));
                var output = new List<string>();
                
                foreach (Activity value in values)
                {
                    var citizensForActivity = _activitiesLog.Where(citizenLog => citizenLog.Value == value);
                    output.Add($"[{citizensForActivity.Count()}] {value.ToString()}");
                }

                LoggingWrapper.Log(logArea, LoggingWrapper.LogType.Message, string.Join("\n", output.ToArray()));
            }
        }
    }
}
