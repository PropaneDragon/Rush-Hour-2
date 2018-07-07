using RushHour2.Citizens.Location;
using RushHour2.Core.Settings;
using RushHour2.Patches.HarmonyLocal;
using System;
using System.Reflection;

namespace RushHour2.Patches.AI
{
    public class TouristAI_UpdateLocation : Patchable
    {
        public override MethodBase BaseMethod => typeof(TouristAI).GetMethod("UpdateLocation", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(uint), typeof(Citizen).MakeByRefType() }, new ParameterModifier[] { });
        public override MethodInfo Prefix => typeof(TouristAI_UpdateLocation).GetMethod(nameof(UpdateLocationPrefix), BindingFlags.Static | BindingFlags.Public);

        public static bool UpdateLocationPrefix(TouristAI __instance, uint citizenID, ref Citizen data)
        {
            if (UserModSettings.Settings.Enabled && UserModSettings.Settings.Tourists_Override)
            {
                return !TouristLocationHandler.Process(ref __instance, citizenID, ref data);
            }

            return true;
        }
    }
}
