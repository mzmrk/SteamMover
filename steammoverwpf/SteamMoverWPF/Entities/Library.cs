using System.ComponentModel;
using System.Globalization;
using System.IO;
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
            set { throw new System.Exception("Do not edit this Property. Use LibraryDirectory instead."); }
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
                long freeSpaceInBytes = new DriveInfo(_libraryDirectory).AvailableFreeSpace;
                return ((double)freeSpaceInBytes / 1024 / 1024 / 1024).ToString("0.00", CultureInfo.InvariantCulture) + " GB";
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
