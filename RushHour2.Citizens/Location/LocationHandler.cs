using ColossalFramework;
using RushHour2.Citizens.Extensions;
using RushHour2.Citizens.Reporting;
using System;

namespace RushHour2.Citizens.Location
{
    public static class LocationHandler
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

        public static bool Process(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            if (!citizen.Arrested && !citizen.Sick && !citizen.Collapsed && !citizen.Dead && citizen.Exists())
            {
                if (citizen.IsInsideBuilding())
                {
                    ProcessInBuilding(ref residentAI, citizenId, ref citizen);
                }
                else if (citizen.IsMoving())
                {
                    ProcessMoving(ref residentAI, citizenId, ref citizen);
                }

                return true;
            }

            return false;
        }

        private static bool ProcessInBuilding(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.Visiting);

            if (citizen.ValidBuilding())
            {
                if (citizen.AtHome())
                {
                    return ProcessAtHome(ref residentAI, citizenId, ref citizen);
                }
                else if (citizen.AtWork())
                {
                    return ProcessAtWork(ref residentAI, citizenId, ref citizen);
                }
                else if (citizen.IsVisiting())
                {
                    return ProcessVisiting(ref residentAI, citizenId, ref citizen);
                }
            }

            return false;
        }

        private static bool ProcessVisiting(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.Visiting);

            if (citizen.ValidWorkBuilding() && citizen.ShouldGoToWork())
            {
                residentAI.GoToWork(citizenId, ref citizen);

                return true;
            }
            else if (citizen.ShouldGoHome())
            {
                residentAI.GoHome(citizenId, ref citizen);

                return true;
            }
            else
            {
                var simulationManager = Singleton<SimulationManager>.instance;
                var shouldStay = simulationManager.m_randomizer.UInt32(10) < 5;
                
                if (!shouldStay)
                {
                    var ageGroup = Citizen.GetAgeGroup(citizen.Age);
                    var happiness = Citizen.GetHappiness(citizen.m_health, citizen.m_wellbeing);
                    var happinessLevel = Citizen.GetHappinessLevel(happiness);
                    var visitSomewhereElse = simulationManager.m_randomizer.UInt32(100) < ((int)happinessLevel * 18) / ((int)ageGroup + 1);

                    if (!visitSomewhereElse || !ProcessActivity(ref residentAI, citizenId, ref citizen))
                    {
                        residentAI.GoHome(citizenId, ref citizen);
                    }
                }

                return true;
            }
        }

        private static bool ProcessAtWork(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.AtWork);

            if (!citizen.ShouldBeAtWork())
            {
                if (citizen.NeedsGoods())
                {
                    var proximityBuilding = citizen.ValidHomeBuilding() ? citizen.HomeBuilding() : citizen.WorkBuilding(); //Prioritise something close to home, rather than work

                    if (residentAI.FindAShop(citizenId, proximityBuilding))
                    {
                        return true;
                    }
                }

                if (citizen.ShouldGoHome())
                {
                    residentAI.GoHome(citizenId, ref citizen);

                    return true;
                }
                else
                {
                    var simulationManager = Singleton<SimulationManager>.instance;
                    var time = simulationManager.m_currentGameTime;
                    var ageGroup = Citizen.GetAgeGroup(citizen.Age);
                    var happiness = Citizen.GetHappiness(citizen.m_health, citizen.m_wellbeing);
                    var happinessLevel = Citizen.GetHappinessLevel(happiness);
                    
                    if (happinessLevel >= Citizen.Happiness.Good)
                    {
                        if (time.DayOfWeek == DayOfWeek.Friday && (ageGroup == Citizen.AgeGroup.Adult || ageGroup == Citizen.AgeGroup.Young))
                        {
                            var goOut = simulationManager.m_randomizer.Int32(5) <= 2;
                            if (goOut)
                            {
                                var closeLeisure = residentAI.FindCloseLeisure(citizenId, ref citizen, 1000f, citizen.WorkBuildingInstance().Value);
                                if (closeLeisure != 0 && residentAI.TryVisit(citizenId, ref citizen, closeLeisure))
                                {
                                    CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.GoingOutAfterWork);
                                    return true;
                                }
                            }
                        }

                        var shouldGoSomewhereElseFirst = simulationManager.m_randomizer.Int32(10) <= 3 + (int)happinessLevel;
                        if (shouldGoSomewhereElseFirst)
                        {
                            if (residentAI.FindAFunActivity(citizenId, citizen.HomeBuilding()))
                            {
                                return true;
                            }
                        }
                    }

                    residentAI.GoHome(citizenId, ref citizen);

                    return true;
                }
            }

            return true;
        }

        private static bool ProcessAtHome(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.AtHome);

            if (citizen.ValidWorkBuilding() && citizen.ShouldGoToWork())
            {
                residentAI.GoToWork(citizenId, ref citizen);

                return true;
            }
            else if (citizen.Tired())
            {
                return true;
            }
            else if (citizen.NeedsGoods())
            {
                if (residentAI.FindAShop(citizenId, citizen.HomeBuilding()))
                {
                    return true;
                }
            }
            else
            {
                var simulationManager = Singleton<SimulationManager>.instance;
                var happiness = Citizen.GetHappiness(citizen.m_health, citizen.m_wellbeing);
                var wealth = citizen.WealthLevel;
                var happinessLevel = Citizen.GetHappinessLevel(happiness);
                var dayOfWeek = simulationManager.m_currentGameTime.DayOfWeek;
                var weekend = !citizen.ValidWorkBuilding() || dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
                var goSomewhere = simulationManager.m_randomizer.Int32(16) < ((int)happinessLevel + (int)wealth) * (weekend ? 2 : 1);

                if (goSomewhere)
                {
                    ProcessActivity(ref residentAI, citizenId, ref citizen);
                }

                return true;
            }

            return false;
        }

        private static bool ProcessActivity(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            var simulationManager = Singleton<SimulationManager>.instance;
            var ageGroup = Citizen.GetAgeGroup(citizen.Age);
            var happiness = Citizen.GetHappiness(citizen.m_health, citizen.m_wellbeing);
            var wealth = citizen.WealthLevel;
            var happinessLevel = Citizen.GetHappinessLevel(happiness);
            var randomActivityNumber = simulationManager.m_randomizer.Int32(100);
            var currentBuilding = citizen.GetBuilding();
            var currentBuildingInstance = citizen.GetBuildingInstance();

            if (currentBuildingInstance.HasValue)
            {
                if (ageGroup <= Citizen.AgeGroup.Child || ageGroup > Citizen.AgeGroup.Adult)
                {
                    var ventureDistance = 500f * ((int)happinessLevel + 1);
                    var closeActivity = randomActivityNumber < 50 ? residentAI.FindClosePark(citizenId, ref citizen, ventureDistance, currentBuildingInstance.Value) : residentAI.FindCloseShop(citizenId, ref citizen, ventureDistance, currentBuildingInstance.Value);

                    if (closeActivity != 0)
                    {
                        var closeActivityBuilding = Singleton<BuildingManager>.instance.m_buildings.m_buffer[closeActivity];
                        if (!citizen.Tired(TravelTime.EstimateTravelTime(currentBuildingInstance.Value, closeActivityBuilding)))
                        {
                            residentAI.TryVisit(citizenId, ref citizen, closeActivity);
                        }
                    }

                    return true;
                }
                else if (ageGroup <= Citizen.AgeGroup.Adult)
                {
                    if (randomActivityNumber < 80)
                    {
                        var ventureDistance = 1000f * ((int)happinessLevel + 1);
                        ushort closeActivity = 0;

                        if (randomActivityNumber < 26 || simulationManager.m_currentGameTime.Hour > 21)
                        {
                            closeActivity = residentAI.FindCloseLeisure(citizenId, ref citizen, ventureDistance, currentBuildingInstance.Value);
                        }
                        else if (randomActivityNumber < 52)
                        {
                            closeActivity = residentAI.FindClosePark(citizenId, ref citizen, ventureDistance, currentBuildingInstance.Value);
                        }
                        else
                        {
                            closeActivity = residentAI.FindCloseShop(citizenId, ref citizen, ventureDistance, currentBuildingInstance.Value);
                        }

                        if (closeActivity != 0)
                        {
                            var closeActivityBuilding = Singleton<BuildingManager>.instance.m_buildings.m_buffer[closeActivity];
                            if (!citizen.Tired(TravelTime.EstimateTravelTime(currentBuildingInstance.Value, closeActivityBuilding)))
                            {
                                residentAI.TryVisit(citizenId, ref citizen, closeActivity);
                            }
                        }
                    }
                    else
                    {
                        residentAI.FindAFunActivity(citizenId, currentBuilding);
                    }

                    return true;
                }
            }

            return false;
        }

        private static bool ProcessMoving(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            if (citizen.ValidWorkBuilding() && citizen.ShouldGoToWork())
            {
                residentAI.GoToWork(citizenId, ref citizen);

                return true;
            }

            return false;
        }

        private static bool ValidService(ItemClass.Service service)
        {
            return service != ItemClass.Service.PoliceDepartment &&
                   service != ItemClass.Service.Disaster &&
                   service != ItemClass.Service.HealthCare;
        }
    }
}
