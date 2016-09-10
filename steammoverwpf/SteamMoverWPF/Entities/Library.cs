using System.ComponentModel;
using System.Globalization;
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
            set { _libraryDirectory = value; OnPropertyChanged("LibraryDirectory"); OnPropertyChanged("SteamAppsDirectory"); OnPropertyChanged("FreeSpaceOnDisk"); }
        }
        public string SteamAppsDirectory
        {
            get { return (_libraryDirectory + "\\SteamApps"); }
        }
        public string LibrarySizeOnDisk
        {
            get
            {
                long size = 0;
                foreach (Game game in _gamesList)
                {
                    if (game.RealSizeOnDiskIsChecked)
                    {
                        size += game.RealSizeOnDisk;
                    }
                    else
                    {
                        size += game.SizeOnDisk;
                    }
                }
                return ((double)size / 1024 / 1024 / 1024).ToString("0.00", CultureInfo.InvariantCulture) + " GB";

            }
        }
        public string FreeSpaceOnDisk
        {
            get
            {
                long freeSpaceInBytes = GetDiskFreeSpace.FreeSpace(_libraryDirectory);
                return ((double)freeSpaceInBytes / 1024 / 1024 / 1024).ToString("0.00", CultureInfo.InvariantCulture) + " GB";
            }
        }
        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
