using System.Reflection;

namespace RushHour2.Patches.HarmonyLocal
{
    public abstract class Patchable : IPatchable
    {
        public abstract MethodBase BaseMethod { get; }

        public virtual MethodInfo Prefix => null;

        public virtual MethodInfo Postfix => null;
    }
}
