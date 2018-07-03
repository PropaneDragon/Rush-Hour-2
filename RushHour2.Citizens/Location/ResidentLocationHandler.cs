using ColossalFramework;
using RushHour2.Citizens.Extensions;
using RushHour2.Citizens.Reporting;
using System;

namespace RushHour2.Citizens.Location
{
    public static class ResidentLocationHandler
    {
        public static bool Process(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            if (!citizen.Arrested && !citizen.Sick && !citizen.Collapsed && !citizen.Dead && citizen.Exists())
            {
                CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.Unknown);

                if (citizen.IsInsideBuilding())
                {
                    ProcessInBuilding(ref residentAI, citizenId, ref citizen);
                }
                else if (citizen.IsMoving())
                {
                    return ProcessMoving(ref residentAI, citizenId, ref citizen);
                }

                return true;
            }

            return false;
        }

        private static bool ProcessInBuilding(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            if (citizen.ValidBuilding())
            {
                var buildingInstance = citizen.GetBuildingInstance();
                if (buildingInstance?.m_flags.IsFlagSet(Building.Flags.Evacuating) ?? false)
                {
                    return false;
                }

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
            CitizenActivityMonitor.LogActivity(citizenId, GetBuildingActivity(ref citizen));

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
                var simulationManager = SimulationManager.instance;
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
                    var simulationManager = SimulationManager.instance;
                    var time = simulationManager.m_currentGameTime;
                    var ageGroup = Citizen.GetAgeGroup(citizen.Age);
                    var happiness = Citizen.GetHappiness(citizen.m_health, citizen.m_wellbeing);
                    var happinessLevel = Citizen.GetHappinessLevel(happiness);
                    
                    if (happinessLevel >= Citizen.Happiness.Good)
                    {
                        if (time.DayOfWeek == DayOfWeek.Friday && (ageGroup == Citizen.AgeGroup.Adult || ageGroup == Citizen.AgeGroup.Young))
                        {
                            var goOut = simulationManager.m_randomizer.Int32(10) <= 6;
                            if (goOut)
                            {
                                var allLeisure = residentAI.FindAllClosePlaces(citizenId, ref citizen, 400f, citizen.WorkBuildingInstance().Value.m_position, ItemClass.Service.Commercial, ItemClass.SubService.CommercialLeisure);
                                var currentBuildingInt = (int)citizen.GetBuilding();
                                var closest = -1;
                                var chosenBuilding = (ushort)0;
                                
                                foreach (var leisure in allLeisure)
                                {
                                    var leisureInt = (int)leisure;
                                    var difference = Math.Abs(leisureInt - currentBuildingInt);
                                    if (closest < 0 || difference < closest)
                                    {
                                        closest = difference;
                                        chosenBuilding = leisure;
                                    }
                                }
                                
                                if (chosenBuilding != 0 && residentAI.TryVisit(citizenId, ref citizen, chosenBuilding))
                                {
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
                residentAI.GoToSleep(citizenId);

                return true;
            }
            else
            {
                var simulationManager = SimulationManager.instance;
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
        }

        private static bool ProcessActivity(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            var simulationManager = SimulationManager.instance;
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
                    var ventureDistance = 60f * ((int)happinessLevel + 1);
                    var closeActivity = randomActivityNumber < 50 ? residentAI.FindClosePark(citizenId, ref citizen, ventureDistance, currentBuildingInstance.Value) : residentAI.FindCloseShop(citizenId, ref citizen, ventureDistance, currentBuildingInstance.Value);

                    if (closeActivity != 0)
                    {
                        var closeActivityBuilding = BuildingManager.instance.m_buildings.m_buffer[closeActivity];
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
                        var ventureDistance = 100f * ((int)happinessLevel + 1);
                        ushort closeActivity = 0;

                        if (randomActivityNumber < 26 || simulationManager.m_currentGameTime.Hour >= 21)
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
                            var closeActivityBuilding = BuildingManager.instance.m_buildings.m_buffer[closeActivity];
                            if (!citizen.Tired(TravelTime.EstimateTravelTime(currentBuildingInstance.Value, closeActivityBuilding)))
                            {
                                residentAI.TryVisit(citizenId, ref citizen, closeActivity);
                            }
                        }
                    }
                    else
                    {
                        var goShopping = simulationManager.m_randomizer.Int32(10) < 5;

                        if (goShopping)
                        {
                            residentAI.FindAShop(citizenId, ref citizen);
                        }
                        else
                        {
                            residentAI.FindAFunActivity(citizenId, currentBuilding);
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private static bool ProcessMoving(ref ResidentAI residentAI, uint citizenId, ref Citizen citizen)
        {
            CitizenActivityMonitor.LogActivity(citizenId, CitizenActivityMonitor.Activity.Moving);

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

        private static CitizenActivityMonitor.Activity GetBuildingActivity(ref Citizen citizen)
        {
            var building = citizen.GetBuildingInstance();
            if (building.HasValue)
            {
                var info = building.Value.Info;
                var service = info.m_class.m_service;
                
                switch (service)
                {
                    case ItemClass.Service.Beautification:
                        return CitizenActivityMonitor.Activity.VisitingPark;
                    case ItemClass.Service.Citizen:
                        return CitizenActivityMonitor.Activity.VisitingCitizen;
                    case ItemClass.Service.Commercial:
                        return CitizenActivityMonitor.Activity.VisitingShop;
                    case ItemClass.Service.Disaster:
                        return CitizenActivityMonitor.Activity.InShelter;
                    case ItemClass.Service.Education:
                        return CitizenActivityMonitor.Activity.VisitingSchool;
                    case ItemClass.Service.Electricity:
                        return CitizenActivityMonitor.Activity.VisitingElectricity;
                    case ItemClass.Service.FireDepartment:
                        return CitizenActivityMonitor.Activity.VisitingFireDepartment;
                    case ItemClass.Service.Garbage:
                        return CitizenActivityMonitor.Activity.VisitingGarbage;
                    case ItemClass.Service.HealthCare:
                        return CitizenActivityMonitor.Activity.VisitingHealthcare;
                    case ItemClass.Service.Industrial:
                        return CitizenActivityMonitor.Activity.VisitingIndustrial;
                    case ItemClass.Service.Monument:
                        return CitizenActivityMonitor.Activity.VisitingMonumnet;
                    case ItemClass.Service.Natural:
                        return CitizenActivityMonitor.Activity.VisitingNature;
                    case ItemClass.Service.Office:
                        return CitizenActivityMonitor.Activity.VisitingOffice;
                    case ItemClass.Service.PoliceDepartment:
                        return CitizenActivityMonitor.Activity.VisitingPolice;
                    case ItemClass.Service.PublicTransport:
                        return CitizenActivityMonitor.Activity.OnPublicTransport;
                    case ItemClass.Service.Residential:
                        return CitizenActivityMonitor.Activity.AtAHouse;
                    case ItemClass.Service.Road:
                        return CitizenActivityMonitor.Activity.OnARoad;
                    case ItemClass.Service.Tourism:
                        return CitizenActivityMonitor.Activity.AtATouristAttraction;
                }
            }

            return CitizenActivityMonitor.Activity.Visiting;
        }
    }
}
