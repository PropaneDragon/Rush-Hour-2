namespace RushHour2.Localisation.Language
{
    public class Localisation
    {
        /// <summary>
        /// The ISO 639-1 code associated with the language.
        /// See https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
        /// </summary>
        public string IsoLanguageCode = "en-gb";

        public string ReadableName = "English (GB)";

        public bool CanTranslateIfUnavailable = false;

        public string Settings_SaveToDisk = "Save to disk";
        public string Settings_SaveToDisk_Tooltip = "Settings are applied automatically ingame. Clicking this button saves these settings to disk and will be loaded again on a restart.";

        public string Settings_Group_Core = "Core";
        public string Settings_Core_Language = "Language";
        public string Settings_Core_Enabled = "Enabled";
        public string Settings_Core_ModifyCitizenBehaviour = "Modify citizen behaviours";
        public string Settings_Core_ModifyTouristBehaviour = "Modify tourist behaviours";

        public string Settings_Group_Time = "Time";
        public string Settings_SimulationSpeed = "Simulation speed modifier";
        public string Settings_Time_ModifyDateTimeBar = "Modify the date/time bar to show the time";
        public string Settings_Time_24hrTime = "24 hour clock";

        public string Settings_Group_School = "School";
        public string Settings_School_StartTime = "Start time";
        public string Settings_School_Duration = "Duration";
        public string Settings_School_DurationFormat = "HH'h' mm'm'";

        public string Settings_Group_University = "University";
        public string Settings_University_StartTime = "Start time";
        public string Settings_University_Duration = "Duration";
        public string Settings_University_DurationFormat = "HH'h' mm'm'";

        public string Settings_Group_Commercial = "Commercial";
        public string Settings_Commercial_Weekday_StartTime = "Weekday start time";
        public string Settings_Commercial_Weekday_Duration = "Weekday duration";
        public string Settings_Commercial_Weekend_StartTime = "Weekend start time";
        public string Settings_Commercial_Weekend_Duration = "Weekend duration";
        public string Settings_Commercial_DurationFormat = "HH'h' mm'm'";

        public string Settings_Group_Industrial = "Industrial";
        public string Settings_Industrial_Weekday_StartTime = "Weekday start time";
        public string Settings_Industrial_Weekday_Duration = "Weekday duration";
        public string Settings_Industrial_Weekend_StartTime = "Weekend start time";
        public string Settings_Industrial_Weekend_Duration = "Weekend duration";
        public string Settings_Industrial_DurationFormat = "HH'h' mm'm'";

        public string Settings_Group_Offices = "Offices";
        public string Settings_Offices_Weekday_StartTime = "Weekday start time";
        public string Settings_Offices_Weekday_Duration = "Weekday duration";
        public string Settings_Offices_DurationFormat = "HH'h' mm'm'";

        public string Settings_Group_Work = "Work";
        public string Settings_Work_AllowLeisure = "Allow some workers to go to leisure areas after work on Fridays";

        public string Settings_Group_Citizens = "Citizens";
        public string Settings_Citizens_ReactToWeather = "React to weather";

        public string Settings_Group_Lighting = "Lighting";
        public string Settings_Lighting_OverrideLights = "Allow control of building lighting";
        public string Settings_Lighting_LightsEducation = "Allow control of educational building lighting";
        public string Settings_Lighting_LightsCommercial = "Allow control of commercial building lighting";
        public string Settings_Lighting_LightsOffices = "Allow control of office building lighting";
        public string Settings_Lighting_LightsResidential = "Allow control of residential building lighting";
        public string Settings_Lighting_LightsIndustrial = "Allow control of industrial building lighting";

        public string Settings_Group_DangerZone = "Danger zone";
        public string Settings_DangerZone_IgnoreVehiclePercentages = "Ignore vehicle percentages when spawning citizens";
        public string Settings_DangerZone_IgnoreVehiclePercentages_Tooltip = "Enabling this will allow more citizens to spawn, however it may cause vehicles to randomly disappear if you are near the maximum allowed vehicles in your city.";
        public string Settings_DangerZone_EnableMessageBoxes = "Enable message boxes";
        public string Settings_DangerZone_EnableMessageBoxes_Tooltip = "Disabling this will prevent all {0} message boxes from appearing. This could lead to you missing out on important information.";

        public string Settings_Group_Logging = "Logging";
        public string Settings_Logging_LogCitizenStatus = "Log citizen activity status to file";
        public string Settings_Logging_LogToFile = "Log to file";
        public string Settings_Logging_LogToDebugPanel = "Log to debug panel";
        public string Settings_Logging_LogToConsole = "Log to console";
        public string Settings_Logging_LogToFileInterval = "Log to file interval";
        public string Settings_Logging_LogToFileIntervalFormat = "m'm' ss's'";

        public string ExperimentalBuild_Title = "{0} - experimental build";
        public string ExperimentalBuild_Description = "You currently have an experimental build of {0} running.\n\n" +
                    "This build may have bugs and issues, as well as incomplete features as it is intended for testing only. " +
                    "Please subscribe to the non-experimental version of the mod if you don't intend on testing {0}.";

        public string Incompatibility_Title = "{0} - Incompatibility detected";
        public string Incompatibility_Description = "There appears to be an issue overriding part of the game, likely due to another conflicting mod.\n\nSome features have been disabled, but everything else will still work.";
    }
}
