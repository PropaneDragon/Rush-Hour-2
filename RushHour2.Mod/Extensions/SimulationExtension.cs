using ColossalFramework;
using ColossalFramework.IO;
using RushHour2.Core.Info;
using RushHour2.Core.Settings;
using UnityEngine;

namespace RushHour2.Mod.Extensions
{
    class SimulationExtension : ISimulationManager
    {
        private int step = 0;

        public void GetData(FastList<IDataContainer> data)
        {
        }

        public string GetName()
        {
            return $"{Details.ModName}SimulationExtension";
        }

        public ThreadProfiler GetSimulationProfiler()
        {
            return null;
        }

        public void EarlyUpdateData()
        {
        }

        public void LateUpdateData(SimulationManager.UpdateMode mode)
        {
        }

        public void SimulationStep(int subStep)
        {
            var _simulation = Singleton<SimulationManager>.instance;

            if (_simulation.m_enableDayNight)
            {
                if (!_simulation.SimulationPaused && !_simulation.ForcedSimulationPaused)
                {
                    var timeMultiplier = UserModSettings.TimeSpeedMultiplier;

                    if (timeMultiplier >= 1f)
                    {
                        _simulation.m_dayTimeOffsetFrames = (_simulation.m_dayTimeOffsetFrames + (uint)Mathf.RoundToInt(timeMultiplier)) % SimulationManager.DAYTIME_FRAMES;
                    }
                    else
                    {
                        if (step < Mathf.RoundToInt(1f / timeMultiplier))
                        {
                            ++step;
                            _simulation.m_dayTimeOffsetFrames = (_simulation.m_dayTimeOffsetFrames - 1u) % SimulationManager.DAYTIME_FRAMES;
                        }
                        else
                        {
                            step = 0;
                        }
                    }
                }
            }
        }

        public void UpdateData(SimulationManager.UpdateMode mode)
        {
        }
    }
}