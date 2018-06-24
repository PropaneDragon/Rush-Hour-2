using RushHour2.Core.Info;
using RushHour2.Core.Reporting;
using RushHour2.Patches.HarmonyLocal;
using System;
using System.Reflection;

namespace RushHour2.Patches
{
    public static class PatchManager
    {
        public static Assembly PatchAssembly => Assembly.GetAssembly(typeof(PatchManager));

        public static bool PatchAll()
        {
            try
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"Patching {PatchAssembly.GetName().Name}...");

                Patcher.PatchAll();

                LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, "Done!");

                return true;
            }
            catch (Exception ex)
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Error, $"Couldn't patch the required methods!");
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, ex, true);

                MessageBoxWrapper.Show(MessageBoxWrapper.MessageType.Warning,
                    $"{Details.ModName} couldn't start", $"{Details.ModName} couldn't fully start due to an issue overriding parts of the game.\n" +
                    $"This is possibly due to a recent update to Cities, and in order to prevent potential issues {Details.ModName} has disabled some functionality.");

                UnPatchAll();
            }

            return false;
        }

        public static bool UnPatchAll()
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"Removing patching for {PatchAssembly.GetName().Name}...");

            var patchedMethods = HarmonyInstanceHolder.Instance.GetPatchedMethods();
            foreach (var patchedMethod in patchedMethods)
            {
                HarmonyInstanceHolder.Instance.RemovePatch(patchedMethod, Harmony.HarmonyPatchType.All, HarmonyInstanceHolder.Instance.Id);
            }

            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, "Done!");

            return true;
        }
    }
}
