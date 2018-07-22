using RushHour2.Buildings.Extensions;
using RushHour2.Citizens.Extensions;
using RushHour2.Core.Settings;
using RushHour2.Patches.HarmonyLocal;
using System;
using System.Reflection;
using UnityEngine;

namespace RushHour2.Patches.AI
{
    public class NewCommonBuildingAI_GetColor: Patchable
    {
        public override MethodBase BaseMethod => typeof(CommonBuildingAI).GetMethod("GetColor", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(InfoManager.InfoMode) }, new ParameterModifier[] { });
        public override MethodInfo Postfix => typeof(NewCommonBuildingAI_GetColor).GetMethod(nameof(GetColorPostfix), BindingFlags.Static | BindingFlags.Public);

        public static void GetColorPostfix(ref CommonBuildingAI __instance, ushort buildingID, ref Building data, InfoManager.InfoMode infoMode, ref Color __result)
        {
            if (UserModSettings.Settings.Enabled && UserModSettings.Settings.Buildings_OverrideLights && infoMode == InfoManager.InfoMode.None && __result.a > 0f)
            {
                var lightsOn = true;
                var buildingAI = data.Info.m_buildingAI;
                var service = data.Info.m_class.m_service;
                var subService = data.Info.m_class.m_subService;

                switch (service)
                {
                    case ItemClass.Service.Commercial:
                        var allowedLightsOff = UserModSettings.Settings.Buildings_OverrideCommercialLights && subService != ItemClass.SubService.CommercialTourist && subService != ItemClass.SubService.CommercialLeisure;
                        lightsOn = !allowedLightsOff || data.HasCitizensInside(CitizenUnit.Flags.Visit | CitizenUnit.Flags.Work);
                        break;
                    case ItemClass.Service.Education:
                        lightsOn = !UserModSettings.Settings.Buildings_OverrideSchoolLights || (!SimulationManager.instance.m_isNightTime && data.HasCitizensInside(CitizenUnit.Flags.Visit | CitizenUnit.Flags.Student | CitizenUnit.Flags.Work));
                        break;
                    case ItemClass.Service.Office:
                        lightsOn = !UserModSettings.Settings.Buildings_OverrideOfficeLights || data.HasCitizensInside(CitizenUnit.Flags.Work);
                        break;
                    case ItemClass.Service.Industrial:
                        lightsOn = !UserModSettings.Settings.Buildings_OverrideIndustrialLights || data.HasCitizensInside(CitizenUnit.Flags.Work);
                        break;
                    case ItemClass.Service.Residential:
                        if (UserModSettings.Settings.Buildings_OverrideResidentialLights)
                        {
                            var citizensInside = data.HasCitizensInside(CitizenUnit.Flags.Home | CitizenUnit.Flags.Visit);
                            var highrise = subService == ItemClass.SubService.ResidentialHigh || subService == ItemClass.SubService.ResidentialHighEco;

                            if (!highrise)
                            {
                                var allAsleep = !citizensInside || !data.AnyCitizenInside(CitizenUnit.Flags.Home | CitizenUnit.Flags.Visit, citizenId =>
                                {
                                    var citizen = CitizenManager.instance.m_citizens.m_buffer[citizenId];
                                    return !citizen.Tired();
                                });

                                lightsOn = !allAsleep;
                            }
                            else
                            {
                                lightsOn = citizensInside;
                            }
                        }
                        break;
                }

                __result.a = lightsOn ? 1f : 0f;
            }
        }
    }
}
