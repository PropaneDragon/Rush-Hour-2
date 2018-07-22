using ColossalFramework.UI;
using ICities;
using RushHour2.Core.Info;
using RushHour2.Core.Settings;
using RushHour2.Localisation.Language;
using RushHour2.UI.Extensions;
using System;
using System.Linq;

namespace RushHour2.UI.Settings
{
    public static class SettingsCreation
    {
        public static void Create(UIHelperBase helper)
        {
            if (helper.AddButton(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_SaveToDisk), new OnButtonClicked(() => UserModSettings.Save())) is UIButton saveToDiskButton)
            {
                saveToDiskButton.tooltip = LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_SaveToDisk_Tooltip);
            }

            helper.AddSpace(10);

            var core = helper.AddGroup(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Group_Core));
            {
                core.AddDropdown(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Core_Language), LocalisationHolder.Localisations.Select(localisation => localisation.ReadableName).ToArray(), LocalisationHolder.CurrentLocalisationIndex, new OnDropdownSelectionChanged(index => UserModSettings.Settings.Language = LocalisationHolder.Localisations[index].ReadableName));
                core.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Core_Enabled), UserModSettings.Settings.Enabled, new OnCheckChanged(value => UserModSettings.Settings.Enabled = value));
                core.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Core_ModifyCitizenBehaviour), UserModSettings.Settings.Citizens_Override, new OnCheckChanged(value => UserModSettings.Settings.Citizens_Override = value));
                core.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Core_ModifyTouristBehaviour), UserModSettings.Settings.Tourists_Override, new OnCheckChanged(value => UserModSettings.Settings.Tourists_Override = value));
            }

            var time = helper.AddGroup(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Group_Time));
            {
                time.AddSliderWithTooltip(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_SimulationSpeed), 0, 5, 1, 5f - (float)Math.Log((1f / UserModSettings.Settings.Simulation_Speed), 2), new OnValueChanged(value => UserModSettings.Settings.Simulation_Speed = 1f / (float)Math.Pow(2, (5 - value))), value =>
                {
                    var speed = Math.Pow(2, (5 - value));
                    return speed > 1 ? $"1/{speed}x" : $"{1 / speed}x";
                });
                time.AddSpace(5);
                time.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Time_ModifyDateTimeBar), UserModSettings.Settings.DateTimeBar_Modify, new OnCheckChanged(value => UserModSettings.Settings.DateTimeBar_Modify = value));
                time.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Time_24hrTime), UserModSettings.Settings.Time_24Hour, new OnCheckChanged(value => UserModSettings.Settings.Time_24Hour = value));
            }

            var schoolHours = helper.AddGroup(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Group_School));
            {
                schoolHours.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_School_StartTime), TimeSpan.FromHours(6), TimeSpan.FromHours(11), UserModSettings.Settings.StartTime_Schools, new OnValueChanged(value => UserModSettings.Settings.StartTime_Schools = TimeSpan.FromHours(value)));
                schoolHours.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_School_Duration), TimeSpan.FromHours(5), TimeSpan.FromHours(15), UserModSettings.Settings.Duration_Schools, new OnValueChanged(value => UserModSettings.Settings.Duration_Schools = TimeSpan.FromHours(value)), LocalisationHolder.CurrentLocalisation.Settings_School_DurationFormat);
            }

            var universityHours = helper.AddGroup(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Group_University));
            {
                universityHours.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_University_StartTime), TimeSpan.FromHours(6), TimeSpan.FromHours(11), UserModSettings.Settings.StartTime_University, new OnValueChanged(value => UserModSettings.Settings.StartTime_University = TimeSpan.FromHours(value)));
                universityHours.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_University_Duration), TimeSpan.FromHours(5), TimeSpan.FromHours(15), UserModSettings.Settings.Duration_University, new OnValueChanged(value => UserModSettings.Settings.Duration_University = TimeSpan.FromHours(value)), LocalisationHolder.CurrentLocalisation.Settings_University_DurationFormat);
            }

            var commercialHoursGroup = helper.AddGroup(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Group_Commercial));
            {
                commercialHoursGroup.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Commercial_Weekday_StartTime), TimeSpan.FromHours(6), TimeSpan.FromHours(11), UserModSettings.Settings.StartTime_Commercial_Weekday, new OnValueChanged(value => UserModSettings.Settings.StartTime_Commercial_Weekday = TimeSpan.FromHours(value)));
                commercialHoursGroup.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Commercial_Weekday_Duration), TimeSpan.FromHours(5), TimeSpan.FromHours(15), UserModSettings.Settings.Duration_Commercial_Weekday, new OnValueChanged(value => UserModSettings.Settings.Duration_Commercial_Weekday = TimeSpan.FromHours(value)), LocalisationHolder.CurrentLocalisation.Settings_Commercial_DurationFormat);
                commercialHoursGroup.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Commercial_Weekend_StartTime), TimeSpan.FromHours(6), TimeSpan.FromHours(11), UserModSettings.Settings.StartTime_Commercial_Weekend, new OnValueChanged(value => UserModSettings.Settings.StartTime_Commercial_Weekend = TimeSpan.FromHours(value)));
                commercialHoursGroup.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Commercial_Weekend_Duration), TimeSpan.FromHours(5), TimeSpan.FromHours(15), UserModSettings.Settings.Duration_Commercial_Weekend, new OnValueChanged(value => UserModSettings.Settings.Duration_Commercial_Weekend = TimeSpan.FromHours(value)), LocalisationHolder.CurrentLocalisation.Settings_Commercial_DurationFormat);
            }

            var industrialHoursGroup = helper.AddGroup(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Group_Industrial));
            {
                industrialHoursGroup.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Industrial_Weekday_StartTime), TimeSpan.FromHours(6), TimeSpan.FromHours(11), UserModSettings.Settings.StartTime_Industrial_Weekday, new OnValueChanged(value => UserModSettings.Settings.StartTime_Industrial_Weekday = TimeSpan.FromHours(value)));
                industrialHoursGroup.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Industrial_Weekday_Duration), TimeSpan.FromHours(5), TimeSpan.FromHours(15), UserModSettings.Settings.Duration_Industrial_Weekday, new OnValueChanged(value => UserModSettings.Settings.Duration_Industrial_Weekday = TimeSpan.FromHours(value)), LocalisationHolder.CurrentLocalisation.Settings_Industrial_DurationFormat);
                industrialHoursGroup.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Industrial_Weekend_StartTime), TimeSpan.FromHours(6), TimeSpan.FromHours(11), UserModSettings.Settings.StartTime_Industrial_Weekend, new OnValueChanged(value => UserModSettings.Settings.StartTime_Industrial_Weekend = TimeSpan.FromHours(value)));
                industrialHoursGroup.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Industrial_Weekend_Duration), TimeSpan.FromHours(5), TimeSpan.FromHours(15), UserModSettings.Settings.Duration_Industrial_Weekend, new OnValueChanged(value => UserModSettings.Settings.Duration_Industrial_Weekend = TimeSpan.FromHours(value)), LocalisationHolder.CurrentLocalisation.Settings_Industrial_DurationFormat);
            }

            var officesHoursGroup = helper.AddGroup(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Group_Offices));
            {
                officesHoursGroup.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Offices_Weekday_StartTime), TimeSpan.FromHours(6), TimeSpan.FromHours(11), UserModSettings.Settings.StartTime_Offices_Weekday, new OnValueChanged(value => UserModSettings.Settings.StartTime_Offices_Weekday = TimeSpan.FromHours(value)));
                officesHoursGroup.AddTimeSpanHoursSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Offices_Weekday_Duration), TimeSpan.FromHours(5), TimeSpan.FromHours(15), UserModSettings.Settings.Duration_Offices_Weekday, new OnValueChanged(value => UserModSettings.Settings.Duration_Offices_Weekday = TimeSpan.FromHours(value)), LocalisationHolder.CurrentLocalisation.Settings_Offices_DurationFormat);
            }

            var workGroup = helper.AddGroup(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Group_Work));
            {
                workGroup.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Work_AllowLeisure), UserModSettings.Settings.Citizens_AllowLeisureAfterWork, new OnCheckChanged(value => { UserModSettings.Settings.Citizens_AllowLeisureAfterWork = value; }));
            }

            var citizens = helper.AddGroup(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Group_Citizens));
            {
                citizens.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Citizens_ReactToWeather), UserModSettings.Settings.Citizens_ReactToWeather, new OnCheckChanged(value => { UserModSettings.Settings.Citizens_ReactToWeather = value; }));
            }

            var lightingGroup = helper.AddGroup(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Group_Lighting));
            {
                lightingGroup.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Lighting_OverrideLights), UserModSettings.Settings.Buildings_OverrideLights, new OnCheckChanged(value => UserModSettings.Settings.Buildings_OverrideLights = value));
                lightingGroup.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Lighting_LightsEducation), UserModSettings.Settings.Buildings_OverrideSchoolLights, new OnCheckChanged(value => UserModSettings.Settings.Buildings_OverrideSchoolLights = value));
                lightingGroup.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Lighting_LightsCommercial), UserModSettings.Settings.Buildings_OverrideCommercialLights, new OnCheckChanged(value => UserModSettings.Settings.Buildings_OverrideCommercialLights = value));
                lightingGroup.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Lighting_LightsOffices), UserModSettings.Settings.Buildings_OverrideOfficeLights, new OnCheckChanged(value => UserModSettings.Settings.Buildings_OverrideOfficeLights = value));
                lightingGroup.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Lighting_LightsIndustrial), UserModSettings.Settings.Buildings_OverrideIndustrialLights, new OnCheckChanged(value => UserModSettings.Settings.Buildings_OverrideIndustrialLights = value));
                lightingGroup.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Lighting_LightsResidential), UserModSettings.Settings.Buildings_OverrideResidentialLights, new OnCheckChanged(value => UserModSettings.Settings.Buildings_OverrideResidentialLights = value));
            }

            var dangerZone = helper.AddGroup(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Group_DangerZone));
            {
                if (dangerZone.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_DangerZone_IgnoreVehiclePercentages), UserModSettings.Settings.Citizens_IgnoreVehicleCount, new OnCheckChanged(value => UserModSettings.Settings.Citizens_IgnoreVehicleCount = value)) is UICheckBox ignoreVehiclePercentagesCheckbox)
                {
                    ignoreVehiclePercentagesCheckbox.tooltip = LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_DangerZone_IgnoreVehiclePercentages_Tooltip);
                }

                if (dangerZone.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_DangerZone_EnableMessageBoxes), UserModSettings.Settings.MessageBoxes_Enabled, new OnCheckChanged(value => UserModSettings.Settings.MessageBoxes_Enabled = value)) is UICheckBox enableMessageBoxesCheckbox)
                {
                    enableMessageBoxesCheckbox.tooltip = string.Format(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_DangerZone_EnableMessageBoxes_Tooltip), Details.BaseModName);
                }
            }

            var logging = helper.AddGroup(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Group_Logging));
            {
                logging.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Logging_LogCitizenStatus), UserModSettings.Settings.Log_Citizen_Status, new OnCheckChanged(value => UserModSettings.Settings.Log_Citizen_Status = value));
                logging.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Logging_LogToFile), UserModSettings.Settings.Logging_ToFile, new OnCheckChanged(value => UserModSettings.Settings.Logging_ToFile = value));
                logging.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Logging_LogToDebugPanel), UserModSettings.Settings.Logging_ToDebug, new OnCheckChanged(value => UserModSettings.Settings.Logging_ToDebug = value));
                logging.AddCheckbox(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Logging_LogToConsole), UserModSettings.Settings.Logging_ToConsole, new OnCheckChanged(value => UserModSettings.Settings.Logging_ToConsole = value));
                logging.AddTimeSpanSecondsSlider(LocalisationHolder.Translate(LocalisationHolder.CurrentLocalisation.Settings_Logging_LogToFileInterval), TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(2), UserModSettings.Settings.Logging_ToFile_Duration, new OnValueChanged(value => UserModSettings.Settings.Logging_ToFile_Duration = TimeSpan.FromSeconds(value)), LocalisationHolder.CurrentLocalisation.Settings_Logging_LogToFileIntervalFormat);
            }
        }
    }
}
