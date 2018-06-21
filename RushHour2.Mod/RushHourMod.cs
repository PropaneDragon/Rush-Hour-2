using ColossalFramework;
using ICities;
using RushHour2.Core.Logging;

namespace RushHour2.Mod
{
    public class RushHourMod : IUserMod
    {
        public string Name => "Rush Hour";
        public string Description => "Implements Rush Hour traffic and improves Citizen simulations.";

        public void OnEnabled()
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"{Name} has started.");

            var loadingManager = Singleton<LoadingManager>.instance;
            loadingManager.m_introLoaded += OnIntroLoaded;
        }

        private void OnIntroLoaded()
        {
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddCheckbox("Enabled", true, new OnCheckChanged((isChecked) =>
            {

            }));
        }
    }
}
