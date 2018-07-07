using RushHour2.Core.Info;
using RushHour2.Core.Settings;
using System;
using System.Collections.Generic;
using System.IO;

namespace RushHour2.Core.Reporting
{
    public static class FileLogger
    {
        private static object _logLock = new object();
        private static bool _firstRun = true;
        private static DateTime _lastSaveTime = DateTime.Now;
        private static List<string> _queue = new List<string>();

        public static string SaveFileName => $"{Details.BaseModName} Log.xml";
        public static string SaveFilePath => SaveFileName;

        public static void Save()
        {
            lock (_logLock)
            {
                if (_firstRun)
                {
                    _firstRun = false;

                    Clear();
                }

                var timeSinceLastSave = DateTime.Now - _lastSaveTime;
                if (timeSinceLastSave > UserModSettings.Settings.Logging_ToFile_Duration)
                {
                    _lastSaveTime = DateTime.Now;

                    if (_queue.Count > 0)
                    {
                        LoggingWrapper.Log(LoggingWrapper.LogArea.File, LoggingWrapper.LogType.Message, "Log save point");

                        try
                        {
                            File.AppendAllText(SaveFilePath, string.Join("\n", _queue.ToArray()));
                        }
                        catch (Exception ex)
                        {
                            LoggingWrapper.Log(LoggingWrapper.LogArea.Debug, LoggingWrapper.LogType.Error, "Couldn't write log to file!");
                            LoggingWrapper.Log(LoggingWrapper.LogArea.Debug, ex);
                        }

                        _queue.Clear();
                    }
                }
            }
        }

        public static bool Clear()
        {
            lock (_logLock)
            {
                LoggingWrapper.Log(LoggingWrapper.LogArea.Debug, LoggingWrapper.LogType.Message, $"Clearing log file at {SaveFilePath}");

                try
                {
                    if (File.Exists(SaveFilePath))
                    {
                        File.Delete(SaveFilePath);

                        LoggingWrapper.Log(LoggingWrapper.LogArea.Debug, LoggingWrapper.LogType.Message, "Log file deleted");
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    LoggingWrapper.Log(LoggingWrapper.LogArea.Debug, LoggingWrapper.LogType.Error, "Couldn't clear the old log file!");
                    LoggingWrapper.Log(LoggingWrapper.LogArea.Debug, ex);
                }
            }

            return false;
        }

        public static void LogMessage(string message)
        {
            Log("[i]", message);
        }

        public static void LogWarning(string message)
        {
            Log("[#]", message);
        }

        public static void LogError(string message)
        {
            Log("[!]", message);
        }

        private static void Log(string prefix, string message)
        {
            lock (_logLock)
            {
                var currentTime = DateTime.Now.ToString("HH:mm:ss:FFFF");
                _queue.Add($"[{currentTime}] {prefix} {message}");
            }
        }
    }
}
