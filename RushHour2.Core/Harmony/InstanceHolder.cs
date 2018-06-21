using Harmony;

namespace RushHour2.Core.Harmony
{
    public static class InstanceHolder
    {
        private static HarmonyInstance _instance = null;

        public static HarmonyInstance Instance => GetInstance();

        public static void Ensure()
        {
            if (_instance == null)
            {
                _instance = HarmonyInstance.Create("com.PropaneDragon.Mod.RushHour2");
            }
        }

        private static HarmonyInstance GetInstance()
        {
            Ensure();

            return _instance;
        }
    }
}
