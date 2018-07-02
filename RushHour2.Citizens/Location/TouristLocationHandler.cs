using ColossalFramework;
using RushHour2.Citizens.Extensions;
using RushHour2.Citizens.Reporting;
using System;

namespace RushHour2.Citizens.Location
{
    public static class TouristLocationHandler
    {
        public static bool Process(ref TouristAI touristAI, uint citizenId, ref Citizen citizen)
        {
            if (!citizen.Arrested && !citizen.Sick && !citizen.Collapsed && !citizen.Dead && !citizen.AtHome() && !citizen.AtWork() && citizen.Exists())
            {
                CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.Unknown);

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
            CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.Visiting);

            if (!citizen.ValidVisitBuilding())
            {
                return false;
            }

            var currentBuildingInstance = citizen.VisitBuildingInstance().Value;
            var simulationManager = SimulationManager.instance;

            if (citizen.Tired() || citizen.Tired(TimeSpan.FromHours(6)))
            {
                if (citizen.Tired() && citizen.InHotel())
                {
                    touristAI.GoToSleep(citizenId);

                    return true;
                }

                var foundHotel = touristAI.FindCloseHotel(citizenId, ref citizen, 2000, currentBuildingInstance);
                if (foundHotel != 0 && simulationManager.m_randomizer.Int32(10) < 8)
                {
                    var foundHotelInstance = BuildingManager.instance.m_buildings.m_buffer[foundHotel];
                    var estimatedTravelTime = TravelTime.EstimateTravelTime(currentBuildingInstance, foundHotelInstance);

                    if (citizen.Tired(estimatedTravelTime))
                    {
                        touristAI.TryVisit(citizenId, ref citizen, foundHotel);

                        return true;
                    }
                }
                else if(citizen.Tired())
                {
                    touristAI.LeaveTheCity(citizenId, ref citizen);

                    return true;
                }
            }

            var goSomewhere = simulationManager.m_randomizer.Int32(10) < 3 || citizen.InHotel();
            if (goSomewhere)
            {
                var keepItLocal = simulationManager.m_randomizer.Int32(10) < 5;
                if (keepItLocal)
                {
                    var ventureDistance = 1000f + (simulationManager.m_randomizer.Int32(3) * 1000);
                    var randomActivityNumber = simulationManager.m_randomizer.Int32(9);
                    ushort closeActivity = 0;

                    if (randomActivityNumber < 3 || simulationManager.m_currentGameTime.Hour >= 21)
                    {
                        closeActivity = touristAI.FindCloseLeisure(citizenId, ref citizen, ventureDistance, currentBuildingInstance);
                    }
                    else if (randomActivityNumber < 6)
                    {
                        closeActivity = touristAI.FindClosePark(citizenId, ref citizen, ventureDistance, currentBuildingInstance);
                    }
                    else
                    {
                        closeActivity = touristAI.FindCloseShop(citizenId, ref citizen, ventureDistance, currentBuildingInstance);
                    }

                    if (closeActivity != 0)
                    {
                        var closeActivityBuilding = BuildingManager.instance.m_buildings.m_buffer[closeActivity];
                        if (!citizen.Tired(TravelTime.EstimateTravelTime(currentBuildingInstance, closeActivityBuilding)))
                        {
                            touristAI.TryVisit(citizenId, ref citizen, closeActivity);
                        }
                    }
                }
                else
                {
                    var goShopping = simulationManager.m_randomizer.Int32(10) < 5;
                    if (goShopping)
                    {
                        touristAI.FindAShop(citizenId, ref citizen);
                    }
                    else
                    {
                        touristAI.FindAFunActivity(citizenId, citizen.GetBuilding());
                    }
                }
            }

            return true;
        }

        private static bool ProcessMoving(ref TouristAI touristAI, uint citizenId, ref Citizen citizen)
        {
            CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.Moving);

            return citizen.m_vehicle != 0 || citizen.IsVisible();
        }
    }
}
