using System;
using System.Diagnostics;
using System.Windows;
using SteamMoverWPF.Tasks;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace SteamMoverWPF.Utility
{
    public class ErrorHandler
    {
        #region Singleton Stuff
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
        internal void Log(string message, Exception ex)
        {
            //throw new NotImplementedException();
        }
        [Conditional("DEBUG")]
        public void Log(string message)
        {
            //throw new NotImplementedException();
        }
        public void ExitApplication()
        {
            Log("Application is forcefully shutting down.");
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