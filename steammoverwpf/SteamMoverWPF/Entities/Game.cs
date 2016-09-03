using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

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
            set { sizeOnDisk = value; OnPropertyChanged("SizeOnDisk"); OnPropertyChanged("RealSizeOnDiskString"); OnPropertyChanged("RealSizeOnDiskChecked"); }
        }
        private long realSizeOnDisk;
        public long RealSizeOnDisk
        {
            get { return realSizeOnDisk; }
            set { realSizeOnDisk = value; OnPropertyChanged("RealSizeOnDisk"); OnPropertyChanged("RealSizeOnDiskString"); OnPropertyChanged("RealSizeOnDiskChecked"); }
        }

        public long RealSizeOnDiskChecked
        {
            get
            {
                if (realSizeOnDisk_isChecked)
                {
                    return realSizeOnDisk;
                }
                else
                {
                    return sizeOnDisk;
                }
            }
            set { }
        }

        public string RealSizeOnDiskString
        {
            get
            {
                if (realSizeOnDisk_isChecked)
                {
                    return (realSizeOnDisk / 1024 / 1024 / 1024).ToString("0.00", CultureInfo.InvariantCulture) + " GB";
                } else
                {
                    return (sizeOnDisk / 1024 / 1024 / 1024).ToString("0.00", CultureInfo.InvariantCulture) + " GB";
                }
            }
            set {  }
        }
        private bool realSizeOnDisk_isChecked;
        public bool RealSizeOnDisk_isChecked
        {
            get { return realSizeOnDisk_isChecked; }
            set { realSizeOnDisk_isChecked = value; OnPropertyChanged("RealSizeOnDisk_isChecked"); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
