using ColossalFramework;
using Harmony;
using System;

namespace RushHour2.Citizens.Extensions
{
    public static class CitizenExtensions
    {
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

        public static bool ShouldBeAtWork(this Citizen citizen, TimeSpan offset)
        {
            var simulationManager = Singleton<SimulationManager>.instance;
            var currentTime = simulationManager.m_currentGameTime;

            return citizen.ShouldBeAtWork(currentTime + offset);
        }

        public static bool ShouldBeAtWork(this Citizen citizen, DateTime time)
        {
            return citizen.ValidWorkBuilding() && time.Hour >= 9 && time.Hour < 17;
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
            return time.Hour > 10 && time.Hour < 6;
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
