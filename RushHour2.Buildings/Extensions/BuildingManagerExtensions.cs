using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RushHour2.Buildings.Extensions
{
    public static class BuildingManagerExtensions
    {
        public static List<ushort> FindAllBuildings(this BuildingManager buildingManager, Vector3 position, float maxDistance, ItemClass.Service[] services, ItemClass.SubService[] subServices, Building.Flags flagsRequired, Building.Flags flagsForbidden)
        {
            List<ushort> buildings = new List<ushort>();

            var halfBuildingGrid = BuildingManager.BUILDINGGRID_RESOLUTION / 2f;
            var area = maxDistance * maxDistance;

            var minX = Mathf.Max(((position.x - maxDistance) / BuildingManager.BUILDINGGRID_CELL_SIZE) + halfBuildingGrid, 0);
            var minZ = Mathf.Max(((position.z - maxDistance) / BuildingManager.BUILDINGGRID_CELL_SIZE) + halfBuildingGrid, 0);
            var maxX = Mathf.Min(((position.x + maxDistance) / BuildingManager.BUILDINGGRID_CELL_SIZE) + halfBuildingGrid, BuildingManager.BUILDINGGRID_RESOLUTION - 1);
            var maxZ = Mathf.Min(((position.z + maxDistance) / BuildingManager.BUILDINGGRID_CELL_SIZE) + halfBuildingGrid, BuildingManager.BUILDINGGRID_RESOLUTION - 1);

            for (var z = (int)minZ; z <= maxZ; ++z)
            {
                for (var x = (int)minX; x <= maxX; ++x)
                {
                    var potentialBuildingId = buildingManager.m_buildingGrid[(z * BuildingManager.BUILDINGGRID_RESOLUTION) + x];
                    while (potentialBuildingId != 0)
                    {
                        var building = buildingManager.m_buildings.m_buffer[potentialBuildingId];
                        var buildingInfo = building.Info;

                        if ((services.Contains(buildingInfo.m_class.m_service) || services.Contains(ItemClass.Service.None) || services.Count() == 0) && (subServices.Contains(buildingInfo.m_class.m_subService) || subServices.Contains(ItemClass.SubService.None) || subServices.Count() == 0) && (building.m_flags & (flagsRequired | flagsForbidden)) == flagsRequired)
                        {
                            var magnitude = Vector3.SqrMagnitude(position - building.m_position);
                            if (magnitude < area)
                            {
                                buildings.Add(potentialBuildingId);
                            }
                        }

                        potentialBuildingId = building.m_nextGridBuilding;
                    }
                }
            }

            return buildings;
        }
    }
}
