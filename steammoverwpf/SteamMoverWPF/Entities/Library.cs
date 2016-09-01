using System.ComponentModel;
using SteamMoverWPF.Util;

namespace SteamMoverWPF.Entities
{
    public class Library : INotifyPropertyChanged
    {
        private SortableBindingList<Game> gamesList;
        public SortableBindingList<Game> GamesList
        {
            get { return gamesList; }
            set { gamesList = value; OnPropertyChanged("GamesList"); }
        }
        private string libraryDirectory;
        public string LibraryDirectory
        {
            get { return libraryDirectory; }
            set { libraryDirectory = value; OnPropertyChanged("LibraryDirectory"); OnPropertyChanged("SteamAppsDirectory"); }
        }
        public string SteamAppsDirectory
        {
            get { return (libraryDirectory + "\\SteamApps"); }
            set { throw new System.Exception("Do not edit this Property. Use LibraryDirectory instead."); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
