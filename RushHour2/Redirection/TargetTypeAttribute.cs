using System;

namespace RushHour2.Core.Redirection
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    internal class TargetTypeAttribute : Attribute
    {
        public TargetTypeAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}
