using RushHour2.Citizens.Reporting;

namespace RushHour2.Citizens.Extensions
{
    public static class HumanAIExtensions
    {
        public static bool GoHome(this HumanAI humanAI, uint citizenId, ref Citizen citizen)
        {
            CitizenMonitor.LogActivity(citizenId, CitizenMonitor.Activity.GoingHome);

            return humanAI.GoToBuilding(citizenId, ref citizen, citizen.HomeBuilding());
        }

        public static bool GoToWork(this HumanAI humanAI, uint citizenId, ref Citizen citizen)
        {
            CitizenMonitor.LogActivity(citizenId, CitizenMonitor.Activity.GoingToWork);

            return humanAI.GoToBuilding(citizenId, ref citizen, citizen.WorkBuilding());
        }

        public static bool GoToBuilding(this HumanAI humanAI, uint citizenId, ref Citizen citizen, ushort buildingId)
        {
            var currentBuildingId = citizen.GetBuilding();
            return humanAI.StartMoving(citizenId, ref citizen, currentBuildingId, buildingId);
        }
    }
}
