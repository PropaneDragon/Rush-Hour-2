using System;
using System.Reflection;

namespace RushHour2.Core.Info
{
    public static class Details
    {
        public static string ModName => "Rush Hour II";
        public static string ModDescription => "Implements Rush Hour traffic and improves Citizen simulations.";
        public static Version Version => CurrentAssembly?.GetName()?.Version ?? new Version();

        private static Assembly CurrentAssembly => Assembly.GetAssembly(typeof(Details));
    }
}
