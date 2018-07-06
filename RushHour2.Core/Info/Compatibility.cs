using ColossalFramework.PlatformServices;
using ColossalFramework.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RushHour2.Core.Info
{
    public static class Compatibility
    {
        public static List<IncompatibleMod> IncompatibleMods => GetIncompatibleMods();

        private static Dictionary<PublishedFileId, string> AllIncompatibilities => GetAllIncompatibilities();

        private static Dictionary<PublishedFileId, string> _staticIncompatibilities => new Dictionary<PublishedFileId, string>()
        {
            { new PublishedFileId(1420955187), "You can't run both of these mods together. They both change similar elements of the game, and may have issues working together." }
        };

        private static List<IncompatibleMod> GetIncompatibleMods()
        {
            var incompatibleMods = new List<IncompatibleMod>();            

            foreach (PublishedFileId publishedFileId in PlatformService.workshop.GetSubscribedItems())
            {
                if (AllIncompatibilities.ContainsKey(publishedFileId))
                {
                    incompatibleMods.Add(GetModDetails(publishedFileId));
                }
            }

            return incompatibleMods;
        }

        private static IncompatibleMod GetModDetails(PublishedFileId publishedFileId)
        {
            var pluginsInfo = PluginManager.instance.GetPluginsInfo();
            var foundPlugins = pluginsInfo.Where(pluginInfo => pluginInfo.publishedFileID == publishedFileId && pluginInfo.isEnabled);
            var allIncompatibilities = AllIncompatibilities;

            if (foundPlugins.Count() > 0 && allIncompatibilities.ContainsKey(publishedFileId))
            {
                return new IncompatibleMod(foundPlugins.First(), allIncompatibilities[publishedFileId]);
            }

            return null;
        }

        private static Dictionary<PublishedFileId, string> GetAllIncompatibilities()
        {
            var fullIncompatibleModList = new Dictionary<PublishedFileId, string>(_staticIncompatibilities);
            var experimentalCompatibility = GetExperimentalCompatibility();

            fullIncompatibleModList.Add(experimentalCompatibility.Key, experimentalCompatibility.Value);

            return fullIncompatibleModList;
        }

        private static KeyValuePair<PublishedFileId, string> GetExperimentalCompatibility()
        {
            if (Details.ExperimentalBuild)
            {
                return new KeyValuePair<PublishedFileId, string>(new PublishedFileId(605590542), "You can't have both versions active at once. They both change the same elements of the game and won't work together. Please enable just one.");
            }
            else
            {
                return new KeyValuePair<PublishedFileId, string>(new PublishedFileId(1432430887), "You can't have both versions active at once. They both change the same elements of the game and won't work together. Please enable just one.");
            }
        }
    }
}
