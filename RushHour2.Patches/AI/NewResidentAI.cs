using RushHour2.Citizens.Location;
using RushHour2.Patches.HarmonyLocal;
using System;
using System.Reflection;

namespace RushHour2.Patches.AI
{
    public class ResidentAI_UpdateLocation : Patchable
    {
        public override MethodBase BaseMethod => typeof(ResidentAI).GetMethod("UpdateLocation", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(uint), typeof(Citizen).MakeByRefType() }, new ParameterModifier[] { });
        public override MethodInfo Prefix => typeof(ResidentAI_UpdateLocation).GetMethod(nameof(UpdateLocationPrefix), BindingFlags.Static | BindingFlags.Public);

        public static bool UpdateLocationPrefix(ResidentAI __instance, uint citizenID, ref Citizen data)
        {
            return !LocationHandler.Process(ref __instance, citizenID, ref data);
        }
    }
}
