using SteamMoverWPF.Entities;
using SteamMoverWPF.Utility;
using System.Threading;
using System.Threading.Tasks;
using SteamMoverWPF.SteamManagement;

namespace SteamMoverWPF.Tasks
{
    internal class RealSizeOnDiskTask
    {
        #region Singleton Stuff

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static RealSizeOnDiskTask()
        {
        }

        public static RealSizeOnDiskTask Instance { get; } = new RealSizeOnDiskTask();

        #endregion

        private Task _realSizeOnDiskTask;
        private CancellationTokenSource _realSizeOnDiskCts;
        private CancellationToken _realSizeOnDiskCt;
        private int _taskRestartRequestedLock;
        private volatile bool _taskRestartRequested;
        private int _lastFinishedGameAppId;
        private long _lastFinishedRealSizeOnDisk;
        private readonly ManualResetEvent _blockMainThread = new ManualResetEvent(false);

        private RealSizeOnDiskTask()
        {

        }

        public void Cancel()
        {
            if (_realSizeOnDiskTask != null)
            {
                _taskRestartRequested = false;
                _realSizeOnDiskCts.Cancel();
                _blockMainThread.WaitOne();
            }
        }

        public async void Start()
        {
            _taskRestartRequested = true;
            if (Interlocked.Increment(ref _taskRestartRequestedLock) == 1)
            {
                if (_realSizeOnDiskTask != null) await _realSizeOnDiskTask;
                if (_taskRestartRequested)
                {
                    _realSizeOnDiskCts = new CancellationTokenSource();
                    _realSizeOnDiskCt = _realSizeOnDiskCts.Token;
                    _realSizeOnDiskTask = new Task(WorkThreadRealSizeOnDisk, _realSizeOnDiskCt);
                    _realSizeOnDiskTask.Start();
                }
            }
            Interlocked.Decrement(ref _taskRestartRequestedLock);
        }


        public void Restart()
        {
            Cancel();
            Start();
        }

        private void WorkThreadRealSizeOnDisk()
        {
            _blockMainThread.Reset();
            if (_lastFinishedGameAppId != 0)
            {
                foreach (Library library in BindingDataContext.Instance.LibraryList)
                {
                    foreach (Game game in library.GamesList)
                    {
                        if (_lastFinishedGameAppId == game.AppId)
                        {
                            game.RealSizeOnDisk = _lastFinishedRealSizeOnDisk;
                            game.RealSizeOnDiskIsChecked = true;
                            library.OnPropertyChanged("LibrarySizeOnDisk");
                            SteamConfigFileWriter.WriteRealSizeOnDisk(library.SteamAppsDirectory + "\\appmanifest_" + _lastFinishedGameAppId + ".acf", _lastFinishedRealSizeOnDisk);
                            _lastFinishedGameAppId = 0;
                            _lastFinishedRealSizeOnDisk = 0;
                        }
                    }
                }
            }
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                foreach (Game game in library.GamesList)
                {
                    if (!game.RealSizeOnDiskIsChecked)
                    {
                        _blockMainThread.Set();
                        long realSizeOnDisk = (long)UtilityBox.GetWshFolderSize(library.SteamAppsDirectory + "\\common\\" + game.GameDirectory);
                        if (_realSizeOnDiskCt.IsCancellationRequested)
                        {
                            _lastFinishedGameAppId = game.AppId;
                            _lastFinishedRealSizeOnDisk = realSizeOnDisk;
                            return;
                        }
                        _blockMainThread.Reset();
                        game.RealSizeOnDisk = realSizeOnDisk;
                        game.RealSizeOnDiskIsChecked = true;
                        library.OnPropertyChanged("LibrarySizeOnDisk");
                        SteamConfigFileWriter.WriteRealSizeOnDisk(library.SteamAppsDirectory + "\\appmanifest_" + game.AppId + ".acf", realSizeOnDisk);

                        if (game.RealSizeOnDisk == -1)
                        {
                            // usuń z listy, oznacz jako nieaktywny, coś jest nie tak z tym folderem.
                            //library.GamesList.Remove(game);
                        }
                    }
                }
            }
            _blockMainThread.Set();
        }
    }
}
