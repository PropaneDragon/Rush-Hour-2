using RushHour2.Patches.HarmonyLocal;
using System;
using System.Reflection;

namespace RushHour2.Patches.AI
{
    public class NewCommercialBuildingAI_SimulationStepActive : Patchable
    {
        private static byte _lastProblemTimer = 0;

        public override MethodBase BaseMethod => typeof(CommercialBuildingAI).GetMethod("SimulationStepActive", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, new ParameterModifier[] { });
        public override MethodInfo Prefix => typeof(NewCommercialBuildingAI_SimulationStepActive).GetMethod(nameof(SimulationStepActivePrefix), BindingFlags.Static | BindingFlags.Public);
        public override MethodInfo Postfix => typeof(NewCommercialBuildingAI_SimulationStepActive).GetMethod(nameof(SimulationStepActivePostfix), BindingFlags.Static | BindingFlags.Public);

        public static void SimulationStepActivePrefix(CommercialBuildingAI __instance, ref Building buildingData)
        {
            _lastProblemTimer = buildingData.m_outgoingProblemTimer;
        }

        public static void SimulationStepActivePostfix(CommercialBuildingAI __instance, ref Building buildingData)
        {
            if (buildingData.m_outgoingProblemTimer > _lastProblemTimer)
            {
                var simulationManager = SimulationManager.instance;
                var date = simulationManager.m_currentGameTime;
                if ((date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday) || date.Hour < 12 || date.Hour > 21)
                {
                    buildingData.m_outgoingProblemTimer = _lastProblemTimer;
                }
            }
        }
    }
}
