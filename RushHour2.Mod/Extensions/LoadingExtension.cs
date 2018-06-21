using Harmony;
using ICities;
using RushHour2.Core.Harmony;
using RushHour2.Core.Settings;

namespace RushHour2.Mod.Extensions
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (ShouldRedirect(mode))
            {
                InstanceHolder.Ensure();
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
        }

        private bool ShouldRedirect(LoadMode mode)
        {
            return UserModSettings.Enabled && ValidMode(mode);
        }

        private bool ValidMode(LoadMode mode)
        {
            switch (mode)
            {
                case LoadMode.LoadGame:
                case LoadMode.NewGame:
                case LoadMode.NewGameFromScenario:
                    return true;
            }

            return false;
        }
    }
}
