using SteamMoverWPF.Entities;
using System;
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
        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;
        private int taskRestartRequestedLock = 0;
        private volatile bool taskRestartRequested = false;
        int lastFinishedGameAppID = 0;
        double lastFInishedRealSizeOnDisk = 0;
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

            taskRestartRequested = true;
            restartRealSizeOnDiskTask();
        }

        public void refreshLibraries()
        {
            taskRestartRequested = false;
            cancellationTokenSource.Cancel();
            blockMainThread.WaitOne();
            LibraryDetector.refresh();
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
                    cancellationTokenSource = new CancellationTokenSource();
                    cancellationToken = cancellationTokenSource.Token;
                    realSizeOnDiskTask = new Task(WorkThreadRealSizeOnDisk, cancellationToken);
                    blockMainThread.Reset();
                    realSizeOnDiskTask.Start();
                }
            }
            Interlocked.Decrement(ref taskRestartRequestedLock);
        }

        public void WorkThreadRealSizeOnDisk()
        {
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                foreach (Game game in library.GamesList)
                {
                    if (!game.RealSizeOnDisk_isChecked)
                    {
                        if (lastFinishedGameAppID == game.AppID)
                        {
                            game.RealSizeOnDisk = lastFInishedRealSizeOnDisk;
                            game.RealSizeOnDisk_isChecked = true;
                            lastFinishedGameAppID = 0;
                            lastFInishedRealSizeOnDisk = 0;
                        }
                        else
                        {
                            blockMainThread.Set();
                            double realSizeOnDisk = LibraryDetector.GetWSHFolderSize(library.LibraryDirectory + "\\common\\" + game.GameDirectory);
                            if (cancellationToken.IsCancellationRequested)
                            {
                                lastFinishedGameAppID = game.AppID;
                                lastFInishedRealSizeOnDisk = realSizeOnDisk;
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
                    if (comboBoxLeftSelectedItem.LibraryDirectory.Equals(library.LibraryDirectory, StringComparison.InvariantCultureIgnoreCase))
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
                    if (comboBoxRightSelectedItem.LibraryDirectory.Equals(library.LibraryDirectory, StringComparison.InvariantCultureIgnoreCase))
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

                cancellationTokenSource.Cancel();
                realSizeOnDiskTask.Wait();

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

                restartRealSizeOnDiskTask();
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

                cancellationTokenSource.Cancel();
                realSizeOnDiskTask.Wait();

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

                restartRealSizeOnDiskTask();
            }
            else
            {
                MessageBox.Show("Turn Off Steam before moving any games.");
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Task task = Task.Run(() => refreshLibraries());
            bool isAdded = LibraryManager.addLibrary(task);
            if (isAdded)
            {
                refreshLibraries();
            } 
        }
    }
}
