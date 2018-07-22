using RushHour2.Core.Reporting;
using RushHour2.Core.Settings;
using System;
using UnityEngine;

namespace RushHour2.Locations.Travel
{
    public static class TravelTime
    {
        private static readonly double ESTIMATED_DISTANCE_PER_MINUTE = 15 * (.25d / UserModSettings.Settings.Simulation_Speed); //Calculated distance I think you can probably get in an average city per minute.
        private static readonly int MAXIMUM_TRAVEL_HOURS = 4;

        public static TimeSpan EstimateTravelTime(Building startBuilding, Building endBuilding)
        {
            return EstimateTravelTime(startBuilding.m_position, endBuilding.m_position);
        }

        public static TimeSpan EstimateTravelTime(Vector3 startPosition, Vector3 endPosition)
        {
            var simulationManager = SimulationManager.instance;
            var difference = (startPosition - endPosition).magnitude;
            var estimatedTimeToTravelInGame = TimeSpan.FromMinutes(difference / ESTIMATED_DISTANCE_PER_MINUTE);

            if (estimatedTimeToTravelInGame.TotalHours >= MAXIMUM_TRAVEL_HOURS)
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.File, LoggingWrapper.LogType.Warning, $"Estimated travel time for a citizen was over {MAXIMUM_TRAVEL_HOURS} hours ({estimatedTimeToTravelInGame.TotalHours} estimated hours) with a distance of {difference}. This isn't good, so it's been reduced.");
                estimatedTimeToTravelInGame = TimeSpan.FromHours(MAXIMUM_TRAVEL_HOURS);
            }

            return estimatedTimeToTravelInGame;
        }
    }
}
