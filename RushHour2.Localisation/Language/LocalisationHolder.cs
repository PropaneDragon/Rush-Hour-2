
using RushHour2.Core.Info;
using RushHour2.Core.Reporting;
using RushHour2.Core.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RushHour2.Localisation.Language
{
    public static class LocalisationHolder
    {
        private static Localisation _defaultLocalisation = new Localisation();
        private static Localisation _currentLocalisation = null;
        private static List<Localisation> _localisations = new List<Localisation>();
        private static XmlSerializer Serialiser => new XmlSerializer(typeof(Localisation));

        public static string LocalisationFilePath => Path.Combine(Details.ModPath, "Localisations");

        public static int CurrentLocalisationIndex => GetLocalisationIndex();

        public static Localisation[] Localisations => _localisations.ToArray();

        public static Localisation CurrentLocalisation => _currentLocalisation;

        public static string Translate(string text)
        {
            return Translate(text, _currentLocalisation);
        }

        public static string Translate(string text, Localisation localisation)
        {
            if (localisation != null && !string.IsNullOrEmpty(text) && localisation.CanTranslateIfUnavailable && DefaultLocalisationContainsText(text))
            {
                return GoogleTranslate.GetTranslationFor(text, localisation.IsoLanguageCode) ?? text;
            }

            return text;
        }

        public static bool Load()
        {
            var loadedAnything = false;

            LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Loading localisations from {LocalisationFilePath}...");

            _localisations.Clear();

            if (Directory.Exists(LocalisationFilePath))
            {
                var potentialLocalisations = Directory.GetFiles(LocalisationFilePath);
                
                foreach (var potentialLocalisation in potentialLocalisations)
                {
                    var localisation = LoadLocalisation(potentialLocalisation);
                    if (localisation != null)
                    {
                        Save(localisation, potentialLocalisation);
                    }
                }

                loadedAnything = true;
            }
            else
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Creating base directory at {LocalisationFilePath} because it doesn't already exist.");

                Directory.CreateDirectory(LocalisationFilePath);
            }

            SaveDefault();

            if (_localisations.Count == 0)
            {
                _localisations.Add(new Localisation());
            }

            _currentLocalisation = GetLocalisation(UserModSettings.Settings.Language);

            return loadedAnything;
        }

        public static bool SaveDefault()
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Updating default en-gb localisation.");

            var defaultLocalisation = new Localisation();
            var exampleFilePath = Path.Combine(LocalisationFilePath, "en-gb.xml");

            return Save(defaultLocalisation, exampleFilePath);
        }

        public static bool Save(Localisation localisation, string path)
        {
            if (localisation != null)
            {
                try
                {
                    using (var saveFile = File.CreateText(path))
                    {
                        var serialiser = Serialiser;
                        serialiser.Serialize(saveFile, localisation);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Error, "Couldn't save the default localisation file due to an error!");
                    LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, ex);
                }
            }

            return false;
        }

        public static Localisation GetLocalisation(string isoCode)
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Attempting to get the {isoCode} localisation...");

            var selectedLocalisation = _defaultLocalisation;

            if (!string.IsNullOrEmpty(isoCode))
            {
                var potentialLocalisation = _localisations.Find(localisation => localisation.IsoLanguageCode == isoCode);
                if (potentialLocalisation != null)
                {
                    LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Success!");

                    selectedLocalisation = potentialLocalisation;
                }
            }

            return selectedLocalisation;
        }

        public static void ChangeLocalisationFromName(string name)
        {
            var foundLocalisation = _localisations.Find(localisation => localisation.ReadableName == name);
            if (foundLocalisation != null)
            {
                ChangeLocalisation(foundLocalisation.IsoLanguageCode);
            }
        }

        public static void ChangeLocalisation(string isoCode)
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Changing localisation to {isoCode}.");

            _currentLocalisation = GetLocalisation(isoCode);
        }

        private static Localisation LoadLocalisation(string potentialLocalisation)
        {
            LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Attempting to load localisation from {potentialLocalisation}...");

            try
            {
                var localisationFileContents = File.ReadAllText(potentialLocalisation);
                var serialiser = Serialiser;

                using (var textReader = new StringReader(localisationFileContents))
                {
                    if (serialiser.Deserialize(textReader) is Localisation localisation)
                    {
                        _localisations.Add(localisation);

                        LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Message, $"Loaded!");

                        return localisation;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, LoggingWrapper.LogType.Error, $"Failed to load localisation at {potentialLocalisation}!");
                LoggingWrapper.Log(LoggingWrapper.LogArea.Hidden, ex);
            }

            return null;
        }

        private static int GetLocalisationIndex()
        {
            for (int index = 0; index < _localisations.Count; ++index)
            {
                var currentLocalisation = _localisations[index];
                if (currentLocalisation.IsoLanguageCode == _currentLocalisation.IsoLanguageCode)
                {
                    return index;
                }
            }

            return 0;
        }

        private static bool DefaultLocalisationContainsText(string text)
        {
            var fields = typeof(Localisation).GetFields();

            foreach (var field in fields)
            {
                if (typeof(string).IsAssignableFrom(field.FieldType))
                {
                    if (field.GetValue(_defaultLocalisation) is string fieldValue && !string.IsNullOrEmpty(fieldValue))
                    {
                        if (fieldValue == text)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
