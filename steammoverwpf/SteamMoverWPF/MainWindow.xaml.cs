using SteamMoverWPF.Entities;
using SteamMoverWPF.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SteamMoverWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Library comboBoxLeftSelectedItem;
        Library comboBoxRightSelectedItem;


        Task realSizeOnDiskTask;
        CancellationTokenSource realSizeOnDiskCTS;
        CancellationToken RealSizeOnDiskCT;
        private int taskRestartRequestedLock = 0;
        private volatile bool taskRestartRequested = false;
        int lastFinishedGameAppID = 0;
        double lastFinishedRealSizeOnDisk = 0;
        ManualResetEvent blockMainThread = new ManualResetEvent(false);



        public void init()
        {
            LibraryDetector.run();
            this.DataContext = BindingDataContext.Instance;

            SortDescription sortDescription = new SortDescription("GameName", ListSortDirection.Ascending);
            dataGridLeft.Items.SortDescriptions.Add(sortDescription);
            dataGridRight.Items.SortDescriptions.Add(sortDescription);

            if (BindingDataContext.Instance.LibraryList.Count == 1)
            {
                comboBoxLeft.SelectedIndex = 0;
            } else if (BindingDataContext.Instance.LibraryList.Count > 1)
            {
                comboBoxLeft.SelectedIndex = 0;
                comboBoxRight.SelectedIndex = 1;
            }

            startRealSizeOnDiskTask();
        }

        public void cancelRealSizeOnDiskTask()
        {
            taskRestartRequested = false;
            realSizeOnDiskCTS.Cancel();
            blockMainThread.WaitOne();
        }

        public void startRealSizeOnDiskTask()
        {
            taskRestartRequested = true;
            restartRealSizeOnDiskTask();
        }


        public async void restartRealSizeOnDiskTask()
        {
            if (Interlocked.Increment(ref taskRestartRequestedLock) == 1)
            {
                if (realSizeOnDiskTask != null) await realSizeOnDiskTask;
                if (taskRestartRequested)
                {
                    realSizeOnDiskCTS = new CancellationTokenSource();
                    RealSizeOnDiskCT = realSizeOnDiskCTS.Token;
                    realSizeOnDiskTask = new Task(WorkThreadRealSizeOnDisk, RealSizeOnDiskCT);
                    realSizeOnDiskTask.Start();
                }
            }
            Interlocked.Decrement(ref taskRestartRequestedLock);
        }

        public void WorkThreadRealSizeOnDisk()
        {
            blockMainThread.Reset();
            //TODO: Clone lists of lists except for game objects -- they need to keep reference. Do it to make those loops sorting resistant!
            //Library library1 = new Library();
            //library1.
            //List<Library> lista = ((List<Library>)(BindingDataContext.Instance.LibraryList)).ConvertAll(item => (Library)item.Clone()).ToList()
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                foreach (Game game in library.GamesList)
                {
                    if (!game.RealSizeOnDisk_isChecked)
                    {
                        if (lastFinishedGameAppID == game.AppID)
                        {
                            game.RealSizeOnDisk = lastFinishedRealSizeOnDisk;
                            game.RealSizeOnDisk_isChecked = true;
                            lastFinishedGameAppID = 0;
                            lastFinishedRealSizeOnDisk = 0;
                        }
                        else
                        {
                            blockMainThread.Set();
                            double realSizeOnDisk = LibraryDetector.GetWSHFolderSize(library.SteamAppsDirectory + "\\common\\" + game.GameDirectory);
                            if (RealSizeOnDiskCT.IsCancellationRequested)
                            {
                                lastFinishedGameAppID = game.AppID;
                                lastFinishedRealSizeOnDisk = realSizeOnDisk;
                                return;
                            }
                            blockMainThread.Reset();
                            game.RealSizeOnDisk = realSizeOnDisk;
                            game.RealSizeOnDisk_isChecked = true;
                        }


                        if (game.RealSizeOnDisk == -1)
                        {
                            // usuń z listy, oznacz jako nieaktywny, coś jest nie tak z tym folderem.
                            //library.GamesList.Remove(game);
                        }
                    }
                }
            }
            blockMainThread.Set();
        }

        public bool isSteamRunning()
        {
            if (Process.GetProcessesByName("Steam").Length > 0)
            {
                return true;
            }
            return false;
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
                dataGridLeft.Items.SortDescriptions.Clear();
                dataGridLeft.Items.SortDescriptions.Add(new SortDescription("GameName", ListSortDirection.Ascending));
                comboBoxLeftSelectedItem = (Library)comboBoxLeft.SelectedItem;
            } else
            {
                foreach (Library library in BindingDataContext.Instance.LibraryList)
                {
                    if (comboBoxLeftSelectedItem.SteamAppsDirectory.Equals(library.SteamAppsDirectory, StringComparison.InvariantCultureIgnoreCase))
                    {
                        comboBoxLeft.SelectedItem = library;
                        break;
                    }
                }
            }
        }


        private void comboBoxRight_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxRight.SelectedIndex != -1)
            {
                BindingDataContext.Instance.GamesRight = ((Library)comboBoxRight.SelectedItem).GamesList;
                dataGridRight.Items.SortDescriptions.Clear();
                dataGridRight.Items.SortDescriptions.Add(new SortDescription("GameName", ListSortDirection.Ascending));
                comboBoxRightSelectedItem = (Library)comboBoxRight.SelectedItem;
            }
            else
            {
                foreach (Library library in BindingDataContext.Instance.LibraryList)
                {
                    if (comboBoxRightSelectedItem.SteamAppsDirectory.Equals(library.SteamAppsDirectory, StringComparison.InvariantCultureIgnoreCase))
                    {
                        comboBoxRight.SelectedItem = library;
                        break;
                    }
                }
            }
        }


        private void buttonRight_Click_1(object sender, RoutedEventArgs e)
        {
            if (!isSteamRunning())
            {
                Library source = (Library)comboBoxLeft.SelectedItem;
                Library destination = (Library)comboBoxRight.SelectedItem;
                string gameFolder = ((Game)dataGridLeft.SelectedItem).GameDirectory;
                int appID = ((Game)dataGridLeft.SelectedItem).AppID;

                cancelRealSizeOnDiskTask();

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
                destination.GamesList.Add((Game)dataGridLeft.SelectedItem);
                source.GamesList.Remove((Game)dataGridLeft.SelectedItem);
                dataGridRight.Items.SortDescriptions.Clear();
                dataGridRight.Items.SortDescriptions.Add(new SortDescription("GameName", ListSortDirection.Ascending));

                startRealSizeOnDiskTask();
            }
            else
            {
                MessageBox.Show("Turn Off Steam before moving any games.");
            }
        }

        private void buttonLeft_Click_1(object sender, RoutedEventArgs e)
        {
            if (!isSteamRunning())
            {
                Library source = (Library)comboBoxRight.SelectedItem;
                Library destination = (Library)comboBoxLeft.SelectedItem;
                string gameFolder = ((Game)dataGridRight.SelectedItem).GameDirectory;
                int appID = ((Game)dataGridRight.SelectedItem).AppID;

                cancelRealSizeOnDiskTask();

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
                destination.GamesList.Add((Game)dataGridRight.SelectedItem);
                source.GamesList.Remove((Game)dataGridRight.SelectedItem);
                dataGridLeft.Items.SortDescriptions.Clear();
                dataGridLeft.Items.SortDescriptions.Add(new SortDescription("GameName", ListSortDirection.Ascending));

                startRealSizeOnDiskTask();
            }
            else
            {
                MessageBox.Show("Turn Off Steam before moving any games.");
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Task task = Task.Run(() => {
                cancelRealSizeOnDiskTask();
                LibraryDetector.refresh();
                startRealSizeOnDiskTask();
            });
            bool isLibraryAdded = LibraryManager.addLibrary(task);
            if (isLibraryAdded)
            {
                cancelRealSizeOnDiskTask();
                LibraryDetector.refresh();
                startRealSizeOnDiskTask();
            } 
        }
    }
}
