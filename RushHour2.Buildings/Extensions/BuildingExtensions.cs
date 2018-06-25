namespace RushHour2.Buildings.Extensions
{
    public static class BuildingExtensions
    {
        public static ItemClass.Service Service(this Building building) => building.Info.m_class.m_service;

        public static ItemClass.SubService SubService(this Building building) => building.Info.m_class.m_subService;
    }
}
