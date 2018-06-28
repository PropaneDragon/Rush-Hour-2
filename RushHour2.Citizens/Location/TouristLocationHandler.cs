using System;
using ColossalFramework;
using RushHour2.Citizens.Extensions;
using RushHour2.Citizens.Reporting;

namespace RushHour2.Citizens.Location
{
    public static class TouristLocationHandler
    {
        public static bool Process(ref TouristAI touristAI, uint citizenId, ref Citizen citizen)
        {
            if (!citizen.Arrested && !citizen.Sick && !citizen.Collapsed && !citizen.Dead && !citizen.AtHome() && !citizen.AtWork() && citizen.Exists())
            {
                if (citizen.IsInsideBuilding())
                {
                    return ProcessInBuilding(ref touristAI, citizenId, ref citizen);
                }
                else if (citizen.IsMoving())
                {
                    return ProcessMoving(ref touristAI, citizenId, ref citizen);
                }

                return true;
            }

            return false;
        }

        private static bool ProcessInBuilding(ref TouristAI touristAI, uint citizenId, ref Citizen citizen)
        {
            CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.Visiting);

            if (citizen.ValidBuilding())
            {
                var buildingInstance = citizen.GetBuildingInstance();
                if (buildingInstance?.m_flags.IsFlagSet(Building.Flags.Evacuating) ?? false)
                {
                    return false;
                }

                if (citizen.IsVisiting())
                {
                    return ProcessVisiting(ref touristAI, citizenId, ref citizen);
                }
            }

            return false;
        }

        private static bool ProcessVisiting(ref TouristAI touristAI, uint citizenId, ref Citizen citizen)
        {
            var simulationManager = SimulationManager.instance;

            return true;
        }

        private static bool ProcessMoving(ref TouristAI touristAI, uint citizenId, ref Citizen citizen)
        {
            return citizen.m_vehicle != 0 || citizen.IsVisible();
        }
    }
}
