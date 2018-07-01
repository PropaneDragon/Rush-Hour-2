using Harmony;
using RushHour2.Citizens.Reporting;

namespace RushHour2.Citizens.Extensions
{
    public static class TouristAIExtensions
    {
        public static bool FindAFunActivity(this TouristAI touristAI, uint citizenId, ushort proximityBuilding)
        {
            var entertainmentReason = new Traverse(touristAI).Method("GetEntertainmentReason").GetValue<TransferManager.TransferReason>();
            if (entertainmentReason != TransferManager.TransferReason.None)
            {
                new Traverse(touristAI).Method("FindVisitPlace", citizenId, proximityBuilding, entertainmentReason);

                CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.AttemptingToGoForEntertainment);

                return true;
            }

            return false;
        }

        public static bool FindAShop(this TouristAI touristAI, uint citizenId, ref Citizen citizen)
        {
            var buildingId = citizen.GetBuilding();

            return touristAI.FindAShop(citizenId, buildingId);
        }

        public static bool FindAShop(this TouristAI touristAI, uint citizenId, ushort proximityBuilding)
        {
            var shoppingReason = new Traverse(touristAI).Method("GetShoppingReason").GetValue<TransferManager.TransferReason>();
            if (shoppingReason != TransferManager.TransferReason.None)
            {
                new Traverse(touristAI).Method("FindVisitPlace", citizenId, proximityBuilding, shoppingReason);

                CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.AttemptingToGoShopping);

                return true;
            }

            return false;
        }
    }
}
