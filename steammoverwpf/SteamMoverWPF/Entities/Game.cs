using System.ComponentModel;
using System.Globalization;

namespace SteamMoverWPF.Entities
{
    public class Game : INotifyPropertyChanged
    {
        private int appID;
        public int AppID
        {
            get { return appID; }
            set { appID = value; OnPropertyChanged("AppID"); }
        }
        private string gameName;
        public string GameName
        {
            get { return gameName; }
            set { gameName = value; OnPropertyChanged("GameName"); }
        }
        private string gameDirectory;
        public string GameDirectory
        {
            get { return gameDirectory; }
            set { gameDirectory = value; OnPropertyChanged("GameDirectory"); }
        }
        private long sizeOnDisk;
        public long SizeOnDisk
        {
            get { return sizeOnDisk; }
            set { sizeOnDisk = value; OnPropertyChanged("SizeOnDisk"); }
        }
        private double realSizeOnDisk;
        public double RealSizeOnDisk
        {
            get { return realSizeOnDisk; }
            set { realSizeOnDisk = value; OnPropertyChanged("RealSizeOnDisk"); OnPropertyChanged("RealSizeOnDiskString"); }
        }

        public string RealSizeOnDiskString
        {
            get { return (realSizeOnDisk / 1024 / 1024 / 1024).ToString("0.00", CultureInfo.InvariantCulture) + " GB"; }
            set {  }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
