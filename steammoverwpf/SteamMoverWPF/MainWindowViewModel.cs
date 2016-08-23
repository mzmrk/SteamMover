using SteamMoverWPF.Entities;
using SteamMoverWPF.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SteamMoverWPF
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
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
        private BindingList<Library> libraries;
        public BindingList<Library> Libraries
        {
            get { return libraries; }
            set { libraries = value; OnPropertyChanged("Libraries"); }
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
