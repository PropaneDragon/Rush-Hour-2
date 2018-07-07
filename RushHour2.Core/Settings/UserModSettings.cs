using RushHour2.Core.Info;
using RushHour2.Core.Reporting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace RushHour2.Core.Settings
{
    public static class UserModSettings
    {
        private static UserModSettingsHolder settingsHolder = new UserModSettingsHolder();

        private static XmlSerializer Serializer => new XmlSerializer(typeof(UserModSettingsHolder));

        public static string SaveFileName => $"{Details.BaseModName} Settings.xml";
        public static string SaveFilePath => SaveFileName;

        public static UserModSettingsHolder Settings => settingsHolder;

        public static bool TimeIsBetween(DateTime currentTime, TimeSpan startTime, TimeSpan duration)
        {
            return !TimeIsBefore(currentTime, startTime) && !TimeIsAfter(currentTime, startTime, duration);
        }

        public static bool TimeIsBefore(DateTime currentTime, TimeSpan startTime)
        {
            var begins = currentTime.Date.Add(startTime);

            return currentTime < begins;
        }

        public static bool TimeIsAfter(DateTime currentTime, TimeSpan startTime, TimeSpan duration)
        {
            var begins = currentTime.Date.Add(startTime);
            var ends = begins.Add(duration);

            return currentTime > ends;
        }

        public static bool Save()
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Attempting to save settings to {SaveFilePath}");

            try
            {
                using (var saveFile = File.CreateText(SaveFilePath))
                {
                    var serialiser = Serializer;
                    serialiser.Serialize(saveFile, settingsHolder);

                    LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Saved settings to {SaveFilePath}");

                    return true;
                }
            }
            catch (Exception ex)
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Error, $"Unable to save settings to {SaveFilePath}");
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, ex);
            }

            return false;
        }

        public static bool Load()
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Attempting to load settings from {SaveFilePath}");

            try
            {
                if (File.Exists(SaveFilePath))
                {
                    using (var saveFile = File.OpenRead(SaveFilePath))
                    {
                        var serialiser = Serializer;
                        settingsHolder = serialiser.Deserialize(saveFile) as UserModSettingsHolder;

                        LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Loaded settings from {SaveFilePath}");

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Error, $"Unable to load settings from {SaveFilePath}");
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, ex);
            }

            return false;
        }

        private static List<FieldInfo> GetSaveableFields()
        {
            var fields = new List<FieldInfo>();
            var potentialFields = typeof(UserModSettings).GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var potentialField in potentialFields)
            {
                var saveFileAttributes = potentialField.GetCustomAttributes(typeof(SaveFileAttribute), true);
                if (saveFileAttributes.Length > 0)
                {

                }
            }

            return fields;
        }
    }
}
