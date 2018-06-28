using Harmony;
using RushHour2.Core.Reporting;
using RushHour2.Patches.AI;
using RushHour2.Patches.Simulation;
using RushHour2.Patches.UI;
using System;
using System.Linq;

namespace RushHour2.Patches.HarmonyLocal
{
    public static class Patcher
    {
        public static bool Patch(IPatchable patchable)
        {
            var patchableName = patchable?.GetType()?.Name;

            try
            {
                var original = patchable.BaseMethod;
                var prefix = patchable.Prefix;
                var postfix = patchable.Postfix;

                try
                {
                    if (original != null && (prefix != null || postfix != null))
                    {
                        var originalInstanceString = $"{original.Name}({string.Join(", ", original.GetParameters().Select(parameter => parameter.ParameterType.Name).ToArray())})";
                        var prefixInstanceString = prefix != null ? $"{prefix.Name}({string.Join(", ", prefix.GetParameters().Select(parameter => parameter.ParameterType.Name).ToArray())})" : "";
                        var postfixInstanceString = postfix != null ? $"{postfix.Name}({string.Join(", ", postfix.GetParameters().Select(parameter => parameter.ParameterType.Name).ToArray())})" : "";

                        LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Attempting to patch {originalInstanceString} to prefix: {prefixInstanceString} or postfix: {postfixInstanceString}");

                        HarmonyInstanceHolder.Instance.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

                        LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Patched {originalInstanceString}");

                        return true;
                    }
                    else
                    {
                        LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Error, $"Couldn't patch {patchableName} onto {original?.Name ?? "null"}!");
                    }
                }
                catch (Exception ex)
                {
                    LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Error, $"Couldn't patch {patchableName} onto {original?.Name ?? "null"}!");
                    LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, ex);
                }
            }
            catch (Exception ex)
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Error, $"Patchable {patchableName ?? "unknown"} is invalid!");
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, ex);
            }

            return false;
        }

        public static bool PatchAll()
        {
            var patched = true;

            patched = patched && Patch(new ResidentAI_UpdateLocation());
            patched = patched && Patch(new SimulationManager_Update());
            patched = patched && Patch(new NewUIDateTimeWrapper_Check());

            return patched;
        }
    }
}
