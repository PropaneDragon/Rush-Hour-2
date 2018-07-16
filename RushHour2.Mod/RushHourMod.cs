using System.Collections.Generic;
using ColossalFramework.PlatformServices;
using ICities;
using RushHour2.Core.Info;
using RushHour2.Core.Reporting;
using RushHour2.Core.Settings;
using RushHour2.Localisation.Language;
using RushHour2.Mod.Extensions;
using RushHour2.Patches;
using RushHour2.UI.MainMenuScreen;
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
            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"{Name} has been enabled.");

            LoadingManager.instance.m_introLoaded += OnIntroLoaded;

            LocalisationHolder.Load();
            UserModSettings.Load();
            LocalisationHolder.ChangeLocalisationFromName(UserModSettings.Settings.Language);

            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"{Name} has finished enabling.");
        }

        public void OnDisabled()
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"{Name} has been disabled.");

            PatchManager.UnPatchAll();
            
            LoadingManager.instance.m_introLoaded -= OnIntroLoaded;

            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"{Name} has finished disabling.");
        }

        private void OnIntroLoaded()
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"{Name} is loading post intro.");

            CheckForExperimentalVersion();
            CheckForIncompatibilities();

            PatchManager.PatchAll();
            SimulationManager.RegisterSimulationManager(new SimulationExtension());
            UpdateText.DisplayIfRequired();

            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"{Name} has loaded post intro.");
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"{Name} is creating settings.");

            SettingsCreation.Create(helper);

            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"{Name} has created settings.");
        }

        private static void CheckForExperimentalVersion()
        {
            if (Details.ExperimentalBuild && UserModSettings.RecentlyUpdated)
            {
                var title = string.Format(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.ExperimentalBuild_Title), Details.BaseModName);
                var description = string.Format(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.ExperimentalBuild_Description), Details.BaseModName);

                MessageBoxWrapper.Show(MessageBoxWrapper.MessageType.Warning, title, description);
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
