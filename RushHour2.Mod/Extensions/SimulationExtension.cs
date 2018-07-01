using ColossalFramework;
using ColossalFramework.IO;
using RushHour2.Citizens.Reporting;
using RushHour2.Core.Info;
using RushHour2.Core.Reporting;
using RushHour2.Core.Settings;
using System;
using UnityEngine;

namespace RushHour2.Mod.Extensions
{
    class SimulationExtension : ISimulationManager
    {
        private static readonly TimeSpan CITIZEN_LOG_INTERVAL = TimeSpan.FromSeconds(10);

        private int _step = 0;
        private DateTime _nextLogTime = DateTime.Now + CITIZEN_LOG_INTERVAL;

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
            var _simulation = SimulationManager.instance;

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
                        if (_step < Mathf.RoundToInt(1f / timeMultiplier))
                        {
                            ++_step;
                            _simulation.m_dayTimeOffsetFrames = (_simulation.m_dayTimeOffsetFrames - 1u) % SimulationManager.DAYTIME_FRAMES;
                        }
                        else
                        {
                            _step = 0;
                        }
                    }
                }
            }

            if (_nextLogTime < DateTime.Now)
            {
                CitizenActivityMonitor.WriteLog(LoggingWrapper.LogArea.Hidden);

                _nextLogTime = DateTime.Now + CITIZEN_LOG_INTERVAL;
            }
        }

        public void UpdateData(SimulationManager.UpdateMode mode)
        {
        }
    }
}