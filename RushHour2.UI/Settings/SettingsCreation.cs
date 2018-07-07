using ColossalFramework.UI;
using ICities;
using RushHour2.Core.Info;
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
                core.AddCheckbox("Modify citizen behaviours", UserModSettings.Settings.Citizens_Override, new OnCheckChanged(value => UserModSettings.Settings.Citizens_Override = value));
                core.AddCheckbox("Modify tourist behaviours", UserModSettings.Settings.Tourists_Override, new OnCheckChanged(value => UserModSettings.Settings.Tourists_Override = value));
            }

            var schoolHours = helper.AddGroup("School");
            {
                schoolHours.AddTimeSpanHoursSlider("Start time", TimeSpan.FromHours(6), TimeSpan.FromHours(11), UserModSettings.Settings.StartTime_Schools, new OnValueChanged(value => UserModSettings.Settings.StartTime_Schools = TimeSpan.FromHours(value)));
                schoolHours.AddTimeSpanHoursSlider("Duration", TimeSpan.FromHours(5), TimeSpan.FromHours(15), UserModSettings.Settings.Duration_Schools, new OnValueChanged(value => UserModSettings.Settings.Duration_Schools = TimeSpan.FromHours(value)), "HH'h' mm'm'");
            }

            var universityHours = helper.AddGroup("University");
            {
                universityHours.AddTimeSpanHoursSlider("Start time", TimeSpan.FromHours(6), TimeSpan.FromHours(11), UserModSettings.Settings.StartTime_University, new OnValueChanged(value => UserModSettings.Settings.StartTime_University = TimeSpan.FromHours(value)));
                universityHours.AddTimeSpanHoursSlider("Duration", TimeSpan.FromHours(5), TimeSpan.FromHours(15), UserModSettings.Settings.Duration_University, new OnValueChanged(value => UserModSettings.Settings.Duration_University = TimeSpan.FromHours(value)), "HH'h' mm'm'");
            }

            var workHours = helper.AddGroup("Work");
            {
                workHours.AddTimeSpanHoursSlider("Start time", TimeSpan.FromHours(6), TimeSpan.FromHours(11), UserModSettings.Settings.StartTime_Work, new OnValueChanged(value => UserModSettings.Settings.StartTime_Work = TimeSpan.FromHours(value)));
                workHours.AddTimeSpanHoursSlider("Duration", TimeSpan.FromHours(5), TimeSpan.FromHours(15), UserModSettings.Settings.Duration_Work, new OnValueChanged(value => UserModSettings.Settings.Duration_Work = TimeSpan.FromHours(value)), "HH'h' mm'm'");
                workHours.AddCheckbox("Allow some workers to go to leisure areas after work on Fridays", UserModSettings.Settings.Citizens_AllowLeisureAfterWork, new OnCheckChanged(value => { }));
            }

            var dangerZone = helper.AddGroup("Danger zone");
            {
                if (dangerZone.AddCheckbox("Ignore vehicle percentages when spawning citizens", UserModSettings.Settings.Citizens_IgnoreVehicleCount, new OnCheckChanged(value => UserModSettings.Settings.Citizens_IgnoreVehicleCount = value)) is UICheckBox ignoreVehiclePercentagesCheckbox)
                {
                    ignoreVehiclePercentagesCheckbox.tooltip = "Enabling this will allow more citizens to spawn, however it may cause vehicles to randomly disappear if you are near the maximum allowed vehicles in your city.";
                }

                if (dangerZone.AddCheckbox("Enable message boxes", UserModSettings.Settings.MessageBoxes_Enabled, new OnCheckChanged(value => UserModSettings.Settings.MessageBoxes_Enabled = value)) is UICheckBox enableMessageBoxesCheckbox)
                {
                    enableMessageBoxesCheckbox.tooltip = $"Disabling this will prevent all {Details.BaseModName} message boxes from appearing. This could lead to you missing out on important information.";
                }
            }

            var logging = helper.AddGroup("Logging");
            {
                logging.AddCheckbox("Log to file", UserModSettings.Settings.Logging_ToFile, new OnCheckChanged(value => UserModSettings.Settings.Logging_ToFile = value));
                logging.AddCheckbox("Log to debug panel", UserModSettings.Settings.Logging_ToDebug, new OnCheckChanged(value => UserModSettings.Settings.Logging_ToDebug = value));
                logging.AddCheckbox("Log to console", UserModSettings.Settings.Logging_ToConsole, new OnCheckChanged(value => UserModSettings.Settings.Logging_ToConsole = value));
                logging.AddTimeSpanSecondsSlider("Log to file interval", TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(2), UserModSettings.Settings.Logging_ToFile_Duration, new OnValueChanged(value => UserModSettings.Settings.Logging_ToFile_Duration = TimeSpan.FromSeconds(value)), "m'm' ss's'");
            }
        }
    }
}
