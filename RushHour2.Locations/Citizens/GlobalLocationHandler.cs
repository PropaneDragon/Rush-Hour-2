using RushHour2.Buildings.Extensions;
using RushHour2.Core.Settings;
using System.Collections.Generic;

namespace RushHour2.Locations.Citizens
{
    public static class GlobalLocationHandler
    {
        private static Dictionary<Citizen.AgeGroup, string[]> _badBuildingNames = new Dictionary<Citizen.AgeGroup, string[]>()
        {
            {
                Citizen.AgeGroup.Child,
                new[]
                {
                    "bar", "club", "night"
                }
            },
            {
                Citizen.AgeGroup.Senior,
                new []
                {
                    "playground", "children"
                }
            },
            {
                Citizen.AgeGroup.Teen,
                new []
                {
                    "playground", "children"
                }
            }
        };

        private static string[] _globalBadBuildingNames = new[]
        {
            "parking", "carpark", "parking lot", "car park"
        };

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

        public static bool GoodBuildingToVisit(ushort buildingId)
        {
            if (buildingId != 0)
            {
                var building = BuildingManager.instance.m_buildings.m_buffer[buildingId];

                return GoodBuildingToVisit(ref building);
            }

            return false;
        }

        public static bool GoodBuildingToVisit(ushort buildingId, ref Citizen citizen)
        {
            if (buildingId != 0)
            {
                var building = BuildingManager.instance.m_buildings.m_buffer[buildingId];

                return GoodBuildingToVisit(ref building, ref citizen);
            }

            return false;
        }

        public static bool GoodBuildingToVisit(ref Building building)
        {
            if (building.Visitable())
            {
                var info = building.Info;
                var @class = info.m_class;
                var buildingName = info.name.ToLower();

                foreach (var globalBadName in _globalBadBuildingNames)
                {
                    if (buildingName.Contains(globalBadName))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static bool GoodBuildingToVisit(ref Building building, ref Citizen citizen)
        {
            if (GoodBuildingToVisit(ref building))
            {
                var info = building.Info;
                var @class = info.m_class;
                var ageGroup = Citizen.GetAgeGroup(citizen.Age);
                var buildingName = info.name.ToLower();

                if (_badBuildingNames.ContainsKey(ageGroup))
                {
                    foreach (var badName in _badBuildingNames[ageGroup])
                    {
                        if (buildingName.Contains(badName))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public static List<ushort> FilterAcceptableBuildingsForCitizen(ref Citizen citizen, List<ushort> originalList)
        {
            List<ushort> filteredList = new List<ushort>();

            foreach (var buildingId in originalList)
            {
                if (GoodBuildingToVisit(buildingId, ref citizen))
                {
                    filteredList.Add(buildingId);
                }
            }

            return filteredList;
        }
    }
}
