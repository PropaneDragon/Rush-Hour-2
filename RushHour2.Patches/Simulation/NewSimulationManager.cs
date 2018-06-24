using RushHour2.Core.Reporting;
using RushHour2.Patches.HarmonyLocal;
using System;
using System.Reflection;

namespace RushHour2.Patches.Simulation
{
    public class SimulationManager_FrameToTime : Patchable
    {
        private static object _timeLock = new object();
        private static TimeSpan _idealTimePerFrame = TimeSpan.FromMilliseconds(100);

        public override MethodBase BaseMethod => typeof(SimulationManager).GetMethod("FrameToTime", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(uint) }, new ParameterModifier[] { });
        public override MethodInfo Prefix => typeof(SimulationManager_FrameToTime).GetMethod(nameof(FrameToTimePrefix), BindingFlags.Static | BindingFlags.Public);

        public static void FrameToTimePrefix(uint frame, ref TimeSpan ___m_timePerFrame)
        {
            lock (_timeLock)
            {
                if (___m_timePerFrame.TotalMilliseconds != _idealTimePerFrame.TotalMilliseconds)
                {
                    LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Changing time per frame from {___m_timePerFrame.TotalMilliseconds}ms to {_idealTimePerFrame.TotalMilliseconds}ms per tick");

                    ___m_timePerFrame = _idealTimePerFrame;
                }
            }
        }
    }
}
