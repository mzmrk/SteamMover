using System;
using System.Diagnostics;
using System.Windows;
using SteamMoverWPF.Tasks;

namespace SteamMoverWPF.Utility
{
    public class ErrorHandler
    {
        #region Singleton Stuff
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ErrorHandler()
        {
        }

        private ErrorHandler()
        {

        }

        public static ErrorHandler Instance { get; } = new ErrorHandler();

        #endregion

        public void ShowErrorMessage(string message, Exception ex)
        {
            ShowErrorMessage(message);
        }
        public void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public void ShowNotificationMessage(string message)
        {
            MessageBox.Show(message, "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        [Conditional("DEBUG")]
        public void LogError(string message)
        {
            //throw new NotImplementedException();
        }

        public void ExitApplication()
        {
            LogError("Application is forcefully shutting down.");
            RealSizeOnDiskTask.Instance.Cancel();
            if (Application.Current.MainWindow.IsLoaded)
            {
                Application.Current.Shutdown();
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}