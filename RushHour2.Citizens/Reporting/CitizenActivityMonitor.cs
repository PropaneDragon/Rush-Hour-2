using ColossalFramework;
using RushHour2.Core.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RushHour2.Citizens.Reporting
{
    public static class CitizenActivityMonitor
    {
        public enum Activity
        {
            Unknown,
            AtHome,
            AtWork,
            Visiting,
            GoingToVisit,
            GoingHome,
            GoingToWork,
            GoingOutAfterWork,
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

        public static Activity GetActivity(uint citizenId)
        {
            if (_activitiesLog.ContainsKey(citizenId))
            {
                return _activitiesLog[citizenId];
            }

            return Activity.Unknown;
        }

        public static void WriteLog(LoggingWrapper.LogArea logArea)
        {
            lock (_logLockObject)
            {
                var values = Enum.GetValues(typeof(Activity));
                var output = new List<string>();
                var citizenManager = Singleton<CitizenManager>.instance;
                
                foreach (Activity value in values)
                {
                    var citizensForActivity = _activitiesLog.Where(citizenLog => citizenLog.Value == value);
                    var agesForActivity = citizensForActivity.GroupBy(citizenLog => Citizen.GetAgeGroup(citizenManager.m_citizens.m_buffer[citizenLog.Key].Age));
                    var ageOutput = new List<string>();

                    foreach (var ageForActivity in agesForActivity)
                    {
                        ageOutput.Add($"[{ageForActivity.Count()} {ageForActivity.Key}]");
                    }
                    
                    output.Add($"{value.ToString()}: {string.Join(" ", ageOutput.ToArray())}");
                }

                LoggingWrapper.Log(logArea, LoggingWrapper.LogType.Message, $"Current activities: \n{string.Join("\n", output.ToArray())}");
            }
        }
    }
}
