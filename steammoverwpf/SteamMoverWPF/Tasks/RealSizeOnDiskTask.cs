using SteamMoverWPF.Entities;
using SteamMoverWPF.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteamMoverWPF.Tasks
{
    class RealSizeOnDiskTask
    {
        #region Singleton Stuff
        private static readonly RealSizeOnDiskTask instance = new RealSizeOnDiskTask();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static RealSizeOnDiskTask()
        {
        }

        public static RealSizeOnDiskTask Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        Task realSizeOnDiskTask;
        CancellationTokenSource realSizeOnDiskCTS;
        CancellationToken RealSizeOnDiskCT;
        private int taskRestartRequestedLock = 0;
        private volatile bool taskRestartRequested = false;
        int lastFinishedGameAppID = 0;
        long lastFinishedRealSizeOnDisk = 0;
        ManualResetEvent blockMainThread = new ManualResetEvent(false);

        private RealSizeOnDiskTask()
        {

        }

        public void cancel()
        {
            if (realSizeOnDiskTask != null)
            {
                taskRestartRequested = false;
                realSizeOnDiskCTS.Cancel();
                blockMainThread.WaitOne();
            }
        }

        public async void start()
        {
            taskRestartRequested = true;
            if (Interlocked.Increment(ref taskRestartRequestedLock) == 1)
            {
                if (realSizeOnDiskTask != null) await realSizeOnDiskTask;
                if (taskRestartRequested)
                {
                    realSizeOnDiskCTS = new CancellationTokenSource();
                    RealSizeOnDiskCT = realSizeOnDiskCTS.Token;
                    realSizeOnDiskTask = new Task(WorkThreadRealSizeOnDisk, RealSizeOnDiskCT);
                    realSizeOnDiskTask.Start();
                }
            }
            Interlocked.Decrement(ref taskRestartRequestedLock);
        }


        public void restart()
        {
            cancel();
            start();
        }

        private void WorkThreadRealSizeOnDisk()
        {
            blockMainThread.Reset();
            if (lastFinishedGameAppID != 0)
            {
                foreach (Library library in BindingDataContext.Instance.LibraryList)
                {
                    foreach (Game game in library.GamesList)
                    {
                        if (lastFinishedGameAppID == game.AppID)
                        {
                            game.RealSizeOnDisk = lastFinishedRealSizeOnDisk;
                            game.RealSizeOnDisk_isChecked = true;
                            SteamConfigFileWriter.writeRealSizeOnDisk(library.SteamAppsDirectory + "\\appmanifest_" + lastFinishedGameAppID + ".acf", lastFinishedRealSizeOnDisk);
                            lastFinishedGameAppID = 0;
                            lastFinishedRealSizeOnDisk = 0;
                        }
                    }
                }
            }
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                foreach (Game game in library.GamesList)
                {
                    if (!game.RealSizeOnDisk_isChecked)
                    {
                        blockMainThread.Set();
                        long realSizeOnDisk = (long)UtilityBox.GetWSHFolderSize(library.SteamAppsDirectory + "\\common\\" + game.GameDirectory);
                        if (RealSizeOnDiskCT.IsCancellationRequested)
                        {
                            lastFinishedGameAppID = game.AppID;
                            lastFinishedRealSizeOnDisk = realSizeOnDisk;
                            return;
                        }
                        blockMainThread.Reset();
                        game.RealSizeOnDisk = realSizeOnDisk;
                        game.RealSizeOnDisk_isChecked = true;
                        SteamConfigFileWriter.writeRealSizeOnDisk(library.SteamAppsDirectory + "\\appmanifest_" + game.AppID + ".acf", realSizeOnDisk);

                        if (game.RealSizeOnDisk == -1)
                        {
                            // usuń z listy, oznacz jako nieaktywny, coś jest nie tak z tym folderem.
                            //library.GamesList.Remove(game);
                        }
                    }
                }
            }
            blockMainThread.Set();
        }
    }
}
