using ColossalFramework;
using RushHour2.Buildings.Extensions;
using RushHour2.Core.Settings;
using RushHour2.Locations.Citizens;
using RushHour2.Locations.Travel;
using System;

namespace RushHour2.Citizens.Extensions
{
    public static class CitizenExtensions
    {
        public static bool Exists(this Citizen citizen) => citizen.m_homeBuilding != 0 || citizen.m_workBuilding != 0 || citizen.m_visitBuilding != 0 || citizen.m_instance != 0 || citizen.m_vehicle != 0;

        public static bool NeedsGoods(this Citizen citizen) => citizen.m_flags.IsFlagSet(Citizen.Flags.NeedGoods) && SimulationManager.instance.m_randomizer.Int32(10) > 7;

        public static bool IsAtABuilding(this Citizen citizen) => !citizen.IsMoving();

        public static bool IsVisible(this Citizen citizen) => citizen.m_instance != 0;

        public static bool CanMove(this Citizen citizen) => citizen.IsVisible() || GlobalLocationHandler.ShouldMove();

        public static bool IsMoving(this Citizen citizen) => citizen.CurrentLocation == Citizen.Location.Moving;

        public static bool ShouldGoHome(this Citizen citizen)
        {
            if (citizen.ValidHomeBuilding() && citizen.Tired(TimeSpan.FromHours(6)))
            {
                var currentBuildingInstance = citizen.GetBuildingInstance();
                var homeBuildingInstance = citizen.HomeBuildingInstance();

                if (currentBuildingInstance.HasValue)
                {
                    var travelTime = TravelTime.EstimateTravelTime(currentBuildingInstance.Value, homeBuildingInstance.Value);
                    return citizen.Tired(travelTime);
                }
                else
                {
                    return citizen.Tired(TimeSpan.FromHours(2));
                }
            }

            return false;
        }

        public static bool ShouldGoToWork(this Citizen citizen, uint citizenId)
        {
            if (citizen.ValidWorkBuilding() && (citizen.ShouldBeAtWork(citizenId) || citizen.ShouldBeAtWork(citizenId, TimeSpan.FromHours(6))))
            {
                var currentBuildingInstance = citizen.GetBuildingInstance();
                var workBuildingInstance = citizen.WorkBuildingInstance();

                if (currentBuildingInstance.HasValue)
                {
                    var travelTime = TravelTime.EstimateTravelTime(currentBuildingInstance.Value, workBuildingInstance.Value);
                    return citizen.ShouldBeAtWork(citizenId, travelTime);
                }
                else
                {
                    return citizen.ShouldBeAtWork(citizenId, TimeSpan.FromHours(2));
                }
            }

            return false;
        }

        public static bool ShouldBeAtWork(this Citizen citizen, uint citizenId)
        {
            return citizen.ShouldBeAtWork(citizenId, new TimeSpan(0));
        }

        public static bool ShouldBeAtWork(this Citizen citizen, uint citizenID, TimeSpan offset)
        {
            var simulationManager = SimulationManager.instance;
            var currentTime = simulationManager.m_currentGameTime;

            return citizen.ShouldBeAtWork(citizenID, currentTime + offset);
        }

        public static bool ShouldBeAtWork(this Citizen citizen, uint citizenId, DateTime time)
        {
            var weekend = time.DayOfWeek == DayOfWeek.Saturday || time.DayOfWeek == DayOfWeek.Sunday;

            if (citizen.ValidWorkBuilding())
            {
                var ageGroup = Citizen.GetAgeGroup(citizen.m_age);
                if (ageGroup <= Citizen.AgeGroup.Teen)
                {
                    return !weekend && UserModSettings.TimeIsBetween(time, UserModSettings.Settings.StartTime_Schools, UserModSettings.Settings.Duration_Schools) || (citizen.AtWork() && UserModSettings.TimeIsBefore(time, UserModSettings.Settings.StartTime_Schools));
                }
                else if (ageGroup <= Citizen.AgeGroup.Young && citizen.Education3)
                {
                    return !weekend && UserModSettings.TimeIsBetween(time, UserModSettings.Settings.StartTime_University, UserModSettings.Settings.Duration_University) || (citizen.AtWork() && UserModSettings.TimeIsBefore(time, UserModSettings.Settings.StartTime_University));
                }
                else if (ageGroup <= Citizen.AgeGroup.Senior)
                {
                    var workInstance = citizen.WorkBuildingInstance();
                    return workInstance.HasValue ? citizen.WorkBuildingInstance().Value.ShouldBeAtWork(citizenId, time) : false;
                }
            }

            return false;
        }

