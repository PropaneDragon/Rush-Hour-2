using RushHour2.Core.Reporting;
using RushHour2.Core.Settings;
using RushHour2.Patches.HarmonyLocal;
using System;
using System.Reflection;

namespace RushHour2.Patches.Simulation
{
    public class SimulationManager_Update : Patchable
    {
        public override MethodBase BaseMethod => typeof(SimulationManager).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, new ParameterModifier[] { });
        public override MethodInfo Postfix => typeof(SimulationManager_Update).GetMethod(nameof(UpdatePostfix), BindingFlags.Static | BindingFlags.Public);

        public static void UpdatePostfix(ref SimulationManager __instance)
        {
            var idealTimePerFrame = new TimeSpan((long)((double)(TimeSpan.FromHours(24).Ticks / SimulationManager.DAYTIME_FRAMES) * UserModSettings.TimeSpeedMultiplier));

            if (__instance.m_timePerFrame.TotalMilliseconds != idealTimePerFrame.TotalMilliseconds)
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Changing time per frame from {__instance.m_timePerFrame.TotalMilliseconds}ms to {idealTimePerFrame.TotalMilliseconds}ms per tick");

                __instance.m_timePerFrame = idealTimePerFrame;
            }

            __instance.m_currentGameTime = new DateTime((long)(__instance.m_referenceFrameIndex + __instance.m_referenceTimer) * __instance.m_timePerFrame.Ticks);
            
            if (__instance.m_currentDayTimeHour > __instance.m_currentGameTime.TimeOfDay.TotalHours)
            {
                __instance.m_currentGameTime = __instance.m_currentGameTime.AddDays(-1);
            }

            __instance.m_currentGameTime = __instance.m_currentGameTime.AddHours(__instance.m_currentDayTimeHour - __instance.m_currentGameTime.TimeOfDay.TotalHours);
        }
    }
}