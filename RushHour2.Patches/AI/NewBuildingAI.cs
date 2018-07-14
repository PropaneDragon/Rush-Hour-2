using ColossalFramework.Math;
using RushHour2.Core.Settings;
using RushHour2.Patches.HarmonyLocal;
using System;
using System.Reflection;
using UnityEngine;

namespace RushHour2.Patches.AI
{
    public class NewBuildingAI_CalculateUnspawnPosition: Patchable
    {
        public override MethodBase BaseMethod => typeof(BuildingAI).GetMethod("CalculateUnspawnPosition", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Randomizer).MakeByRefType(), typeof(CitizenInfo), typeof(ushort), typeof(Vector3).MakeByRefType(), typeof(Vector3).MakeByRefType(), typeof(Vector2).MakeByRefType(), typeof(CitizenInstance.Flags).MakeByRefType() }, new ParameterModifier[] { });
        public override MethodInfo Postfix => typeof(NewBuildingAI_CalculateUnspawnPosition).GetMethod(nameof(CalculateUnspawnPositionPostfix), BindingFlags.Static | BindingFlags.Public);

        public static void CalculateUnspawnPositionPostfix(BuildingAI __instance, ushort buildingID, ref Building data, ref Randomizer randomizer, CitizenInfo info, ref Vector3 position, ref Vector3 target, ref CitizenInstance.Flags specialFlags)
        {
            if (UserModSettings.Settings.Enabled)
            {
                var simulationManager = SimulationManager.instance;
                var weatherManager = WeatherManager.instance;
                var rainPercentage = weatherManager.m_currentRain * 100f;

                if (data.m_eventIndex == 0 && rainPercentage > 0.5)
                {
                    __instance.CalculateSpawnPosition(buildingID, ref data, ref randomizer, info, out var spawnPosition, out var spawnTarget);

                    position = spawnPosition;
                    target = spawnTarget;

                    specialFlags &= ~(CitizenInstance.Flags.HangAround | CitizenInstance.Flags.SittingDown);
                }
            }
        }
    }
}
