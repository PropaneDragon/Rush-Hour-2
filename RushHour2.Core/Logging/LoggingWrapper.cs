using ColossalFramework.Plugins;
using RushHour2.Core.Settings;
using System;

namespace RushHour2.Core.Logging
{
    public static class LoggingWrapper
    {
        [Flags]
        public enum LogArea
        {
            File = 0,
            Console = 1,
            All = File | Console
        }

        public enum LogType
        {
            Message,
            Warning,
            Error
        }

        public static void Log(LogArea area, LogType type, string message)
        {
            if ((area & LogArea.Console) != 0)
            {
                LogToConsole(type, message);
            }
            
            if ((area & LogArea.File) != 0)
            {
                LogToFile(type, message);
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
            DebugOutputPanel.AddMessage(ConvertToMessageType(type), message);
        }

        private static PluginManager.MessageType ConvertToMessageType(LogType type)
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
    }
}
