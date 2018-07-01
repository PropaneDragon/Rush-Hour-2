using Harmony;
using RushHour2.Citizens.Reporting;

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
