using RushHour2.Core.Settings;

namespace RushHour2.Citizens.Location
{
    public static class GlobalLocationHandler
    {
        public static bool ShouldMove()
        {
            var vehicleCount = VehicleManager.instance.m_vehicleCount;
            var citizenCount = CitizenManager.instance.m_instanceCount;
            var percentageVehicles = (vehicleCount / (double)VehicleManager.MAX_VEHICLE_COUNT) * 100d;
            var percentageCitizens = (citizenCount / (double)CitizenManager.MAX_CITIZEN_COUNT) * 100d;
            var simulationManager = SimulationManager.instance;
            var randomPercentage = ((simulationManager.m_randomizer.Int32(int.MaxValue) / (double)int.MaxValue) * 100d);

            if (UserModSettings.Settings.Citizens_IgnoreVehicleCount)
            {
                return randomPercentage > percentageCitizens;
            }

            return randomPercentage > (percentageCitizens > percentageVehicles ? percentageCitizens : percentageVehicles);
        }

        public static bool GoodBuildingToVisit(ref Building building, ref Citizen citizen)
        {
            var info = building.Info;
            var @class = info.m_class;
            var ageGroup = Citizen.GetAgeGroup(citizen.Age);


            return true;
        }
    }
}
