namespace RushHour2.Buildings.Extensions
{
    public static class BuildingExtensions
    {
        public static bool Enterable(this Building building) => building.Info.m_enterDoors.Length > 0;

        public static bool Visitable(this Building building) => building.Enterable() || building.Info.m_specialPlaces.Length > 0;

        public static ItemClass.Service Service(this Building building) => building.Info.m_class.m_service;

        public static ItemClass.SubService SubService(this Building building) => building.Info.m_class.m_subService;
    }
}
