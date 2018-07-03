using Harmony;
using RushHour2.Citizens.Reporting;

namespace RushHour2.Citizens.Extensions
{
    public static class ResidentAIExtensions
    {
        public static bool FindAFunActivity(this ResidentAI residentAI, uint citizenId, ref Citizen citizen, ushort proximityBuilding)
        {
            var visitMonument = SimulationManager.instance.m_randomizer.Int32(10) < 3;
            if (visitMonument && proximityBuilding != 0)
            {
                var proximityBuildingInstance = BuildingManager.instance.m_buildings.m_buffer[proximityBuilding];
                var monument = residentAI.FindSomewhereClose(citizenId, ref citizen, BuildingManager.BUILDINGGRID_RESOLUTION * BuildingManager.BUILDINGGRID_CELL_SIZE, proximityBuildingInstance, new[] { ItemClass.Service.Monument }, new[] { ItemClass.SubService.None });
                if (monument != 0)
                {
                    residentAI.GoToBuilding(citizenId, ref citizen, monument);

                    return true;
                }
            }

            var entertainmentReason = new Traverse(residentAI).Method("GetEntertainmentReason").GetValue<TransferManager.TransferReason>();
            if (entertainmentReason != TransferManager.TransferReason.None)
            {
                new Traverse(residentAI).Method("FindVisitPlace", citizenId, proximityBuilding, entertainmentReason);

                CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.AttemptingToGoForEntertainment);

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

                CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.AttemptingToGoShopping);

                return true;
            }

            return false;
        }
    }
}
