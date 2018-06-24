using RushHour2.Citizens.Extensions;

namespace RushHour2.Citizens.Location
{
    public static class LocationHandler
    {
        public static bool Process(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            if (!citizen.Arrested && !citizen.Sick && !citizen.Collapsed && !citizen.Dead && citizen.Exists())
            {
                if (citizen.IsInsideBuilding())
                {
                    return ProcessInBuilding(ref residentAI, citizenId, ref citizen);
                }
                else if (citizen.IsMoving())
                {
                    return ProcessMoving(ref residentAI, citizenId, ref citizen);
                }
            }

            return false;
        }

        private static bool ProcessInBuilding(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            if (citizen.ValidBuilding())
            {
                if (citizen.AtHome())
                {
                    return ProcessAtHome(ref residentAI, citizenId, ref citizen);
                }
                else if (citizen.AtWork())
                {
                    return ProcessAtWork(ref residentAI, citizenId, ref citizen);
                }
                else if (citizen.IsVisiting())
                {
                    return ProcessVisiting(ref residentAI, citizenId, ref citizen);
                }
            }

            return false;
        }

        private static bool ProcessVisiting(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            return false;
        }

        private static bool ProcessAtWork(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            return false;
        }

        private static bool ProcessAtHome(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            return false;
        }

        private static bool ProcessMoving(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            return false;
        }
    }
}
