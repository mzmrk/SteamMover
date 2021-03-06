﻿using System.ComponentModel;
using System.Globalization;
// ReSharper disable InconsistentNaming

namespace SteamMoverWPF.Entities
{
    public class Game : INotifyPropertyChanged
    {
        private int _appID;
        public int AppID
        {
            get { return _appID; }
            set { _appID = value; OnPropertyChanged("AppID"); }
        }
        private string _gameName;
        public string GameName
        {
            get { return _gameName; }
            set { _gameName = value; OnPropertyChanged("GameName"); }
        }
        private string _gameFolder;
        public string GameFolder
        {
            get { return _gameFolder; }
            set { _gameFolder = value; OnPropertyChanged("GameFolder"); }
        }
        private long _sizeOnDisk;
        public long SizeOnDisk
        {
            get { return _sizeOnDisk; }
            set { _sizeOnDisk = value; OnPropertyChanged("SizeOnDisk"); OnPropertyChanged("RealSizeOnDiskString"); OnPropertyChanged("RealSizeOnDiskLong"); }
        }
        private long _realSizeOnDisk;
        public long RealSizeOnDisk
        {
            get { return _realSizeOnDisk; }
            set { _realSizeOnDisk = value; OnPropertyChanged("RealSizeOnDisk"); OnPropertyChanged("RealSizeOnDiskString"); OnPropertyChanged("RealSizeOnDiskLong"); }
        }
        public long RealSizeOnDiskLong
        {
            get
            {
                if (_realSizeOnDiskIsChecked)
                {
                    return _realSizeOnDisk;
                }
                else
                {
                    return _sizeOnDisk;
                }
            }
        }
        public string RealSizeOnDiskString
        {
            get
            {
                if (_realSizeOnDiskIsChecked)
                {
                    return ((double)_realSizeOnDisk / 1024 / 1024 / 1024).ToString("0.00", CultureInfo.InvariantCulture) + " GB";
                } else
                {
                    return ((double)_sizeOnDisk / 1024 / 1024 / 1024).ToString("0.00", CultureInfo.InvariantCulture) + " GB";
                }
            }
        }
        private bool _realSizeOnDiskIsChecked;
        public bool RealSizeOnDiskIsChecked
        {
            get { return _realSizeOnDiskIsChecked; }
            set { _realSizeOnDiskIsChecked = value; OnPropertyChanged("RealSizeOnDiskIsChecked"); }
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