        public static bool Tired(this Citizen citizen)
        {
            return citizen.Tired(new TimeSpan(0));
        }

        public static bool Tired(this Citizen citizen, TimeSpan offset)
        {
            var simulationManager = SimulationManager.instance;
            var currentTime = simulationManager.m_currentGameTime;

            return citizen.Tired(currentTime + offset);
        }

        public static bool Tired(this Citizen citizen, DateTime time)
        {
            var timeOfDay = time.TimeOfDay;
            var timeLater = time.AddHours(6);
            var weekendLater = timeLater.DayOfWeek == DayOfWeek.Saturday || timeLater.DayOfWeek == DayOfWeek.Sunday;
            var needsToWorkSoon = !weekendLater && citizen.ValidWorkBuilding();
            var ageGroup = Citizen.GetAgeGroup(citizen.m_age);
            var wealth = citizen.WealthLevel;
            var health = citizen.m_health;
            var healthLevel = Citizen.GetHealthLevel(health);
            var wellbeing = citizen.m_wellbeing;
            var happiness = Citizen.GetHappiness(health, wellbeing);
            var happinessLevel = Citizen.GetHappinessLevel(happiness);
            var education = citizen.EducationLevel;

            if (ageGroup <= Citizen.AgeGroup.Child)
            {
                return timeOfDay.TotalHours >= 18 + ((int)happinessLevel / 2f) || timeOfDay.TotalHours < 6; //Happier they are, the longer they want to stay awake
            }
            else if (ageGroup <= Citizen.AgeGroup.Teen)
            {
                var adjustedHour = timeOfDay.TotalHours < 12 ? timeOfDay.TotalHours + 24 : timeOfDay.TotalHours;

                return adjustedHour >= 26 - (int)happinessLevel && adjustedHour < (24 + 6); //Happier they are, the more sleep they get
            }
            else if (ageGroup <= Citizen.AgeGroup.Young)
            {
                if (needsToWorkSoon)
                {
                    return timeOfDay.TotalHours >= 23 - ((int)happinessLevel / 2d) || timeOfDay.TotalHours < 7 - ((int)happinessLevel / 2.5d); //Happier they are, the earlier they go to bed and the earlier they wake
                }

                return timeOfDay.TotalHours >= 22 + (((int)happinessLevel / 2d) + (int)wealth) || timeOfDay.TotalHours < 10 - ((int)happinessLevel / 1.5d);
            }
            else if (ageGroup <= Citizen.AgeGroup.Adult)
            {
                if (needsToWorkSoon)
                {
                    return timeOfDay.TotalHours >= 22 || timeOfDay.TotalHours < 7 - ((int)happinessLevel / 2.5d); //Happier they are, the earlier they wake
                }

                return timeOfDay.TotalHours >= 20 + (((int)happinessLevel / 2d) + (int)wealth) || timeOfDay.TotalHours < 10 - ((int)happinessLevel / 1.5d);
            }
            else if (ageGroup <= Citizen.AgeGroup.Senior)
            {
                return timeOfDay.TotalHours >= 18 + ((int)healthLevel / 2d) || timeOfDay.TotalHours < 8 - ((int)healthLevel / 2d); //Unhealthy seniors go to bed earlier and get up later
            }

            return timeOfDay.TotalHours >= 22 || timeOfDay.TotalHours < 5;
        }

        public static bool GettingWet(this Citizen citizen)
        {
            if (citizen.IsVisible() && UserModSettings.Settings.Citizens_ReactToWeather)
            {
                var weatherManager = WeatherManager.instance;
                var simulationManager = SimulationManager.instance;
                var currentRainPercentage = weatherManager.m_currentRain * 100d;

                return currentRainPercentage > 0d && simulationManager.m_randomizer.Int32(0, 100) < currentRainPercentage;
            }

            return false;
        }

        public static bool AfraidOfGettingWet(this Citizen citizen)
        {
            var weatherManager = WeatherManager.instance;
            var currentRain = weatherManager.m_currentRain;

            return UserModSettings.Settings.Citizens_ReactToWeather && currentRain > 0.15;
        }
    }
}
