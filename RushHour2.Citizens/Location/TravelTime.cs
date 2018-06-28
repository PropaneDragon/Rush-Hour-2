using ColossalFramework;
using RushHour2.Core.Reporting;
using System;
using UnityEngine;

namespace RushHour2.Citizens.Location
{
    public static class TravelTime
    {
        private static readonly double ESTIMATED_DISTANCE_PER_MINUTE = 1500; //Calculated distance I think you can probably get in an average city per minute.
        private static readonly int MAXIMUM_TRAVEL_HOURS = 5;

        public static TimeSpan EstimateTravelTime(Building startBuilding, Building endBuilding)
        {
            return EstimateTravelTime(startBuilding.m_position, endBuilding.m_position);
        }

        public static TimeSpan EstimateTravelTime(Vector3 startPosition, Vector3 endPosition)
        {
            var simulationManager = Singleton<SimulationManager>.instance;
            var difference = (startPosition - endPosition).magnitude;
            var estimatedTimeToTravelIrl = TimeSpan.FromMinutes(difference / ESTIMATED_DISTANCE_PER_MINUTE);
            var irlAverageTimePerStep = TimeSpan.FromTicks(Math.Max(100, simulationManager.m_simulationProfiler.m_averageStepDuration) * 10L);
            var timeToTravelFrames = estimatedTimeToTravelIrl.Ticks / irlAverageTimePerStep.Ticks;
            var estimatedTimeToTravelInGame = TimeSpan.FromTicks(simulationManager.m_timePerFrame.Ticks * timeToTravelFrames);

            if (estimatedTimeToTravelInGame.TotalHours >= MAXIMUM_TRAVEL_HOURS)
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Warning, $"Estimated travel time for a citizen was over {MAXIMUM_TRAVEL_HOURS} hours ({estimatedTimeToTravelInGame.TotalHours} estimated hours) with an estimated IRL time of {estimatedTimeToTravelIrl.Minutes} minutes over a distance of {difference}. This isn't good, so it's been reduced.");
                estimatedTimeToTravelInGame = TimeSpan.FromHours(MAXIMUM_TRAVEL_HOURS);
            }

            return estimatedTimeToTravelInGame;
        }
    }
}
