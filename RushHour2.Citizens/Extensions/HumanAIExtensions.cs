using Harmony;
using RushHour2.Buildings.Extensions;
using RushHour2.Citizens.Reporting;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RushHour2.Citizens.Extensions
{
    public static class HumanAIExtensions
    {
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

        public static bool GoToBuilding(this HumanAI humanAI, uint citizenId, ref Citizen citizen, ushort buildingId)
        {
            if (citizen.CanMove())
            {
                var currentBuildingId = citizen.GetBuilding();
                return humanAI.StartMoving(citizenId, ref citizen, currentBuildingId, buildingId);
            }

            return false;
        }

        public static ushort FindCloseLeisure(this HumanAI humanAI, uint citizenId, ref Citizen citizen, float distance, Building building)
        {
            return humanAI.FindSomewhereClose(citizenId, ref citizen, distance, building, ItemClass.Service.Commercial, ItemClass.SubService.CommercialLeisure);
        }

        public static ushort FindCloseHotel(this HumanAI humanAI, uint citizenId, ref Citizen citizen, float distance, Building building)
        {
            return humanAI.FindSomewhereClose(citizenId, ref citizen, distance, building, ItemClass.Service.Commercial, ItemClass.SubService.CommercialTourist);
        }

        public static ushort FindCloseShop(this HumanAI humanAI, uint citizenId, ref Citizen citizen, float distance, Building building)
        {
            return humanAI.FindSomewhereClose(citizenId, ref citizen, distance, building, ItemClass.Service.Commercial, ItemClass.SubService.CommercialHigh | ItemClass.SubService.CommercialLow);
        }

        public static ushort FindClosePark(this HumanAI humanAI, uint citizenId, ref Citizen citizen, float distance, Building building)
        {
            return humanAI.FindSomewhereClose(citizenId, ref citizen, distance, building, ItemClass.Service.Beautification | ItemClass.Service.Monument | ItemClass.Service.Natural, ItemClass.SubService.None);
        }

        public static ushort FindSomewhereClose(this HumanAI humanAI, uint citizenId, ref Citizen citizen, float distance, Building building, ItemClass.Service service, ItemClass.SubService subService)
        {
            var buildingInstance = citizen.GetBuildingInstance();
            if (buildingInstance.HasValue)
            {
                return humanAI.FindSomewhereClose(citizenId, ref citizen, distance, buildingInstance.Value.m_position, service, subService);
            }

            return 0;
        }

        public static ushort FindSomewhereClose(this HumanAI humanAI, uint citizenId, ref Citizen citizen, float distance, Vector3 position, ItemClass.Service service, ItemClass.SubService subService)
        {
            var simulationManager = SimulationManager.instance;
            var closeBuildings = humanAI.FindAllClosePlaces(citizenId, ref citizen, distance, position, service, subService);
            if (closeBuildings.Count > 0)
            {
                var randomBuilding = simulationManager.m_randomizer.Int32(0, closeBuildings.Count);
                return closeBuildings[randomBuilding];
            }

            return 0;
        }

        public static List<ushort> FindAllClosePlaces(this HumanAI humanAI, uint citizenId, ref Citizen citizen, float distance, Vector3 position, ItemClass.Service service, ItemClass.SubService subService)
        {
            var simulationManager = SimulationManager.instance;
            var currentBuilding = citizen.GetBuilding();
            if (currentBuilding != 0)
            {
                var buildingManager = BuildingManager.instance;
                var closeBuildings = buildingManager.FindAllBuildings(position, distance, service, subService, Building.Flags.Active, Building.Flags.Demolishing | Building.Flags.Deleted);

                return closeBuildings;
            }

            return null;
        }

        public static bool TryVisit(this HumanAI humanAI, uint citizenId, ref Citizen citizen, ushort buildingId)
        {
            var currentBuilding = citizen.GetBuilding();

            if (buildingId != 0 && currentBuilding != 0 && citizen.CanMove())
            {
                humanAI.StartMoving(citizenId, ref citizen, currentBuilding, buildingId);
                citizen.SetVisitplace(citizenId, buildingId, 0U);
                citizen.m_visitBuilding = buildingId;

                CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.GoingToVisit);

                return true;
            }

            return false;
        }

        public static void GoToSleep(this HumanAI humanAI, uint citizenId)
        {
            CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.Sleeping);
        }
    }
}
