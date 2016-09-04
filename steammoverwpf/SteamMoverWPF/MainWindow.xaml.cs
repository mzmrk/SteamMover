using SteamMoverWPF.Entities;
using SteamMoverWPF.Tasks;
using SteamMoverWPF.Utility;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SteamMoverWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Library comboBoxLeftSelectedItem;
        Library comboBoxRightSelectedItem;


        public void init()
        {
            LibraryDetector.run();
            this.DataContext = BindingDataContext.Instance;

            if (BindingDataContext.Instance.LibraryList.Count == 1)
            {
                comboBoxLeft.SelectedIndex = 0;
            }
            else if (BindingDataContext.Instance.LibraryList.Count > 1)
            {
                comboBoxLeft.SelectedIndex = 0;
                comboBoxRight.SelectedIndex = 1;
            }

            Game game = new Game();
            //  get property descriptions
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(game);
            //  get specific descriptor
            PropertyDescriptor property = properties.Find("GameName", false);
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                library.GamesList.SortMyList(property, ListSortDirection.Ascending);
                library.GamesList.ListChanged += GamesList_ListChanged;
            }
            SortDescription sortDescription = new SortDescription("GameName", ListSortDirection.Ascending);
            dataGridLeft.Items.SortDescriptions.Add(sortDescription);
            dataGridRight.Items.SortDescriptions.Add(sortDescription);

            RealSizeOnDiskTask.Instance.start();
            
        }


        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0} {1}", e.Exception.Message, e.Exception.ToString());
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            init();
        }
        private void comboBoxLeft_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxLeft.SelectedIndex != -1)
            {
                BindingDataContext.Instance.GamesLeft = ((Library)comboBoxLeft.SelectedItem).GamesList;
                comboBoxLeftSelectedItem = (Library)comboBoxLeft.SelectedItem;
            } else
            {
                foreach (Library library in BindingDataContext.Instance.LibraryList)
                {
                    if (comboBoxLeftSelectedItem.SteamAppsDirectory.Equals(library.SteamAppsDirectory, StringComparison.InvariantCultureIgnoreCase))
                    {
                        comboBoxLeft.SelectedItem = library;
                        return;
                    }
                }
                if (BindingDataContext.Instance.LibraryList.Count > 0)
                {
                    comboBoxLeft.SelectedIndex = 0;
                }
            }
        }


        private void comboBoxRight_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxRight.SelectedIndex != -1)
            {
                BindingDataContext.Instance.GamesRight = ((Library)comboBoxRight.SelectedItem).GamesList;
                comboBoxRightSelectedItem = (Library)comboBoxRight.SelectedItem;
            }
            else
            {
                foreach (Library library in BindingDataContext.Instance.LibraryList)
                {
                    if (comboBoxRightSelectedItem.SteamAppsDirectory.Equals(library.SteamAppsDirectory, StringComparison.InvariantCultureIgnoreCase))
                    {
                        comboBoxRight.SelectedItem = library;
                        return;
                    }
                }
                if (BindingDataContext.Instance.LibraryList.Count > 0)
                {
                    comboBoxRight.SelectedIndex = 0;
                }
            }
        }

        private void GamesList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset && e.NewIndex == -1)
            {
                RealSizeOnDiskTask.Instance.restart();
            }
        }


        private void moveSteamGame(Library source, Library destination, Game selectedGame)
        {
            if (!UtilityBox.isSteamRunning())
            {
                string gameFolder = selectedGame.GameDirectory;
                int appID = selectedGame.AppID;
                RealSizeOnDiskTask.Instance.cancel();

                if (!LibraryManager.moveGame(source, destination, gameFolder))
                {
                    //TODO: Reverse move game folder operation? do some cleanup?
                    return;
                }
                if (!LibraryManager.moveACF(source, destination, appID))
                {
                    //TODO: Reverse move game folder operation? do some cleanup?
                    return;
                }
                destination.GamesList.Add(selectedGame);
                source.GamesList.Remove(selectedGame);

                RealSizeOnDiskTask.Instance.start();
            }
            else
            {
                MessageBox.Show("Turn Off Steam before moving any games.");
            }
        }

        private void buttonRight_Click_1(object sender, RoutedEventArgs e)
        {
            Library source = (Library)comboBoxLeft.SelectedItem;
            Library destination = (Library)comboBoxRight.SelectedItem;
            Game selectedGame = (Game)dataGridLeft.SelectedItem;
            moveSteamGame(source, destination, selectedGame);
        }

        private void buttonLeft_Click_1(object sender, RoutedEventArgs e)
        {
            Library source = (Library)comboBoxRight.SelectedItem;
            Library destination = (Library)comboBoxLeft.SelectedItem;
            Game selectedGame = (Game)dataGridRight.SelectedItem;
            moveSteamGame(source, destination, selectedGame);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Task task = Task.Run(() => {
                RealSizeOnDiskTask.Instance.cancel();
                LibraryDetector.refresh();
                RealSizeOnDiskTask.Instance.start();
            });
            bool isLibraryAdded = LibraryManager.addLibrary(task);
            if (isLibraryAdded)
            {
                RealSizeOnDiskTask.Instance.cancel();
                LibraryDetector.refresh();
                foreach (Library library in BindingDataContext.Instance.LibraryList)
                {
                    library.GamesList.ListChanged += GamesList_ListChanged;
                }
                RealSizeOnDiskTask.Instance.start();
            } 
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
            if (comboBoxRight.SelectedIndex > 0)
            {
                RealSizeOnDiskTask.Instance.cancel();
                LibraryManager.removeLibrary((Library)comboBoxRight.SelectedItem);
                RealSizeOnDiskTask.Instance.start();
            }

        }
    }
}
