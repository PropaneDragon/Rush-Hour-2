using ColossalFramework;
using ICities;

namespace RushHour2.Core
{
    public class RushHourMod : IUserMod
    {
        public string Name => "Rush Hour";
        public string Description => "Implements Rush Hour traffic and improves Citizen simulations.";

        public void OnEnabled()
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, "Rush Hour has been enabled.");

            Singleton<LoadingManager>.Ensure();
            Singleton<LoadingManager>.instance.m_introLoaded += OnIntroLoaded;
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
