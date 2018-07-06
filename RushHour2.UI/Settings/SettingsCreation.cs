using ColossalFramework.UI;
using ICities;
using RushHour2.Core.Settings;
using System;

namespace RushHour2.UI.Settings
{
    public static class SettingsCreation
    {
        public static void Create(UIHelperBase helper)
        {
            var schoolHours = helper.AddGroup("School");
            {
                schoolHours.AddSlider("Start time", 6, 11, 0.1f, (float)UserModSettings.StartTime_Schools.TotalHours, new OnValueChanged(value => UserModSettings.StartTime_Schools = TimeSpan.FromHours(value)));
                schoolHours.AddSlider("Duration", 5, 15, 0.1f, (float)UserModSettings.Duration_Schools.TotalHours, new OnValueChanged(value => UserModSettings.Duration_Schools = TimeSpan.FromHours(value)));
            }

            var universityHours = helper.AddGroup("University");
            {
                universityHours.AddSlider("Start time", 6, 11, 0.1f, (float)UserModSettings.StartTime_University.TotalHours, new OnValueChanged(value => UserModSettings.StartTime_University = TimeSpan.FromHours(value)));
                universityHours.AddSlider("Duration", 5, 15, 0.1f, (float)UserModSettings.Duration_University.TotalHours, new OnValueChanged(value => UserModSettings.Duration_University = TimeSpan.FromHours(value)));
            }

            var workHours = helper.AddGroup("Work");
            {
                workHours.AddSlider("Start time", 6, 11, 0.1f, (float)UserModSettings.StartTime_Work.TotalHours, new OnValueChanged(value => UserModSettings.StartTime_Work = TimeSpan.FromHours(value)));
                workHours.AddSlider("Duration", 5, 15, 0.1f, (float)UserModSettings.Duration_Work.TotalHours, new OnValueChanged(value => UserModSettings.Duration_Work = TimeSpan.FromHours(value)));
                workHours.AddCheckbox("Chance of going out after work on Fridays to a bar", true, new OnCheckChanged(value => { }));
            }

            var dangerZone = helper.AddGroup("Danger zone");
            {
                if (dangerZone.AddCheckbox("Ignore vehicle percentages when spawning citizens", false, new OnCheckChanged(isChecked => UserModSettings.Citizens_IgnoreVehicleCount = isChecked)) is UICheckBox ignoreVehiclePercentagesCheckbox)
                {
                    ignoreVehiclePercentagesCheckbox.tooltip = "Enabling this will allow more citizens to spawn, however it may cause vehicles to randomly disappear if you are near the maximum allowed vehicles in your city.";
                }
            }
        }
    }
}
