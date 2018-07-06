using System.Collections.Generic;
using ColossalFramework.PlatformServices;
using ICities;
using RushHour2.Core.Info;
using RushHour2.Core.Reporting;
using RushHour2.Mod.Extensions;
using RushHour2.Patches;
using RushHour2.UI.Settings;

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
            CheckForExperimentalVersion();
            CheckForIncompatibilities();

            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"Patching...");

            PatchManager.PatchAll();

            SimulationManager.RegisterSimulationManager(new SimulationExtension());
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            SettingsCreation.Create(helper);
        }

        private static void CheckForExperimentalVersion()
        {
            if (Details.ExperimentalBuild)
            {
                MessageBoxWrapper.Show(MessageBoxWrapper.MessageType.Warning,
                    $"{Details.BaseModName} - experimental build",
                    $"You currently have an experimental build of {Details.BaseModName} running.\n\n" +
                    $"This build may have bugs and issues, as well as incomplete features as it is intended for testing only. " +
                    $"Please subscribe to the non-experimental version of the mod if you don't intend on testing {Details.BaseModName}.");
            }
        }

        private void CheckForIncompatibilities()
        {
            var incompatibleMods = Compatibility.IncompatibleMods;
            if (incompatibleMods.Count > 0)
            {
                var incompatibleStrings = new List<string>();

                foreach (var incompatibleMod in incompatibleMods)
                {
                    if (incompatibleMod != null)
                    {
                        var pluginName = incompatibleMod.PluginInfo.name;
                        var instances = incompatibleMod.PluginInfo.GetInstances<IUserMod>();

                        if (instances.Length > 0)
                        {
                            pluginName = instances[0].Name;
                        }

                        incompatibleStrings.Add($"{pluginName}:\n{incompatibleMod.Description}");
                    }
                }

                if (incompatibleStrings.Count > 0)
                {
                    MessageBoxWrapper.Show(MessageBoxWrapper.MessageType.Warning, $"{Details.ModName} incompatibilities", $"{Details.BaseModName} has detected some mod incompatibilities. These mods are known to not work with this mod: \n\n{string.Join("\n\n", incompatibleStrings.ToArray())}");
                }
            }
        }
    }
}
