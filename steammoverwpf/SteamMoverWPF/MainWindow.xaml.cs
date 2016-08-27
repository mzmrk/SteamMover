using SteamMoverWPF.Entities;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace SteamMoverWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ManualResetEvent pauseBackgroundWorker = new ManualResetEvent(true);

        public void init()
        {
            LibraryDetector.run();
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                Game game = new Game();
                //  get property descriptions
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(game);
                //  get specific descriptor
                PropertyDescriptor property = properties.Find("GameName", false);
                library.GamesList.SortMyList(property, ListSortDirection.Ascending);
            }
            this.DataContext = BindingDataContext.Instance;
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += WorkThreadRealSizeOnDisk;
            //worker.RunWorkerAsync();
            comboBoxLeft.SelectedIndex = 0;
            comboBoxRight.SelectedIndex = 1;
        }

        public void WorkThreadRealSizeOnDisk(object sender, DoWorkEventArgs e)
        {
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                foreach (Game game in library.GamesList.ToArray())
                {
                    pauseBackgroundWorker.WaitOne();
                    LibraryDetector.detectRealSizeOnDisk(library, game);
                    if (game.RealSizeOnDisk == -1)
                    {
                        // usuń z listy, oznacz jako nieaktywny, coś jest nie tak z tym folderem.
                        //library.GamesList.Remove(game);
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
            //BindingDataContext.Instance.GamesLeft = ((Library)comboBoxLeft.SelectedItem).GamesList;
        }


        private void comboBoxRight_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            //BindingDataContext.Instance.GamesRight = ((Library)comboBoxRight.SelectedItem).GamesList;
        }


        private void buttonRight_Click_1(object sender, RoutedEventArgs e)
        {
            if (!isSteamRunning())
            {
                Library source = (Library)comboBoxLeft.SelectedItem;
                Library destination = (Library)comboBoxRight.SelectedItem;
                string gameFolder = ((Game)dataGridLeft.SelectedItem).GameDirectory;
                int appID = ((Game)dataGridLeft.SelectedItem).AppID;

                pauseBackgroundWorker.Reset();
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
                pauseBackgroundWorker.Set();

                destination.GamesList.Add((Game)dataGridLeft.SelectedItem);
                source.GamesList.Remove((Game)dataGridLeft.SelectedItem);
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

                pauseBackgroundWorker.Reset();
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
                pauseBackgroundWorker.Set();

                destination.GamesList.Add((Game)dataGridRight.SelectedItem);
                source.GamesList.Remove((Game)dataGridRight.SelectedItem);
            }
            else
            {
                MessageBox.Show("Turn Off Steam before moving any games.");
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //LibraryManager.addLibrary();
            LibraryDetector.refresh();
            //bindingDataContext.LibraryList = new BindingList<Library>(LibrariesContainer.Instance.LibraryList);
            //bindingDataContext.OnPropertyChanged("LibraryList");
            //bindingDataContext.OnPropertyChanged("GamesRight");
            //bindingDataContext.OnPropertyChanged("GamesLeft");
        }
    }
}
