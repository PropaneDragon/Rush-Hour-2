using ColossalFramework.UI;
using RushHour2.Core.Settings;
using RushHour2.Patches.HarmonyLocal;
using System;
using System.Reflection;

namespace RushHour2.Patches.UI
{
    public class NewInfoPanel_Update : Patchable
    {
        public override MethodBase BaseMethod => typeof(InfoPanel).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, new ParameterModifier[] { });
        public override MethodInfo Postfix => typeof(NewInfoPanel_Update).GetMethod(nameof(UpdatePostfix), BindingFlags.Static | BindingFlags.Public);

        public static void UpdatePostfix(ref UILabel ___m_TimeLabel, ref UISprite ___m_TimeSprite)
        {
            if (UserModSettings.Settings.Enabled)
            {
                ___m_TimeLabel.width = ___m_TimeSprite.width;
            }
        }
    }
}
