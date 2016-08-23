using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamMoverWPF.Util;

namespace SteamMoverWPF.Entities
{
    class Library : INotifyPropertyChanged
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
            set { libraryDirectory = value; OnPropertyChanged("LibraryDirectory"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
