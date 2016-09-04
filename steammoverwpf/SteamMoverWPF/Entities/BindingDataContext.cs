using System.ComponentModel;
using SteamMoverWPF.Utility;

namespace SteamMoverWPF.Entities
{
    public sealed class BindingDataContext : INotifyPropertyChanged
    {
        #region Singleton Stuff

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static BindingDataContext()
        {
        }

        private BindingDataContext()
        {
            LibraryList = new BindingList<Library>();
        }

        public static BindingDataContext Instance { get; } = new BindingDataContext();

        #endregion

        private string _steamPath;
        public string SteamPath
        {
            get { return _steamPath; }
            set { _steamPath = value; OnPropertyChanged("SteamPath"); }
        }
        private SortableBindingList<Game> _gamesLeft;
        public SortableBindingList<Game> GamesLeft
        {
            get { return _gamesLeft; }
            set { _gamesLeft = value; OnPropertyChanged("GamesLeft"); }
        }
        private SortableBindingList<Game> _gamesRight;
        public SortableBindingList<Game> GamesRight
        {
            get { return _gamesRight; }
            set { _gamesRight = value; OnPropertyChanged("GamesRight"); }
        }
        private BindingList<Library> _libraryList;
        public BindingList<Library> LibraryList
        {
            get { return _libraryList; }
            set { _libraryList = value; OnPropertyChanged("LibraryList"); }
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
