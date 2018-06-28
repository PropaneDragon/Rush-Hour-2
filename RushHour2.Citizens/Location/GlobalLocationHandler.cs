using ColossalFramework;

namespace RushHour2.Citizens.Location
{
    public static class GlobalLocationHandler
    {
        public static bool ShouldMove()
        {
            var vehicleCount = (uint)Singleton<VehicleManager>.instance.m_vehicleCount;
            var citizenCount = (uint)Singleton<CitizenManager>.instance.m_instanceCount;
            var simulationManager = Singleton<SimulationManager>.instance;

            if (vehicleCount * CitizenManager.MAX_INSTANCE_COUNT > citizenCount * VehicleManager.MAX_VEHICLE_COUNT)
            {
                return simulationManager.m_randomizer.UInt32(VehicleManager.MAX_VEHICLE_COUNT) >= vehicleCount;
            }
            else
            {
                return simulationManager.m_randomizer.UInt32(CitizenManager.MAX_INSTANCE_COUNT) >= citizenCount;
            }
        }
    }
}
