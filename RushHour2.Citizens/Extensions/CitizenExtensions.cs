using ColossalFramework;
using Harmony;
using RushHour2.Core.Reporting;
using System;
using UnityEngine;

namespace RushHour2.Citizens.Extensions
{
    public static class CitizenExtensions
    {
        private static readonly int ESTIMATED_DISTANCE_PER_MINUTE = 1500; //Calculated distance I think you can probably get in an average city per minute.
        private static readonly int MAXIMUM_TRAVEL_HOURS = 5;

        public static bool Exists(this Citizen citizen) => citizen.m_homeBuilding != 0 || citizen.m_workBuilding != 0 || citizen.m_visitBuilding != 0 || citizen.m_instance != 0 || citizen.m_vehicle != 0;

        public static bool NeedsGoods(this Citizen citizen) => citizen.m_flags.IsFlagSet(Citizen.Flags.NeedGoods);

        public static bool IsInsideBuilding(this Citizen citizen) => !citizen.IsMoving();

        public static bool IsVisible(this Citizen citizen) => citizen.m_instance != 0;

        public static bool IsMoving(this Citizen citizen) => citizen.CurrentLocation == Citizen.Location.Moving;

        public static bool AtHome(this Citizen citizen) => citizen.CurrentLocation == Citizen.Location.Home;

        public static bool AtWork(this Citizen citizen) => citizen.CurrentLocation == Citizen.Location.Work;

        public static bool IsVisiting(this Citizen citizen) => citizen.CurrentLocation == Citizen.Location.Visit;

        public static bool ValidHomeBuilding(this Citizen citizen) => citizen.HomeBuilding() != 0;

        public static bool ValidWorkBuilding(this Citizen citizen) => citizen.WorkBuilding() != 0;

        public static bool ValidVisitBuilding(this Citizen citizen) => citizen.VisitBuilding() != 0;

        public static ushort HomeBuilding(this Citizen citizen) => citizen.m_homeBuilding;

        public static ushort WorkBuilding(this Citizen citizen) => citizen.m_workBuilding;

        public static ushort VisitBuilding(this Citizen citizen) => citizen.m_visitBuilding;

        public static Building? HomeBuildingInstance(this Citizen citizen) => GetBuildingFromId(citizen.HomeBuilding());

        public static Building? WorkBuildingInstance(this Citizen citizen) => GetBuildingFromId(citizen.WorkBuilding());

        public static Building? VisitBuildingInstance(this Citizen citizen) => GetBuildingFromId(citizen.VisitBuilding());

        public static bool ValidBuilding(this Citizen citizen)
        {
            var validHome = citizen.AtHome() && citizen.ValidHomeBuilding();
            var validWork = citizen.AtWork() && citizen.ValidWorkBuilding();
            var validVisit = citizen.IsVisiting() && citizen.ValidVisitBuilding();

            return validHome || validWork || validVisit;
        }

        public static bool ShouldBeAtWork(this Citizen citizen)
        {
            return citizen.ShouldBeAtWork(new TimeSpan(0));
        }

        public static bool ShouldGoToWork(this Citizen citizen)
        {
            if (citizen.ValidWorkBuilding() && citizen.ShouldBeAtWork(TimeSpan.FromHours(MAXIMUM_TRAVEL_HOURS)))
            {
                var currentBuildingInstance = citizen.GetBuildingInstance();
                var workBuildingInstance = citizen.WorkBuildingInstance();

                if (currentBuildingInstance.HasValue && workBuildingInstance.HasValue)
                {
                    var simulationManager = Singleton<SimulationManager>.instance;
                    var homeBuildingLocation = currentBuildingInstance.Value.m_position;
                    var workBuildingPosition = workBuildingInstance.Value.m_position;
                    var difference = (homeBuildingLocation - workBuildingPosition).magnitude;
                    var estimatedTimeToTravelIrl = TimeSpan.FromMinutes(difference / ESTIMATED_DISTANCE_PER_MINUTE);
                    var irlAverageTimePerStep = TimeSpan.FromTicks(simulationManager.m_simulationProfiler.m_averageStepDuration * 10L);
                    var timeToTravelFrames = estimatedTimeToTravelIrl.Ticks / irlAverageTimePerStep.Ticks;
                    var estimatedTimeToTravelInGame = TimeSpan.FromTicks(simulationManager.m_timePerFrame.Ticks * timeToTravelFrames);

                    if (estimatedTimeToTravelInGame.TotalHours >= MAXIMUM_TRAVEL_HOURS)
                    {
                        LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Warning, $"Estimated travel time for a citizen was over {MAXIMUM_TRAVEL_HOURS} hours ({estimatedTimeToTravelInGame.Hours} estimated hours) with an estimated IRL time of {estimatedTimeToTravelIrl.Minutes} minutes over a distance of {difference}. This isn't good, so it's been reduced.");
                        estimatedTimeToTravelInGame = TimeSpan.FromHours(MAXIMUM_TRAVEL_HOURS);
                    }

                    return citizen.ShouldBeAtWork(estimatedTimeToTravelInGame);
                }
            }

            return false;
        }

        public static bool ShouldBeAtWork(this Citizen citizen, TimeSpan offset)
        {
            var simulationManager = Singleton<SimulationManager>.instance;
            var currentTime = simulationManager.m_currentGameTime;

            return citizen.ShouldBeAtWork(currentTime + offset);
        }

        public static bool ShouldBeAtWork(this Citizen citizen, DateTime time)
        {
            return citizen.ValidWorkBuilding() && (time.Hour >= 9 || (citizen.AtWork() && time.Hour >= 6)) && time.Hour < 17;
        }

        public static bool Tired(this Citizen citizen)
        {
            return citizen.Tired(new TimeSpan(0));
        }

        public static bool Tired(this Citizen citizen, TimeSpan offset)
        {
            var simulationManager = Singleton<SimulationManager>.instance;
            var currentTime = simulationManager.m_currentGameTime;

            return citizen.Tired(currentTime + offset);
        }

        public static bool Tired(this Citizen citizen, DateTime time)
        {
            return time.Hour > 22 || time.Hour < 5;
        }

        public static ushort GetBuilding(this Citizen citizen)
        {
            var buildingManager = Singleton<BuildingManager>.instance;

            if (citizen.AtHome() && citizen.ValidHomeBuilding())
            {
                return citizen.HomeBuilding();
            }
            else if (citizen.AtWork() && citizen.ValidWorkBuilding())
            {
                return citizen.WorkBuilding();
            }
            else if (citizen.IsVisiting() && citizen.ValidVisitBuilding())
            {
                return citizen.VisitBuilding();
            }

            return 0;
        }

        public static Building? GetBuildingInstance(this Citizen citizen)
        {
            var buildingManager = Singleton<BuildingManager>.instance;
            var buildingId = citizen.GetBuilding();

            return GetBuildingFromId(buildingId);
        }

        private static Building? GetBuildingFromId(ushort buildingId)
        {
            if (buildingId != 0)
            {
                var buildingManager = Singleton<BuildingManager>.instance;
                return buildingManager.m_buildings.m_buffer[buildingId];
            }

            return null;
        }
    }
}
