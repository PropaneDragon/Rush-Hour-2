using ColossalFramework.PlatformServices;
using ColossalFramework.Plugins;
using System;
using System.Reflection;
using static ColossalFramework.Plugins.PluginManager;

namespace RushHour2.Core.Info
{
    public static class Details
    {
        private static PublishedFileId ID_EXPERIMENTAL = new PublishedFileId(1432430887);

        public static string BaseModName => "Rush Hour II";
        public static string ModName => BaseModName + (ExperimentalBuild ? " - Development" : "");
        public static string ModDescription => "Implements Rush Hour traffic and improves Citizen simulations.";
        public static bool ExperimentalBuild => IsExperimental();
        public static Version Version => CurrentAssembly?.GetName()?.Version ?? new Version();
        public static PluginInfo Info => PluginManager.instance.FindPluginInfo(CurrentAssembly);

        private static Assembly CurrentAssembly => Assembly.GetAssembly(typeof(Details));

        private static bool IsExperimental()
        {
            var info = Info;
            if (info != null)
            {
                return info.publishedFileID == ID_EXPERIMENTAL;
            }

            return false;
        }
    }
}
