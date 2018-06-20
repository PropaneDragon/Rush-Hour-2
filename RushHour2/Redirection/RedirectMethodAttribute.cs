using System;

namespace RushHour2.Core.Redirection
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class RedirectMethodAttribute : Attribute
    {
        public RedirectMethodAttribute()
        {
            OnCreated = false;
        }

        public RedirectMethodAttribute(bool onCreated)
        {
            OnCreated = onCreated;
        }

        public bool OnCreated { get; }
    }
}
