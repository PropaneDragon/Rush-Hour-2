using RushHour2.Buildings.Extensions;
using RushHour2.Citizens.Extensions;
using RushHour2.Core.Reporting;
using System;

namespace RushHour2.Citizens.Location
{
    public static class LocationHandler
    {
        private static readonly int ESTIMATED_DISTANCE_PER_MINUTE = 200; //Calculated distance I think you can probably get in an average city per minute.

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
            var building = citizen.GetBuildingInstance();
            if (building.HasValue)
            {
                var service = building.Value.Service();
                if (ValidService(service))
                {
                    if (service == ItemClass.Service.Residential)
                    {

                    }
                }
            }

            return false;
        }

        private static bool ProcessAtWork(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            if (!citizen.ShouldBeAtWork())
            {
                if (citizen.NeedsGoods())
                {
                    var proximityBuilding = citizen.ValidHomeBuilding() ? citizen.HomeBuilding() : citizen.WorkBuilding(); //Prioritise something close to home

                    if (residentAI.FindAShop(citizenId, proximityBuilding))
                    {
                        return true;
                    }
                }

                if (citizen.Tired(TimeSpan.FromHours(3)))
                {
                    if (residentAI.GoHome(citizenId, ref citizen))
                    {
                        return true;
                    }
                }
                else
                {

                }
            }
            else
            {
                return true;
            }

            return false;
        }

        private static bool ProcessAtHome(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            if (citizen.ValidWorkBuilding() && citizen.ShouldBeAtWork(TimeSpan.FromHours(5)))
            {
                var homeBuildingInstance = citizen.HomeBuildingInstance();
                var workBuildingInstance = citizen.WorkBuildingInstance();

                if (homeBuildingInstance.HasValue && workBuildingInstance.HasValue)
                {
                    var homeBuildingLocation = homeBuildingInstance.Value.m_position;
                    var workBuildingPosition = workBuildingInstance.Value.m_position;
                    var difference = (homeBuildingLocation - workBuildingPosition).magnitude;
                    var estimatedTimeToTravelIrl = ESTIMATED_DISTANCE_PER_MINUTE * difference;

                    if (citizen.ShouldBeAtWork(TimeSpan.FromHours(4)))
                    {

                    }
                }
            }

            return false;
        }

        private static bool ProcessMoving(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            return false;
        }

        private static bool ValidService(ItemClass.Service service)
        {
            return service != ItemClass.Service.PoliceDepartment &&
                   service != ItemClass.Service.Disaster &&
                   service != ItemClass.Service.HealthCare;
        }
    }
}
