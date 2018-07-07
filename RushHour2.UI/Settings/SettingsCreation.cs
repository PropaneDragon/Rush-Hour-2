using ColossalFramework.UI;
using ICities;
using RushHour2.Core.Settings;
using RushHour2.UI.Extensions;
using System;

namespace RushHour2.UI.Settings
{
    public static class SettingsCreation
    {
        public static void Create(UIHelperBase helper)
        {
            UserModSettings.Load();

            if (helper.AddButton("Save to disk", new OnButtonClicked(() => UserModSettings.Save())) is UIButton button)
            {
                button.tooltip = "Settings are applied automatically ingame. Clicking this button saves these settings to disk and will be loaded again on a restart.";
            }

            helper.AddSpace(10);

            var core = helper.AddGroup("Core");
            {
                core.AddCheckbox("Enabled", UserModSettings.Settings.Enabled, new OnCheckChanged(value => UserModSettings.Settings.Enabled = value));
                core.AddCheckbox("Modify the date/time bar to show the time", UserModSettings.Settings.DateTimeBar_Modify, new OnCheckChanged(value => UserModSettings.Settings.DateTimeBar_Modify = value));
            }

            var schoolHours = helper.AddGroup("School");
            {
                schoolHours.AddTimeSpanSlider("Start time", TimeSpan.FromHours(6), TimeSpan.FromHours(11), UserModSettings.Settings.StartTime_Schools, new OnValueChanged(value => UserModSettings.Settings.StartTime_Schools = TimeSpan.FromHours(value)));
                schoolHours.AddTimeSpanSlider("Duration", TimeSpan.FromHours(5), TimeSpan.FromHours(15), UserModSettings.Settings.Duration_Schools, new OnValueChanged(value => UserModSettings.Settings.Duration_Schools = TimeSpan.FromHours(value)));
            }

            var universityHours = helper.AddGroup("University");
            {
                universityHours.AddTimeSpanSlider("Start time", TimeSpan.FromHours(6), TimeSpan.FromHours(11), UserModSettings.Settings.StartTime_University, new OnValueChanged(value => UserModSettings.Settings.StartTime_University = TimeSpan.FromHours(value)));
                universityHours.AddTimeSpanSlider("Duration", TimeSpan.FromHours(5), TimeSpan.FromHours(15), UserModSettings.Settings.Duration_University, new OnValueChanged(value => UserModSettings.Settings.Duration_University = TimeSpan.FromHours(value)));
            }

            var workHours = helper.AddGroup("Work");
            {
                workHours.AddTimeSpanSlider("Start time", TimeSpan.FromHours(6), TimeSpan.FromHours(11), UserModSettings.Settings.StartTime_Work, new OnValueChanged(value => UserModSettings.Settings.StartTime_Work = TimeSpan.FromHours(value)));
                workHours.AddTimeSpanSlider("Duration", TimeSpan.FromHours(5), TimeSpan.FromHours(15), UserModSettings.Settings.Duration_Work, new OnValueChanged(value => UserModSettings.Settings.Duration_Work = TimeSpan.FromHours(value)));
                workHours.AddCheckbox("Chance of going out after work on Fridays to a bar", true, new OnCheckChanged(value => { }));
            }

            var dangerZone = helper.AddGroup("Danger zone");
            {
                if (dangerZone.AddCheckbox("Ignore vehicle percentages when spawning citizens", UserModSettings.Settings.Citizens_IgnoreVehicleCount, new OnCheckChanged(isChecked => UserModSettings.Settings.Citizens_IgnoreVehicleCount = isChecked)) is UICheckBox ignoreVehiclePercentagesCheckbox)
                {
                    ignoreVehiclePercentagesCheckbox.tooltip = "Enabling this will allow more citizens to spawn, however it may cause vehicles to randomly disappear if you are near the maximum allowed vehicles in your city.";
                }
            }

            var logging = helper.AddGroup("Logging");
            {
                logging.AddCheckbox("Log to file", UserModSettings.Settings.Logging_ToFile, new OnCheckChanged(isChecked => UserModSettings.Settings.Logging_ToFile = isChecked));
                logging.AddCheckbox("Log to debug panel", UserModSettings.Settings.Logging_ToDebug, new OnCheckChanged(isChecked => UserModSettings.Settings.Logging_ToDebug = isChecked));
                logging.AddCheckbox("Log to console", UserModSettings.Settings.Logging_ToConsole, new OnCheckChanged(isChecked => UserModSettings.Settings.Logging_ToConsole = isChecked));
            }
        }
    }
}
