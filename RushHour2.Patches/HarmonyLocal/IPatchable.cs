using System.Reflection;

namespace RushHour2.Patches.HarmonyLocal
{
    public interface IPatchable
    {
        MethodBase BaseMethod { get; }
        MethodInfo Prefix { get; }
        MethodInfo Postfix { get; }
    }
}
