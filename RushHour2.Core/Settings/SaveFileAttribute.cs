using System;

namespace RushHour2.Core.Settings
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SaveFileAttribute : Attribute
    {
        public SaveFileAttribute()
        {
        }
    }
}
