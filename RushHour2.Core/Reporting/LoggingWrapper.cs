using ColossalFramework.Plugins;
using RushHour2.Core.Info;
using RushHour2.Core.Settings;
using System;
using UnityEngine;

namespace RushHour2.Core.Reporting
{
    public static class LoggingWrapper
    {
        [Flags]
        public enum LogArea
        {
            File,
            Console,
            Debug,
            Hidden = File | Debug,
            All = File | Console | Debug
        }

        public enum LogType
        {
            Message,
            Warning,
            Error
        }

        public static void Log(LogArea area, Exception exception, bool recurse = false)
        {
            Log(area, LogType.Error, $"{exception?.Message ?? "null exception"}\n{exception.Source ?? "null source"}\n{exception?.StackTrace ?? "null stack"}");

            if (recurse && exception?.InnerException != null)
            {
                Log(area, exception.InnerException, recurse);
            }
        }

        public static void Log(LogArea area, LogType type, string message)
        {
            message = $"[{Details.ModName}] {message}";

            if ((area & LogArea.Console) != 0)
            {
                LogToConsole(type, message);
            }
            
            if ((area & LogArea.File) != 0)
            {
                LogToFile(type, message);
            }

            if ((area & LogArea.Debug) != 0)
            {
                LogToDebug(type, message);
            }
        }

        private static void LogToFile(LogType type, string message)
        {
            if (UserModSettings.LogToFile)
            {

            }
        }

        private static void LogToConsole(LogType type, string message)
        {
            DebugOutputPanel.AddMessage(ConvertToConsoleMessageType(type), message);
        }

        private static PluginManager.MessageType ConvertToConsoleMessageType(LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    return PluginManager.MessageType.Error;
                case LogType.Message:
                    return PluginManager.MessageType.Message;
                case LogType.Warning:
                    return PluginManager.MessageType.Warning;
            }

            return PluginManager.MessageType.Error;
        }

        private static void LogToDebug(LogType type, string message)
        {
            switch (type)
            {
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Message:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
            }
        }
    }
}
