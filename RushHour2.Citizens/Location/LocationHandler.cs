using ColossalFramework;
using RushHour2.Buildings.Extensions;
using RushHour2.Citizens.Extensions;
using RushHour2.Citizens.Reporting;
using System;

namespace RushHour2.Citizens.Location
{
    public static class LocationHandler
    {
        public static bool ShouldMove()
        {
            var vehicleCount = (uint)Singleton<VehicleManager>.instance.m_vehicleCount;
            var citizenCount = (uint)Singleton<CitizenManager>.instance.m_instanceCount;
            var simulationManager = Singleton<SimulationManager>.instance;

            if (vehicleCount * CitizenManager.MAX_INSTANCE_COUNT > citizenCount * VehicleManager.MAX_VEHICLE_COUNT)
            {
                return simulationManager.m_randomizer.UInt32(VehicleManager.MAX_VEHICLE_COUNT) >= vehicleCount;
            }
            else
            {
                return simulationManager.m_randomizer.UInt32(CitizenManager.MAX_INSTANCE_COUNT) >= citizenCount;
            }
        }

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
            CitizenMonitor.LogActivity(citizenId, CitizenMonitor.Activity.Visiting);

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
            CitizenMonitor.LogActivity(citizenId, CitizenMonitor.Activity.Visiting);

            if (citizen.ValidWorkBuilding() && citizen.ShouldGoToWork())
            {
                residentAI.GoToWork(citizenId, ref citizen);

                return true;
            }
            else if (citizen.Tired(TimeSpan.FromHours(3)))
            {
                residentAI.GoHome(citizenId, ref citizen);

                return true;
            }
            else
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
            }

            return false;
        }

        private static bool ProcessAtWork(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            CitizenMonitor.LogActivity(citizenId, CitizenMonitor.Activity.AtWork);

            if (!citizen.ShouldBeAtWork())
            {
                if (citizen.NeedsGoods())
                {
                    var proximityBuilding = citizen.ValidHomeBuilding() ? citizen.HomeBuilding() : citizen.WorkBuilding(); //Prioritise something close to home, rather than work

                    if (residentAI.FindAShop(citizenId, proximityBuilding))
                    {
                        return true;
                    }
                }

                if (citizen.Tired(TimeSpan.FromHours(3)))
                {
                    residentAI.GoHome(citizenId, ref citizen);

                    return true;
                }
                else
                {

                    residentAI.GoHome(citizenId, ref citizen);

                    return true;
                }
            }

            return true;
        }

        private static bool ProcessAtHome(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            CitizenMonitor.LogActivity(citizenId, CitizenMonitor.Activity.AtHome);

            if (citizen.ValidWorkBuilding() && citizen.ShouldGoToWork())
            {
                residentAI.GoToWork(citizenId, ref citizen);

                return true;
            }
            else if (citizen.Tired())
            {
                return true;
            }

            return false;
        }

        private static bool ProcessMoving(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            if (citizen.ValidWorkBuilding() && citizen.ShouldGoToWork())
            {
                residentAI.GoToWork(citizenId, ref citizen);

                return true;
            }
            else if (citizen.Tired(TimeSpan.FromHours(3)))
            {
                if (residentAI.GoHome(citizenId, ref citizen))
                {
                    return true;
                }
            }

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
