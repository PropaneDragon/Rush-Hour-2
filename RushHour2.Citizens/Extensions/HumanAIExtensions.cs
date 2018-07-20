using Harmony;
using RushHour2.Buildings.Extensions;
using RushHour2.Citizens.Reporting;
using System.Collections.Generic;
using UnityEngine;
using RushHour2.Citizens.Location;

namespace RushHour2.Citizens.Extensions
{
    public static class HumanAIExtensions
    {
        public const float MAX_DISTANCE = BuildingManager.BUILDINGGRID_CELL_SIZE * BuildingManager.BUILDINGGRID_RESOLUTION;

        public static bool LeaveTheCity(this HumanAI humanAI, uint citizenId, ref Citizen citizen)
        {
            var leavingReason = GetLeavingReason(citizenId, ref citizen);
            if (leavingReason != TransferManager.TransferReason.None)
            {
                new Traverse(humanAI).Method("FindVisitPlace", citizenId, citizen.GetBuilding(), leavingReason);

                CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.Leaving);

                return true;
            }

            return false;
        }

        public static TransferManager.TransferReason GetLeavingReason(uint citizenId, ref Citizen citizen)
        {
            switch (citizen.WealthLevel)
            {
                case Citizen.Wealth.Low:
                    return TransferManager.TransferReason.LeaveCity0;
                case Citizen.Wealth.Medium:
                    return TransferManager.TransferReason.LeaveCity1;
                case Citizen.Wealth.High:
                    return TransferManager.TransferReason.LeaveCity2;
                default:
                    return TransferManager.TransferReason.LeaveCity0;
            }
        }

        public static bool GoHome(this HumanAI humanAI, uint citizenId, ref Citizen citizen)
        {
            CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.GoingHome);

