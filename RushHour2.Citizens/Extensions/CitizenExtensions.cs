namespace RushHour2.Citizens.Extensions
{
    public static class CitizenExtensions
    {
        public static bool Exists(this Citizen citizen)
        {
            return citizen.m_homeBuilding != 0 || citizen.m_workBuilding != 0 || citizen.m_visitBuilding != 0 || citizen.m_instance != 0 || citizen.m_vehicle != 0;
        }

        public static bool IsInsideBuilding(this Citizen citizen)
        {
            return !citizen.IsMoving();
        }

        public static bool IsMoving(this Citizen citizen)
        {
            return citizen.CurrentLocation == Citizen.Location.Moving;
        }

        public static bool IsVisible(this Citizen citizen)
        {
            return citizen.m_instance != 0;
        }

        public static bool AtHome(this Citizen citizen)
        {
            return citizen.CurrentLocation == Citizen.Location.Home;
        }

        public static bool ValidHomeBuilding(this Citizen citizen)
        {
            return citizen.m_homeBuilding != 0;
        }

        public static bool AtWork(this Citizen citizen)
        {
            return citizen.CurrentLocation == Citizen.Location.Work;
        }

        public static bool ValidWorkBuilding(this Citizen citizen)
        {
            return citizen.m_workBuilding != 0;
        }

        public static bool IsVisiting(this Citizen citizen)
        {
            return citizen.CurrentLocation == Citizen.Location.Visit;
        }

        public static bool ValidVisitBuilding(this Citizen citizen)
        {
            return citizen.m_visitBuilding != 0;
        }

        public static bool ValidBuilding(this Citizen citizen)
        {
            var validHome = citizen.AtHome() && citizen.ValidHomeBuilding();
            var validWork = citizen.AtWork() && citizen.ValidWorkBuilding();
            var validVisit = citizen.IsVisiting() && citizen.ValidVisitBuilding();

            return validHome || validWork || validVisit;
        }
    }
}
