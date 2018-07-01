using ColossalFramework;
using ICities;
using RushHour2.Core.Info;
using RushHour2.Core.Reporting;
using RushHour2.Mod.Extensions;
using RushHour2.Patches;

namespace RushHour2.Mod
{
    public class RushHourMod : IUserMod
    {
        public string Name => Details.ModName;
        public string Description => $"{Details.ModDescription} v{Details.Version.ToString(3)}";

        public void OnEnabled()
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"{Name} has started.");

            LoadingManager.instance.m_introLoaded += OnIntroLoaded;
        }

        public void OnDisabled()
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"{Name} has ended.");

            PatchManager.UnPatchAll();
            
            LoadingManager.instance.m_introLoaded -= OnIntroLoaded;
        }

        private void OnIntroLoaded()
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"Patching...");

            PatchManager.PatchAll();

            SimulationManager.RegisterSimulationManager(new SimulationExtension());
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddCheckbox("Enabled", true, new OnCheckChanged((isChecked) =>
            {

            }));
        }
    }
}
