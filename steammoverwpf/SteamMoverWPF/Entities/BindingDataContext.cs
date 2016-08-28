using SteamMoverWPF.Entities;
using SteamMoverWPF.Util;
using System.ComponentModel;

namespace SteamMoverWPF
{
    public sealed class BindingDataContext : INotifyPropertyChanged
    {
        #region Singleton Stuff
        private static readonly BindingDataContext instance = new BindingDataContext();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static BindingDataContext()
        {
        }

        private BindingDataContext()
        {
            LibraryList = new BindingList<Library>();
        }

        public static BindingDataContext Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        private string steamPath;
        public string SteamPath
        {
            get { return steamPath; }
            set { steamPath = value; OnPropertyChanged("SteamPath"); }
        }
        private SortableBindingList<Game> gamesLeft;
        public SortableBindingList<Game> GamesLeft
        {
            get { return gamesLeft; }
            set { gamesLeft = value; OnPropertyChanged("GamesLeft"); }
        }
        private SortableBindingList<Game> gamesRight;
        public SortableBindingList<Game> GamesRight
        {
            get { return gamesRight; }
            set { gamesRight = value; OnPropertyChanged("GamesRight"); }
        }
        private BindingList<Library> libraryList;
        public BindingList<Library> LibraryList
        {
            get { return libraryList; }
            set { libraryList = value; OnPropertyChanged("LibraryList"); }
        }

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
