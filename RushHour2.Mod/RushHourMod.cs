using ColossalFramework.PlatformServices;
using ICities;
using RushHour2.Core.Info;
using RushHour2.Core.Reporting;
using RushHour2.Mod.Extensions;
using RushHour2.Patches;

namespace RushHour2.Mod
{
    public class RushHourMod : IUserMod
    {
        private static PublishedFileId experimentalId = new PublishedFileId(1432430887);

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

            if (Details.ExperimentalBuild)
            {
                MessageBoxWrapper.Show(MessageBoxWrapper.MessageType.Warning,
                    $"{Details.ModName} - experimental build",
                    $"You currently have an experimental build of {Details.ModName} running.\n\n" +
                    $"This build may have bugs and issues, as well as incomplete features as it is intended for testing only. " +
                    $"Please subscribe to the non-experimental version of the mod if you don't intend on testing {Details.ModName}.");
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddCheckbox("Enabled", true, new OnCheckChanged((isChecked) =>
            {

            }));
        }
    }
}