            return humanAI.GoToBuilding(citizenId, ref citizen, citizen.HomeBuilding());
        }

        public static bool GoToWork(this HumanAI humanAI, uint citizenId, ref Citizen citizen)
        {
            CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.GoingToWork);

            return humanAI.GoToBuilding(citizenId, ref citizen, citizen.WorkBuilding());
        }

        public static bool GoToBuilding(this HumanAI humanAI, uint citizenId, ref Citizen citizen, ushort buildingId, bool ignoreSameBuilding = false)
        {
            if (citizen.CanMove() && (citizen.GetBuilding() != buildingId || ignoreSameBuilding))
            {
                var currentBuildingId = citizen.GetBuilding();
                var moving = humanAI.StartMoving(citizenId, ref citizen, currentBuildingId, buildingId);

                if (buildingId != citizen.WorkBuilding() && buildingId != citizen.HomeBuilding())
                {
                    citizen.SetVisitplace(citizenId, buildingId, 0U);
                }
                else
                {
                    citizen.SetVisitplace(citizenId, 0, 0U);
                }

                return moving;
            }

            return false;
        }

        public static ushort FindLeisure(this HumanAI humanAI, uint citizenId, ref Citizen citizen, Building building, float distance)
        {
            return humanAI.FindSomewhere(citizenId, ref citizen, building, new[] { ItemClass.Service.Commercial }, new[] { ItemClass.SubService.CommercialLeisure }, distance);
        }

        public static ushort FindHotel(this HumanAI humanAI, uint citizenId, ref Citizen citizen, Building building, float distance)
        {
            return humanAI.FindSomewhere(citizenId, ref citizen, building, new[] { ItemClass.Service.Commercial }, new[] { ItemClass.SubService.CommercialTourist }, distance);
        }

        public static ushort FindShop(this HumanAI humanAI, uint citizenId, ref Citizen citizen, Building building, float distance)
        {
            return humanAI.FindSomewhere(citizenId, ref citizen, building, new[] { ItemClass.Service.Commercial }, new[] { ItemClass.SubService.CommercialHigh, ItemClass.SubService.CommercialLow }, distance);
        }

        public static ushort FindPark(this HumanAI humanAI, uint citizenId, ref Citizen citizen, Building building, float distance)
        {
            return humanAI.FindSomewhere(citizenId, ref citizen, building, new[] { ItemClass.Service.Beautification, ItemClass.Service.Monument, ItemClass.Service.Natural }, new[] { ItemClass.SubService.None }, distance);
        }

        public static bool FindAFunActivity(this HumanAI humanAI, uint citizenId, ref Citizen citizen, ushort proximityBuilding, float distance)
        {
            var visitMonument = SimulationManager.instance.m_randomizer.Int32(10) < (humanAI is TouristAI ? 6 : 2);
            var proximityBuildingInstance = BuildingManager.instance.m_buildings.m_buffer[proximityBuilding];

            if (visitMonument && proximityBuilding != 0)
            {
                var monument = humanAI.FindSomewhere(citizenId, ref citizen, proximityBuildingInstance, new[] { ItemClass.Service.Monument }, new[] { ItemClass.SubService.None }, distance * 5);
                if (monument != 0)
                {
                    humanAI.GoToBuilding(citizenId, ref citizen, monument);

                    return true;
                }
            }

            var foundBuilding = humanAI.FindSomewhere(citizenId, ref citizen, proximityBuildingInstance, new[] { ItemClass.Service.Beautification, ItemClass.Service.Commercial, ItemClass.Service.Natural, ItemClass.Service.Tourism }, new[] { ItemClass.SubService.None }, distance);
            if (foundBuilding != 0)
            {
                humanAI.GoToBuilding(citizenId, ref citizen, foundBuilding);

                return true;
            }

            return false;
        }

        public static bool FindAShop(this HumanAI humanAI, uint citizenId, ref Citizen citizen, float distance)
        {
            var buildingId = citizen.GetBuilding();

            return humanAI.FindAShop(citizenId, ref citizen, buildingId);
        }

        public static bool FindAShop(this HumanAI humanAI, uint citizenId, ref Citizen citizen, ushort proximityBuilding, float distance)
        {
            var proximityBuildingInstance = BuildingManager.instance.m_buildings.m_buffer[proximityBuilding];
            var foundBuilding = humanAI.FindSomewhere(citizenId, ref citizen, proximityBuildingInstance, new[] { ItemClass.Service.Commercial }, new[] { ItemClass.SubService.CommercialEco, ItemClass.SubService.CommercialHigh, ItemClass.SubService.CommercialLow }, distance);

            if (foundBuilding != 0)
            {
                humanAI.GoToBuilding(citizenId, ref citizen, foundBuilding);

                return true;
            }

            return false;
        }

        public static ushort FindSomewhere(this HumanAI humanAI, uint citizenId, ref Citizen citizen, Building building, ItemClass.Service[] services, ItemClass.SubService[] subServices, float distance)
        {
            var buildingInstance = citizen.GetBuildingInstance();
            if (buildingInstance.HasValue)
            {
                return humanAI.FindSomewhere(citizenId, ref citizen, buildingInstance.Value.m_position, services, subServices, distance);
            }

            return 0;
        }

        public static ushort FindSomewhere(this HumanAI humanAI, uint citizenId, ref Citizen citizen, Vector3 position, ItemClass.Service[] services, ItemClass.SubService[] subServices, float distance)
        {
            var simulationManager = SimulationManager.instance;
            var closeBuildings = humanAI.FindAllClosePlaces(citizenId, ref citizen, position, services, subServices, distance);
            if (closeBuildings.Count > 0)
            {
                var randomBuilding = simulationManager.m_randomizer.Int32(0, closeBuildings.Count - 1);
                return closeBuildings[randomBuilding];
            }

            return 0;
        }

        public static List<ushort> FindAllClosePlaces(this HumanAI humanAI, uint citizenId, ref Citizen citizen, Vector3 position, ItemClass.Service[] services, ItemClass.SubService[] subServices, float distance)
        {
            var simulationManager = SimulationManager.instance;
            var currentBuilding = citizen.GetBuilding();
            if (currentBuilding != 0)
            {
                var buildingManager = BuildingManager.instance;
                var closeBuildings = buildingManager.FindAllBuildings(position, distance, services, subServices, Building.Flags.Created, Building.Flags.Demolishing | Building.Flags.Deleted | Building.Flags.Abandoned);

                return GlobalLocationHandler.FilterAcceptableBuildingsForCitizen(ref citizen, closeBuildings);
            }

            return null;
        }

        public static bool TryVisit(this HumanAI humanAI, uint citizenId, ref Citizen citizen, ushort buildingId)
        {
            var currentBuilding = citizen.GetBuilding();

            if (buildingId != 0 && currentBuilding != 0 && currentBuilding != buildingId && GlobalLocationHandler.GoodBuildingToVisit(buildingId, ref citizen))
            {
                var moving = humanAI.GoToBuilding(citizenId, ref citizen, buildingId);

                CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.GoingToVisit);

                return moving;
            }

            return false;
        }

        public static void GoToSleep(this HumanAI humanAI, uint citizenId)
        {
            CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.Sleeping);
        }
    }
}
