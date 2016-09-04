using System.ComponentModel;
using SteamMoverWPF.Utility;

namespace SteamMoverWPF.Entities
{
    public class Library : INotifyPropertyChanged
    {
        private SortableBindingList<Game> _gamesList;
        public SortableBindingList<Game> GamesList
        {
            get { return _gamesList; }
            set { _gamesList = value; OnPropertyChanged("GamesList"); }
        }
        private string _libraryDirectory;
        public string LibraryDirectory
        {
            get { return _libraryDirectory; }
            set { _libraryDirectory = value; OnPropertyChanged("LibraryDirectory"); OnPropertyChanged("SteamAppsDirectory"); }
        }
        public string SteamAppsDirectory
        {
            get { return (_libraryDirectory + "\\SteamApps"); }
            set { throw new System.Exception("Do not edit this Property. Use LibraryDirectory instead."); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
