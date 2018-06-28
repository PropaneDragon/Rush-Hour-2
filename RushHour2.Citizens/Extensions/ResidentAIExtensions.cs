using ColossalFramework;
using Harmony;
using RushHour2.Citizens.Reporting;
using UnityEngine;

namespace RushHour2.Citizens.Extensions
{
    public static class ResidentAIExtensions
    {
        public static bool FindAFunActivity(this ResidentAI residentAI, uint citizenId, ushort proximityBuilding)
        {
            var entertainmentReason = new Traverse(residentAI).Method("GetEntertainmentReason").GetValue<TransferManager.TransferReason>();
            if (entertainmentReason != TransferManager.TransferReason.None)
            {
                new Traverse(residentAI).Method("FindVisitPlace", citizenId, proximityBuilding, entertainmentReason);

                CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.GoingToVisit);

                return true;
            }

            return false;
        }

        public static bool FindAShop(this ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            var buildingId = citizen.GetBuilding();

            return residentAI.FindAShop(citizenId, buildingId);
        }

        public static bool FindAShop(this ResidentAI residentAI, uint citizenId, ushort proximityBuilding)
        {
            var shoppingReason = new Traverse(residentAI).Method("GetShoppingReason").GetValue<TransferManager.TransferReason>();
            if (shoppingReason != TransferManager.TransferReason.None)
            {
                new Traverse(residentAI).Method("FindVisitPlace", citizenId, proximityBuilding, shoppingReason);

                CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.GoingToVisit);

                return true;
            }

            return false;
        }

        public static ushort FindCloseLeisure(this ResidentAI residentAI, uint citizenId, ref Citizen citizen, float distance, Building building)
        {
            return residentAI.FindSomewhereClose(citizenId, ref citizen, distance, building, ItemClass.Service.Commercial, ItemClass.SubService.CommercialLeisure);
        }

        public static ushort FindCloseShop(this ResidentAI residentAI, uint citizenId, ref Citizen citizen, float distance, Building building)
        {
            return residentAI.FindSomewhereClose(citizenId, ref citizen, distance, building, ItemClass.Service.Commercial, ItemClass.SubService.CommercialHigh | ItemClass.SubService.CommercialLow);
        }

        public static ushort FindClosePark(this ResidentAI residentAI, uint citizenId, ref Citizen citizen, float distance, Building building)
        {
            return residentAI.FindSomewhereClose(citizenId, ref citizen, distance, building, ItemClass.Service.Beautification | ItemClass.Service.Monument | ItemClass.Service.Natural, ItemClass.SubService.None);
        }

        public static ushort FindSomewhereClose(this ResidentAI residentAI, uint citizenId, ref Citizen citizen, float distance, Building building, ItemClass.Service service, ItemClass.SubService subService)
        {
            var buildingInstance = citizen.GetBuildingInstance();
            if (buildingInstance.HasValue)
            {
                return residentAI.FindSomewhereClose(citizenId, ref citizen, distance, buildingInstance.Value.m_position, service, subService);
            }

            return 0;
        }

        public static ushort FindSomewhereClose(this ResidentAI residentAI, uint citizenId, ref Citizen citizen, float distance, Vector3 position, ItemClass.Service service, ItemClass.SubService subService)
        {
            var currentBuilding = citizen.GetBuilding();
            if (currentBuilding != 0)
            {
                var buildingManager = Singleton<BuildingManager>.instance;
                var closeBuilding = buildingManager.FindBuilding(position, distance, service, subService, Building.Flags.Active, Building.Flags.Demolishing | Building.Flags.Deleted);

                return closeBuilding;
            }

            return 0;
        }

        public static bool TryVisit(this ResidentAI residentAI, uint citizenId, ref Citizen citizen, ushort buildingId)
        {
            var currentBuilding = citizen.GetBuilding();

            if (buildingId != 0 && currentBuilding != 0 && citizen.CanMove())
            {
                residentAI.StartMoving(citizenId, ref citizen, currentBuilding, buildingId);
                citizen.SetVisitplace(citizenId, buildingId, 0U);
                citizen.m_visitBuilding = buildingId;

                return true;
            }

            return false;
        }
    }
}
