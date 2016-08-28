using SteamMoverWPF.Entities;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
        Task task;
        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;

        public void init()
        {
            LibraryDetector.run();
            this.DataContext = BindingDataContext.Instance;

            SortDescription sortDescription = new SortDescription("GameName", ListSortDirection.Ascending);
            dataGridLeft.Items.SortDescriptions.Add(sortDescription);
            dataGridRight.Items.SortDescriptions.Add(sortDescription);

            startTask();
        }

        public void startTask()
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            task = new Task(WorkThreadRealSizeOnDisk, cancellationToken);
            task.Start();
        }

        public void WorkThreadRealSizeOnDisk()
        {
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                foreach (Game game in library.GamesList.ToArray())
                {
                    if (!game.RealSizeOnDisk_isChecked)
                    {
                        LibraryDetector.detectRealSizeOnDisk(library, game);
                        if (game.RealSizeOnDisk == -1)
                        {
                            // usuń z listy, oznacz jako nieaktywny, coś jest nie tak z tym folderem.
                            //library.GamesList.Remove(game);
                        }
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                    }
                }
            }
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
            }
        }


        private void comboBoxRight_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxRight.SelectedIndex != -1)
            {
                BindingDataContext.Instance.GamesRight = ((Library)comboBoxRight.SelectedItem).GamesList;
                dataGridRight.Items.SortDescriptions.Clear();
                dataGridRight.Items.SortDescriptions.Add(new SortDescription("GameName", ListSortDirection.Ascending));
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
                task.Wait();

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

                startTask();
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
                task.Wait();

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

                startTask();
            }
            else
            {
                MessageBox.Show("Turn Off Steam before moving any games.");
            }
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            //LibraryManager.addLibrary();
            //LibraryDetector.refresh();
            //bindingDataContext.LibraryList = new BindingList<Library>(LibrariesContainer.Instance.LibraryList);
            //bindingDataContext.OnPropertyChanged("LibraryList");
            //bindingDataContext.OnPropertyChanged("GamesRight");
            //bindingDataContext.OnPropertyChanged("GamesLeft");
            //BindingDataContext.Instance.GamesRight.RemoveAt(8);
            cancellationTokenSource.Cancel();
            await task;

            LibraryDetector.refresh();

            comboBoxLeft.SelectedIndex = 0;
            comboBoxRight.SelectedIndex = 1;


            startTask();

        }
    }
}
