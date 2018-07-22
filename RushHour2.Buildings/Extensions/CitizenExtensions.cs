namespace RushHour2.Buildings.Extensions
{
    public static class CitizenExtensions
    {
        public static bool ValidHomeBuilding(this Citizen citizen) => citizen.HomeBuilding() != 0;

        public static bool ValidWorkBuilding(this Citizen citizen) => citizen.WorkBuilding() != 0;

        public static bool ValidVisitBuilding(this Citizen citizen) => citizen.VisitBuilding() != 0;

        public static ushort HomeBuilding(this Citizen citizen) => citizen.m_homeBuilding;

        public static ushort WorkBuilding(this Citizen citizen) => citizen.m_workBuilding;

        public static ushort VisitBuilding(this Citizen citizen) => citizen.m_visitBuilding;

        public static Building? HomeBuildingInstance(this Citizen citizen) => GetBuildingFromId(citizen.HomeBuilding());

        public static Building? WorkBuildingInstance(this Citizen citizen) => GetBuildingFromId(citizen.WorkBuilding());

        public static bool AtHome(this Citizen citizen) => citizen.CurrentLocation == Citizen.Location.Home;

        public static bool AtWork(this Citizen citizen) => citizen.CurrentLocation == Citizen.Location.Work;

        public static bool InHotel(this Citizen citizen) => (citizen.VisitBuildingInstance()?.Info.m_class.m_subService ?? ItemClass.SubService.None) == ItemClass.SubService.CommercialTourist;

        public static bool IsVisiting(this Citizen citizen) => citizen.CurrentLocation == Citizen.Location.Visit;

        public static Building? VisitBuildingInstance(this Citizen citizen) => GetBuildingFromId(citizen.VisitBuilding());

        public static bool ValidBuilding(this Citizen citizen)
        {
            var validHome = citizen.AtHome() && citizen.ValidHomeBuilding();
            var validWork = citizen.AtWork() && citizen.ValidWorkBuilding();
            var validVisit = citizen.IsVisiting() && citizen.ValidVisitBuilding();

            return validHome || validWork || validVisit;
        }

        public static ushort GetBuilding(this Citizen citizen)
        {
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
            var buildingId = citizen.GetBuilding();

            return GetBuildingFromId(buildingId);
        }

        private static Building? GetBuildingFromId(ushort buildingId)
        {
            if (buildingId != 0)
            {
                var buildingManager = BuildingManager.instance;
                return buildingManager.m_buildings.m_buffer[buildingId];
            }

            return null;
        }
    }
}
