using Harmony;

namespace RushHour2.Citizens.Extensions
{
    public static class ResidentAIExtensions
    {
        public static bool FindAFunActivity(this ResidentAI residentAI, uint citizenId, ushort proximityBuilding)
        {
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

                return true;
            }

            return false;
        }
    }
}
