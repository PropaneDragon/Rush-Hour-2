using ColossalFramework.UI;

namespace RushHour2.Core.Reporting
{
    public static class MessageBoxWrapper
    {
        public enum MessageType
        {
            Warning,
            Error
        }

        public static void Show(MessageType messageType, string caption, string message)
        {
            switch (messageType)
            {
                case MessageType.Error:
                    ShowError(caption, message);
                    break;
                case MessageType.Warning:
                    ShowWarning(caption, message);
                    break;
            }
        }

        private static void ShowWarning(string caption, string message)
        {
            UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(caption, message, false);
        }

        private static void ShowError(string caption, string message)
        {
            UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(caption, message, true);
        }
    }
}
