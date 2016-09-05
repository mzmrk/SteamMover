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
        private Library _selectedLibraryComboboxLeft;
        public Library SelectedLibraryComboboxLeft
        {
            get { return _selectedLibraryComboboxLeft; }
            set { _selectedLibraryComboboxLeft = value; OnPropertyChanged("SelectedLibraryComboboxLeft"); }
        }
        private Library _selectedLibraryComboboxRight;
        public Library SelectedLibraryComboboxRight
        {
            get { return _selectedLibraryComboboxRight; }
            set { _selectedLibraryComboboxRight = value; OnPropertyChanged("SelectedLibraryComboboxRight"); }
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
