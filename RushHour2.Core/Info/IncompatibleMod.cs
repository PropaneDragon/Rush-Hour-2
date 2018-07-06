using static ColossalFramework.Plugins.PluginManager;

namespace RushHour2.Core.Info
{
    public class IncompatibleMod
    {
        private string _description = null;
        private PluginInfo _pluginInfo = null;

        public string Description => _description;
        public PluginInfo PluginInfo => _pluginInfo;

        public IncompatibleMod(PluginInfo pluginInfo, string description)
        {
            _pluginInfo = pluginInfo;
            _description = description;
        }
    }
}
