using RushHour2.Core.Reporting;
using RushHour2.Patches.HarmonyLocal;
using System;
using System.Reflection;

namespace RushHour2.Patches.Simulation
{
    public class SimulationManager_FrameToTime : Patchable
    {
        public override MethodBase BaseMethod => typeof(SimulationManager).GetMethod("FrameToTime", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(uint) }, new ParameterModifier[] { });
        public override MethodInfo Prefix => typeof(SimulationManager_FrameToTime).GetMethod(nameof(FrameToTimePrefix), BindingFlags.Static | BindingFlags.Public);
        public override MethodInfo Postfix => typeof(SimulationManager_FrameToTime).GetMethod(nameof(FrameToTimePostfix), BindingFlags.Static | BindingFlags.Public);

        public static void FrameToTimePrefix(ref SimulationManager __instance, uint frame)
        {
            var idealTimePerFrame = new TimeSpan(TimeSpan.FromHours(24).Ticks / SimulationManager.DAYTIME_FRAMES);

            if (__instance.m_timePerFrame.TotalMilliseconds != idealTimePerFrame.TotalMilliseconds)
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Changing time per frame from {__instance.m_timePerFrame.TotalMilliseconds}ms to {idealTimePerFrame.TotalMilliseconds}ms per tick");

                __instance.m_timePerFrame = idealTimePerFrame;
            }
        }

        public static void FrameToTimePostfix(uint frame, ref DateTime __result)
        {
            float currentHour = frame * SimulationManager.DAYTIME_FRAME_TO_HOUR;
            __result = new DateTime(__result.Year, __result.Month, __result.Day).AddHours(currentHour);
        }
    }
    public class SimulationManager_Update : Patchable
    {
        private static double _lastDayTimeHour = 0;

        public override MethodBase BaseMethod => typeof(SimulationManager).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, new ParameterModifier[] { });
        public override MethodInfo Postfix => typeof(SimulationManager_Update).GetMethod(nameof(UpdatePostfix), BindingFlags.Static | BindingFlags.Public);

        public static void UpdatePostfix(ref SimulationManager __instance)
        {
            __instance.m_currentGameTime = __instance.m_currentGameTime.Date.AddHours(__instance.m_currentDayTimeHour);

            if (__instance.m_currentDayTimeHour < _lastDayTimeHour && _lastDayTimeHour > 23.9 && __instance.m_currentDayTimeHour < 0.1)
            {
                __instance.m_currentGameTime = __instance.m_currentGameTime.AddDays(1);
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"A new day has begun ({__instance.m_currentGameTime.ToString("dddd")})");
            }

            _lastDayTimeHour = __instance.m_currentDayTimeHour;
        }
    }
}
