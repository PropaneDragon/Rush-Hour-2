using RushHour2.Core.Settings;
using System;
using System.Collections.Generic;

namespace RushHour2.Buildings.Extensions
{
    public static class BuildingExtensions
    {
        public enum WorkShift
        {
            WeekdayDay,
            WeekendDay
        }

        public static bool Enterable(this Building building) => building.Info?.m_enterDoors != null && building.Info.m_enterDoors.Length > 0;

        public static bool Visitable(this Building building) => building.Enterable() || (building.Info?.m_specialPlaces != null && building.Info.m_specialPlaces.Length > 0);
    
        public static ItemClass.Service Service(this Building building) => building.Info.m_class.m_service;

        public static ItemClass.SubService SubService(this Building building) => building.Info.m_class.m_subService;

        public static bool ShouldBeAtWork(this Building building, uint citizenId, DateTime time)
        {
            if (citizenId != 0)
            {
                var allBuildingWorkers = building.GetAllWorkers();
                var service = building.Info.m_class.m_service;
                var timeOfDay = time.TimeOfDay;
                var weekend = time.DayOfWeek == DayOfWeek.Saturday || time.DayOfWeek == DayOfWeek.Sunday;
                var shift = weekend ? WorkShift.WeekendDay : WorkShift.WeekdayDay;
                var workingToday = false;

                switch (service)
                {
                    case ItemClass.Service.Commercial:
                    case ItemClass.Service.Industrial:
                        workingToday = building.GetWorkersForShift(shift).Contains(citizenId);
                        break;
                    default:
                        workingToday = !weekend && building.GetAllWorkers().Contains(citizenId);
                        break;
                }

                if (workingToday)
                {
                    switch (service)
                    {
                        case ItemClass.Service.Commercial:
                            return UserModSettings.TimeIsBetween(time, weekend ? UserModSettings.Settings.StartTime_Commercial_Weekend : UserModSettings.Settings.StartTime_Commercial_Weekday, weekend ? UserModSettings.Settings.Duration_Commercial_Weekend : UserModSettings.Settings.Duration_Commercial_Weekday);
                        case ItemClass.Service.Industrial:
                            return UserModSettings.TimeIsBetween(time, weekend ? UserModSettings.Settings.StartTime_Industrial_Weekend : UserModSettings.Settings.StartTime_Industrial_Weekday, weekend ? UserModSettings.Settings.Duration_Industrial_Weekend : UserModSettings.Settings.Duration_Industrial_Weekday);
                        default:
                            return UserModSettings.TimeIsBetween(time, UserModSettings.Settings.StartTime_Offices_Weekday, UserModSettings.Settings.Duration_Offices_Weekday);
                    }
                }
            }

            return false;
        }

        public static List<uint> GetWorkersForShift(this Building building, WorkShift shift)
        {
            var allWorkers = building.GetAllWorkers();
            var totalWorkers = allWorkers.Count;
            var weekdaySplitPercentage = 60;
            var weekdayCount = (int)Math.Ceiling((weekdaySplitPercentage / 100f) * totalWorkers);
            var weekendCount = totalWorkers - weekdayCount;

            if (totalWorkers > 0)
            {
                switch (shift)
                {
                    case WorkShift.WeekdayDay:
                        return allWorkers.GetRange(0, weekdayCount);
                    case WorkShift.WeekendDay:
                        if (weekendCount > 0)
                        {
                            return allWorkers.GetRange(weekdayCount - 1, weekendCount);
                        }
                        break;
                }
            }

            return allWorkers;
        }

        public static List<uint> GetAllWorkers(this Building building)
        {
            var workers = building.GetAllCitizens(CitizenUnit.Flags.Work);
            var actualWorkers = new List<uint>();

            foreach (var worker in workers)
            {
                if (worker > 0)
                {
                    var citizen = CitizenManager.instance.m_citizens.m_buffer[worker];
                    var citizenWorkBuilding = citizen.m_workBuilding;
                            
                    if (building.Same(citizenWorkBuilding))
                    {
                        actualWorkers.Add(worker);
                    }
                }
            }

            return actualWorkers;
        }

        public static List<uint> GetAllCitizensInside(this Building building, CitizenUnit.Flags flags)
        {
            var citizens = building.GetAllCitizens(flags);
            var citizensInBuilding = new List<uint>();

            foreach (var citizenId in citizens)
            {
                if (citizenId > 0)
                {
                    var citizen = CitizenManager.instance.m_citizens.m_buffer[citizenId];
                    var citizenBuilding = citizen.GetBuilding();
                    
                    if (building.Same(citizenBuilding))
                    {
                        citizensInBuilding.Add(citizenId);
                    }
                }
            }

            return citizensInBuilding;
        }

        public static List<uint> GetAllCitizens(this Building building, CitizenUnit.Flags flags)
        {
            var citizenUnitId = building.m_citizenUnits;
            var citizenManager = CitizenManager.instance;
            var citizens = new List<uint>();

            while (citizenUnitId != 0)
            {
                var citizenUnit = CitizenManager.instance.m_units.m_buffer[citizenUnitId];
                if ((citizenUnit.m_flags & flags) != CitizenUnit.Flags.None)
                {
                    var citizenIds = new []
                    {
                        citizenUnit.m_citizen0,
                        citizenUnit.m_citizen1,
                        citizenUnit.m_citizen2,
                        citizenUnit.m_citizen3,
                        citizenUnit.m_citizen4
                    };

                    foreach (var citizenId in citizenIds)
                    {
                        if (citizenId > 0)
                        {
                            citizens.Add(citizenId);
                        }
                    }
                }

                citizenUnitId = citizenUnit.m_nextUnit;
            }

            return citizens;
        }

        public static bool HasCitizensInside(this Building building, CitizenUnit.Flags flags)
        {
            return building.AnyCitizenInside(flags, citizenId =>
            {
                return true;
            });
        }

        public static bool AnyCitizenInside(this Building building, CitizenUnit.Flags flags, Func<uint, bool> checkFunction)
        {
            var citizenUnitId = building.m_citizenUnits;
            while (citizenUnitId != 0)
            {
                var citizenUnit = CitizenManager.instance.m_units.m_buffer[citizenUnitId];
                if ((citizenUnit.m_flags & flags) != CitizenUnit.Flags.None)
                {
                    var citizenIds = new[]
                    {
                        citizenUnit.m_citizen0,
                        citizenUnit.m_citizen1,
                        citizenUnit.m_citizen2,
                        citizenUnit.m_citizen3,
                        citizenUnit.m_citizen4
                    };

                    foreach (var citizenId in citizenIds)
                    {
                        if (citizenId > 0)
                        {
                            var citizen = CitizenManager.instance.m_citizens.m_buffer[citizenId];
                            var citizenBuilding = citizen.GetBuilding();

                            if (building.Same(citizenBuilding) && checkFunction.Invoke(citizenId))
                            {
                                return true;
                            }
                        }
                    }
                }

                citizenUnitId = citizenUnit.m_nextUnit;
            }

            return false;
        }

        public static bool Same(this Building building, ushort buildingId)
        {
            if (buildingId > 0)
            {
                var buildingInstance = BuildingManager.instance.m_buildings.m_buffer[buildingId];

                return buildingInstance.Equals(building);
            }

            return false;
        }
    }
}
