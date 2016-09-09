using SteamMoverWPF.Entities;
using SteamMoverWPF.Tasks;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using SteamMoverWPF.SteamManagement;
using SteamMoverWPF.Utility;

namespace SteamMoverWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Library _comboBoxLeftSelectedItem;
        private Library _comboBoxRightSelectedItem;

        public void Init()
        {
            LibraryDetector.Run();
            this.DataContext = BindingDataContext.Instance;
            if (BindingDataContext.Instance.LibraryList.Count == 1)
            {
                BindingDataContext.Instance.SelectedLibraryComboboxLeft = BindingDataContext.Instance.LibraryList[0];
                BindingDataContext.Instance.SelectedLibraryComboboxRight = BindingDataContext.Instance.LibraryList[0];
            }
            else if (BindingDataContext.Instance.LibraryList.Count > 1)
            {
                BindingDataContext.Instance.SelectedLibraryComboboxLeft = BindingDataContext.Instance.LibraryList[0];
                BindingDataContext.Instance.SelectedLibraryComboboxRight = BindingDataContext.Instance.LibraryList[1];
            }

            Game game = new Game();
            //  get property descriptions
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(game);
            //  get specific descriptor
            PropertyDescriptor property = properties.Find("GameName", false);
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                library.GamesList.SortMyList(property, ListSortDirection.Ascending);
            }
            SortDescription sortDescription = new SortDescription("GameName", ListSortDirection.Ascending);
            DataGridLeft.Items.SortDescriptions.Add(sortDescription);
            DataGridRight.Items.SortDescriptions.Add(sortDescription);

            RealSizeOnDiskTask.Instance.Start();
        }


        private static void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            RealSizeOnDiskTask.Instance.Cancel();
            MessageBox.Show("Unknown error occured. Try restarting application. Details of an error will be shown after pressing OK.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            string errorMessage = $"An unhandled exception occurred: {e.Exception.Message} {e.Exception}";
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            ErrorHandler.Instance.ExitApplication();
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            Init();
        }
        private void comboBoxLeft_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxLeft.SelectedIndex != -1)
            {
                _comboBoxLeftSelectedItem = (Library)ComboBoxLeft.SelectedItem;
            } else
            {
                foreach (Library library in BindingDataContext.Instance.LibraryList)
                {
                    if (_comboBoxLeftSelectedItem.SteamAppsDirectory.Equals(library.SteamAppsDirectory, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ComboBoxLeft.SelectedItem = library;
                        return;
                    }
                }
                if (BindingDataContext.Instance.LibraryList.Count > 0)
                {
                    ComboBoxLeft.SelectedIndex = 0;
                }
            }
        }


        private void comboBoxRight_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxRight.SelectedIndex != -1)
            {
                _comboBoxRightSelectedItem = (Library)ComboBoxRight.SelectedItem;
            }
            else
            {
                foreach (Library library in BindingDataContext.Instance.LibraryList)
                {
                    if (_comboBoxRightSelectedItem.SteamAppsDirectory.Equals(library.SteamAppsDirectory, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ComboBoxRight.SelectedItem = library;
                        return;
                    }
                }
                if (BindingDataContext.Instance.LibraryList.Count > 0)
                {
                    ComboBoxRight.SelectedIndex = 0;
                }
            }
        }

        private void buttonRight_Click_1(object sender, RoutedEventArgs e)
        {
            if (DataGridLeft.SelectedIndex == -1)
            {
                ErrorHandler.Instance.ShowNotificationMessage("Please select game before moving.");
                return;
            }
            if (ComboBoxLeft.SelectedIndex == ComboBoxRight.SelectedIndex)
            {
                ErrorHandler.Instance.ShowNotificationMessage("You cannot move games between same libraries.");
                return;
            }
            Library source = (Library)ComboBoxLeft.SelectedItem;
            Library destination = (Library)ComboBoxRight.SelectedItem;
            Game selectedGame = (Game)DataGridLeft.SelectedItem;
            LibraryManager.MoveSteamGame(source, destination, selectedGame);
        }

        private void buttonLeft_Click_1(object sender, RoutedEventArgs e)
        {
            if (DataGridRight.SelectedIndex == -1)
            {
                ErrorHandler.Instance.ShowNotificationMessage("Please select game before moving.");
                return;
            }
            if (ComboBoxLeft.SelectedIndex == ComboBoxRight.SelectedIndex)
            {
                ErrorHandler.Instance.ShowNotificationMessage("You cannot move games between same libraries.");
                return;
            }
            Library source = (Library)ComboBoxRight.SelectedItem;
            Library destination = (Library)ComboBoxLeft.SelectedItem;
            Game selectedGame = (Game)DataGridRight.SelectedItem;
            LibraryManager.MoveSteamGame(source, destination, selectedGame);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            RealSizeOnDiskTask.Instance.Cancel();
            LibraryDetector.Refresh();
            RealSizeOnDiskTask.Instance.Start();

            bool isLibraryAdded = LibraryManager.AddLibrary();

            if (isLibraryAdded)
            {
                RealSizeOnDiskTask.Instance.Cancel();
                LibraryDetector.Refresh();
                RealSizeOnDiskTask.Instance.Start();
            } 
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            if (ComboBoxRight.SelectedIndex == 0)
            {
                ErrorHandler.Instance.ShowNotificationMessage("You cannot remove main library where main steam installation is located.");
                return;
            }
            Library libraryToRemove = (Library) ComboBoxRight.SelectedItem;
            RealSizeOnDiskTask.Instance.Cancel();
            LibraryDetector.Refresh();
            Library refreshedLibraryToRemove = null;
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                if (libraryToRemove.LibraryDirectory.Equals(library.LibraryDirectory,StringComparison.CurrentCultureIgnoreCase))
                {
                    refreshedLibraryToRemove = library;
                }
            }
            if (refreshedLibraryToRemove == null)
            {
                ErrorHandler.Instance.ShowErrorMessage("Library you are trying to remove is already removed.");
                return;
            }
            LibraryManager.RemoveLibrary((Library)ComboBoxRight.SelectedItem);
            RealSizeOnDiskTask.Instance.Start();
        }


        private void DataGridRight_GotFocus(object sender, RoutedEventArgs e)
        {
            DataGridLeft.SelectedIndex = -1;
        }

        private void DataGridLeft_GotFocus(object sender, RoutedEventArgs e)
        {
            DataGridRight.SelectedIndex = -1;
        }
    }
}
