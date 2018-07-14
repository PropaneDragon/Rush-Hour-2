using RushHour2.Core.Settings;
using RushHour2.Patches.HarmonyLocal;
using System;
using System.Reflection;

namespace RushHour2.Patches.UI
{
    public class NewUIDateTimeWrapper_Check : Patchable
    {
        public override MethodBase BaseMethod => typeof(UIDateTimeWrapper).GetMethod("Check", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(DateTime) }, new ParameterModifier[] { });
        public override MethodInfo Prefix => typeof(NewUIDateTimeWrapper_Check).GetMethod(nameof(CheckPrefix), BindingFlags.Static | BindingFlags.Public);

        public static bool CheckPrefix(ref UIDateTimeWrapper __instance, DateTime newVal, ref DateTime ___m_Value, ref string ___m_String)
        {
            if (UserModSettings.Settings.Enabled && UserModSettings.Settings.DateTimeBar_Modify)
            {
                ___m_Value = newVal;
                ___m_String = ___m_Value.ToString("dddd HH:mm");

                return false;
            }

            return true;
        }
    }
}
