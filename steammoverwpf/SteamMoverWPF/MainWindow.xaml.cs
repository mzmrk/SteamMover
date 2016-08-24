using SteamMoverWPF.Entities;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SteamMoverWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        volatile LibraryDetector libraryDetector = new LibraryDetector();
        LibraryManager libraryManager = new LibraryManager();
        //Thread threadRealSizeOnDisk;
        MainWindowViewModel mainWindowViewModel;

        public void init()
        {
            libraryDetector.run();
            foreach (Library library in libraryDetector.libraryList)
            {
                Game game = new Game();
                //  get property descriptions
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties ( game );
                //  get specific descriptor
                PropertyDescriptor property = properties.Find ( "GameName", false );
                library.GamesList.SortMyList(property, ListSortDirection.Ascending);
            }
            mainWindowViewModel = new MainWindowViewModel();
            mainWindowViewModel.Libraries = new BindingList<Library>(libraryDetector.libraryList);
            this.DataContext = mainWindowViewModel;
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += WorkThreadRealSizeOnDisk;
            //worker.RunWorkerCompleted += worker_RunWorkerCompleted
            worker.RunWorkerAsync();
            comboBoxLeft.SelectedIndex = 0;
            comboBoxRight.SelectedIndex = 1;
        }

        public void WorkThreadRealSizeOnDisk(object sender, DoWorkEventArgs e)
        {
                foreach (Library library in libraryDetector.libraryList)
                {
                    foreach (Game game in library.GamesList.ToArray())
                    {
                        libraryDetector.detectRealSizeOnDisk(library, game);
                        if (game.RealSizeOnDisk == -1)
                        {
                            // usuń z listy, oznacz jako nieaktywny, coś jest nie tak z tym folderem.
                            //library.GamesList.Remove(game);
                        }
                    }
                }
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
            mainWindowViewModel.GamesLeft = ((Library)comboBoxLeft.SelectedItem).GamesList;
        }


        private void comboBoxRight_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            mainWindowViewModel.GamesRight = ((Library)comboBoxRight.SelectedItem).GamesList;
        }


        private void buttonRight_Click_1(object sender, RoutedEventArgs e)
        {
            Library source = (Library)comboBoxLeft.SelectedItem;
            Library destination = (Library)comboBoxRight.SelectedItem;
            string gameFolder = ((Game)dataGridLeft.SelectedItem).GameDirectory;
            int appID = ((Game)dataGridLeft.SelectedItem).AppID;

            libraryManager.moveGame(source, destination, gameFolder);
            libraryManager.moveACF(source, destination, appID);

            destination.GamesList.Add((Game)dataGridLeft.SelectedItem);
            source.GamesList.Remove((Game)dataGridLeft.SelectedItem);
        }

        private void buttonLeft_Click_1(object sender, RoutedEventArgs e)
        {
            Library source = (Library)comboBoxRight.SelectedItem;
            Library destination = (Library)comboBoxLeft.SelectedItem;
            string gameFolder = ((Game)dataGridRight.SelectedItem).GameDirectory;
            int appID = ((Game)dataGridRight.SelectedItem).AppID;

            libraryManager.moveGame(source, destination, gameFolder);
            libraryManager.moveACF(source, destination, appID);

            destination.GamesList.Add((Game)dataGridRight.SelectedItem);
            source.GamesList.Remove((Game)dataGridRight.SelectedItem);
        }
    }
}
