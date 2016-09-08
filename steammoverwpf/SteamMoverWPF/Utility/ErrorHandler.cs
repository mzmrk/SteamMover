using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using SteamMoverWPF.Tasks;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

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
        public void ShowCriticalErrorMessage(string message, Exception ex)
        {
            RealSizeOnDiskTask.Instance.Cancel();
            ShowErrorMessage(message);
            ExitApplication();
        }
        public void ShowCriticalErrorMessage(string message)
        {
            RealSizeOnDiskTask.Instance.Cancel();
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            ExitApplication();
        }
        public void ShowNotificationMessage(string message)
        {
            MessageBox.Show(message, "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public bool ShowQuestion(string question)
        {
            MessageBoxResult dialogResult = MessageBox.Show(question, "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialogResult == MessageBoxResult.Yes)
            {
                return true;
            }
            return false;
        }

        [Conditional("DEBUG")]
        public void LogError(string message)
        {
            //throw new NotImplementedException();
        }

        public void ExitApplication()
        {
            LogError("Application is forcefully shutting down.");
            if (Application.Current != null && Application.Current.MainWindow.IsLoaded)
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