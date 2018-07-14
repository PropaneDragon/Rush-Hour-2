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
            var patchSuccess = false;

            LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"Patching...");

            try
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"Patching {PatchAssembly.GetName().Name}...");

                patchSuccess = Patcher.PatchAll();

                LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, "Done!");
            }
            catch (Exception ex)
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Error, $"Couldn't patch the required methods!");
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, ex, true);
            }

            if (!patchSuccess)
            {
                MessageBoxWrapper.Show(MessageBoxWrapper.MessageType.Warning,
                    $"{Details.ModName} couldn't start", $"{Details.ModName} couldn't fully start due to an issue overriding parts of the game.\n\n" +
                    $"This is likely due to a recent update to Cities, and in order to prevent potential compatibility issues {Details.BaseModName} has disabled some functionality by reverting changes until a solution is found. This will result in most mod functionaity being disabled.\n\n" +
                    $"If this has not previously been reported please do so, otherwise an update to the mod is required to fix the incompatibilites.");

                UnPatchAll();
            }

            return patchSuccess;
        }

        public static bool UnPatchAll()
        {
            var unpatchSuccess = false;

            try
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, $"Removing patching for {PatchAssembly.GetName().Name}...");

                var patchedMethods = HarmonyInstanceHolder.Instance.GetPatchedMethods();
                foreach (var patchedMethod in patchedMethods)
                {
                    HarmonyInstanceHolder.Instance.Unpatch(patchedMethod, Harmony.HarmonyPatchType.All, HarmonyInstanceHolder.Instance.Id);
                }

                LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Message, "Done!");

                unpatchSuccess = true;
            }
            catch (Exception ex)
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.All, LoggingWrapper.LogType.Error, $"Couldn't unpatch everything!");
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, ex, true);
            }

            return unpatchSuccess;
        }
    }
}
