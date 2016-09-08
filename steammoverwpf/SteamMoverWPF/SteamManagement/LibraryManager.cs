using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using SteamMoverWPF.Entities;
using SteamMoverWPF.Tasks;
using SteamMoverWPF.Utility;

namespace SteamMoverWPF.SteamManagement
{
    public static class LibraryManager
    {
        public static bool MoveGameFolder(Library source, Library destination, string gameFolder) {
            string src = source.SteamAppsDirectory + "\\common\\" + gameFolder;
            string dst = destination.SteamAppsDirectory + "\\common\\" + gameFolder;
            try
            {
                FileSystem.MoveDirectory(src, dst, UIOption.AllDialogs);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            return true;
        }

        public static bool MoveAcfFile(Library source, Library destination, int appId)
        {
            string src = source.SteamAppsDirectory + "\\" + "appmanifest_" + appId + ".acf";
            string dst = destination.SteamAppsDirectory + "\\" + "appmanifest_" + appId+ ".acf";
            try
            {
                FileSystem.MoveFile(src, dst, UIOption.AllDialogs);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            return true;
        }

        private static bool ValidateSelectedPath(string selectedPath)
        {
            if ((selectedPath == Path.GetPathRoot(selectedPath)))
            {
                ErrorHandler.Instance.ShowErrorMessage("Cannot be RootFolder");
                return false;
            }

            string combinedPaths = Path.Combine(selectedPath, "SteamApps").ToLower();
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                if (combinedPaths.Contains(library.SteamAppsDirectory.ToLower()))
                {
                    ErrorHandler.Instance.ShowErrorMessage("Library is already on the list.");
                    return false;
                }
            }

            foreach (string directory in Directory.GetDirectories(selectedPath))
            {
                string directoryTmp = Path.GetFileName(directory);
                if (string.Equals(directoryTmp, "steamapps", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            int countFiles = Directory.GetFiles(selectedPath).Length;
            int countDirs = Directory.GetDirectories(selectedPath).Length;
            if (!(countFiles == 0 && countDirs == 0))
            {
                ErrorHandler.Instance.ShowErrorMessage("Selected folder must be empty.");
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool AddLibrary()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Create or select new Steam library folder:";
            bool isSelectedPathValidated = false;

            while (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog.SelectedPath;
                isSelectedPathValidated = ValidateSelectedPath(selectedPath);
                if (isSelectedPathValidated)
                {
                    break;
                }
            }
            if (!isSelectedPathValidated)
            {
                return false;
            }
            Library library = new Library();
            library.GamesList = new SortableBindingList<Game>();
            library.LibraryDirectory = folderBrowserDialog.SelectedPath;
            Directory.CreateDirectory(folderBrowserDialog.SelectedPath + "\\steamapps");
            BindingDataContext.Instance.LibraryList.Add(library);
            SteamConfigFileWriter.WriteLibraryList();
            return true;
        }
        public static void RemoveLibrary(Library library)
        {
            BindingDataContext.Instance.LibraryList.Remove(library);
            SteamConfigFileWriter.WriteLibraryList();
            ErrorHandler.Instance.ShowNotificationMessage("Library will still exist on harddrive. It is only removed from the list.");
            //open windows explorer with library folder
            Process.Start(library.LibraryDirectory);
        }
        public static void RenameLibrary(Library library)
        {
            //string newLibraryName = Interaction.InputBox("type new library folder name", "Title", "Default Text");

            //library.LibraryDirectory
            //rename in libraries.vdf
            //ranme library folder

        }

        public static void MoveSteamGame(Library source, Library destination, Game selectedGame)
        {
            if (UtilityBox.IsSteamRunning())
            {
                ErrorHandler.Instance.ShowNotificationMessage("Turn Off Steam before moving any games.");
                return;
            }
            if (Directory.Exists(destination.SteamAppsDirectory + "\\common\\" + selectedGame.GameFolder))
            {
                bool response = ErrorHandler.Instance.ShowQuestion("Game installation of " + selectedGame.GameName + "already exists in destination library. Do you want to overwrite?");
                if (!response) return;
            }
            RealSizeOnDiskTask.Instance.Cancel();
            if (!MoveGameFolder(source, destination, selectedGame.GameFolder))
            {
                ErrorHandler.Instance.ShowNotificationMessage("Game moving was aborted. There might be some inconsistencies in game folder. It is advised to finish game move operation.");
                return;
            }
            SortableBindingList<Game> gamesToRemove = new SortableBindingList<Game>();
            foreach (Game game in source.GamesList)
            {
                if (selectedGame.GameFolder.Equals(game.GameFolder, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!MoveAcfFile(source, destination, game.AppID))
                    {
                        ErrorHandler.Instance.ShowNotificationMessage("Game moving was aborted. Some games might now show up in steam library.");
                        return;
                    }
                    gamesToRemove.Add(game);
                }
            }
            foreach (Game game in gamesToRemove)
            {
                destination.GamesList.Add(game);
                source.GamesList.Remove(game);
            }

            RealSizeOnDiskTask.Instance.Start();
            destination.OnPropertyChanged("LibrarySizeOnDisk");
            source.OnPropertyChanged("LibrarySizeOnDisk");
            destination.OnPropertyChanged("FreeSpaceOnDisk");
            source.OnPropertyChanged("FreeSpaceOnDisk");
        }
    }
}
