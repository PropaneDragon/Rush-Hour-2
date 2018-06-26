using RushHour2.Core.Reporting;
using RushHour2.Patches.HarmonyLocal;
using System;
using System.Reflection;

namespace RushHour2.Patches.Simulation
{
    public class SimulationManager_FrameToTime : Patchable
    {
        private static object _timeLock = new object();
        private static TimeSpan _idealTimePerFrame = new TimeSpan(TimeSpan.FromHours(24).Ticks / SimulationManager.DAYTIME_FRAMES);

        public override MethodBase BaseMethod => typeof(SimulationManager).GetMethod("FrameToTime", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(uint) }, new ParameterModifier[] { });
        public override MethodInfo Prefix => typeof(SimulationManager_FrameToTime).GetMethod(nameof(FrameToTimePrefix), BindingFlags.Static | BindingFlags.Public);
        public override MethodInfo Postfix => typeof(SimulationManager_FrameToTime).GetMethod(nameof(FrameToTimePostfix), BindingFlags.Static | BindingFlags.Public);

        public static void FrameToTimePrefix(SimulationManager __instance, uint frame)
        {
            lock (_timeLock)
            {
                if (__instance.m_timePerFrame.TotalMilliseconds != _idealTimePerFrame.TotalMilliseconds)
                {
                    LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Changing time per frame from {__instance.m_timePerFrame.TotalMilliseconds}ms to {_idealTimePerFrame.TotalMilliseconds}ms per tick");

                    __instance.m_timePerFrame = _idealTimePerFrame;
                }
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
        public override MethodBase BaseMethod => typeof(SimulationManager).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, new ParameterModifier[] { });
        public override MethodInfo Postfix => typeof(SimulationManager_Update).GetMethod(nameof(UpdatePostfix), BindingFlags.Static | BindingFlags.Public);

        public static void UpdatePostfix(ref SimulationManager __instance)
        {
            __instance.m_currentGameTime = new DateTime(__instance.m_currentGameTime.Year, __instance.m_currentGameTime.Month, __instance.m_currentGameTime.Day).AddHours(__instance.m_currentDayTimeHour);
            __instance.m_metaData.m_currentDateTime = __instance.m_currentGameTime;
        }
    }
}
