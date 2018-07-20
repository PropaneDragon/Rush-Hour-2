using RushHour2.Core.Reporting;
using RushHour2.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RushHour2.Citizens.Reporting
{
    public static class CitizenActivityMonitor
    {
        public enum Activity
        {
            Leaving,
            GoingHome,
            GoingToWork,
            AttemptingToGoForEntertainment,
            AttemptingToGoShopping,
            GoingToVisit,
            Visiting,
            AtWork,
            Moving,
            Sleeping,
            Unknown,
            AtHome,
            VisitingPark,
            VisitingCitizen,
            VisitingShop,
            InShelter,
            VisitingSchool,
            VisitingElectricity,
            VisitingFireDepartment,
            VisitingGarbage,
            VisitingHealthcare,
            VisitingIndustrial,
            VisitingNature,
            VisitingMonumnet,
            VisitingOffice,
            VisitingPolice,
            OnPublicTransport,
            AtAHouse,
            OnARoad,
            AtATouristAttraction,
            GettingWet,
        }

        private static object _logLockObject = new object();
        private static Dictionary<uint, Activity> _activitiesLog = new Dictionary<uint, Activity>();

        public static void LogActivity(uint citizenId, Activity activity)
        {
            if (UserModSettings.Settings.Log_Citizen_Status)
            {
                lock (_logLockObject)
                {
                    _activitiesLog[citizenId] = activity;
                }
            }
        }

        public static Activity GetActivity(uint citizenId)
        {
            if (_activitiesLog.ContainsKey(citizenId))
            {
                return _activitiesLog[citizenId];
            }

            return Activity.Moving;
        }

        public static void WriteLog(LoggingWrapper.LogArea logArea)
        {
            if (UserModSettings.Settings.Log_Citizen_Status)
            {
                lock (_logLockObject)
                {
                    var activityValues = Enum.GetValues(typeof(Activity));
                    var output = new List<string>();
                    var citizenManager = CitizenManager.instance;
                    var vehicleCount = VehicleManager.instance.m_vehicleCount;
                    var citizenCount = CitizenManager.instance.m_instanceCount;
                    var percentageVehicles = (vehicleCount / (double)VehicleManager.MAX_VEHICLE_COUNT) * 100d;
                    var percentageCitizens = (citizenCount / (double)CitizenManager.MAX_CITIZEN_COUNT) * 100d;

                    foreach (Activity activity in activityValues)
                    {
                        var citizensForActivity = _activitiesLog.Where(citizenLog => citizenLog.Value == activity);
                        var agesForActivity = citizensForActivity.GroupBy(citizenLog => Citizen.GetAgeGroup(citizenManager.m_citizens.m_buffer[citizenLog.Key].Age));
                        var ageOutput = new List<string>();
                        var total = 0;

                        foreach (var ageForActivity in agesForActivity)
                        {
                            ageOutput.Add($"[{ageForActivity.Count()} {ageForActivity.Key}]");

                            total += ageForActivity.Count();
                        }

                        output.Add($"{activity.ToString()}: {string.Join(" ", ageOutput.ToArray())} (total: {total})");
                    }

                    LoggingWrapper.Log(logArea, LoggingWrapper.LogType.Message, $"Current activities: \n{string.Join("\n", output.ToArray())}\nCitizen instances: {percentageCitizens}%, Vehicle instances: {percentageVehicles}%");
                }
            }
        }
    }
}
