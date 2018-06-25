namespace RushHour2.Citizens.Extensions
{
    public static class HumanAIExtensions
    {
        public static bool GoHome(this HumanAI humanAI, uint citizenId, ref Citizen citizen)
        {
            var buildingId = citizen.GetBuilding();

            humanAI.StartMoving(citizenId, ref citizen, buildingId, citizen.HomeBuilding());

            return false;
        }
    }
}
