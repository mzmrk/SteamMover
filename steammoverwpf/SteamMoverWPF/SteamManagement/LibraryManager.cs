using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using SteamMoverWPF.Entities;
using SteamMoverWPF.Tasks;
using SteamMoverWPF.Utility;

namespace SteamMoverWPF.SteamManagement
{
    public static class LibraryManager
    {
        private static bool MoveFolder(string source, string destination)
        {
            InteropShFileOperation interopShFileOperation = new InteropShFileOperation();
            interopShFileOperation.WFunc = InteropShFileOperation.FoFunc.FoMove;
            interopShFileOperation.PFrom = source;
            interopShFileOperation.PTo = destination;
            interopShFileOperation.FFlags.FofNoconfirmmkdir = true;
            return interopShFileOperation.Execute();
        }
        public static bool MoveGameFolder(Library source, Library destination, string gameFolder) {
            string src = source.SteamAppsDirectory + "\\common\\" + gameFolder;
            string dst = destination.SteamAppsDirectory + "\\common\\" + gameFolder;
            return MoveFolder(src, dst);
        }

        public static bool MoveAcfFile(Library source, Library destination, int appId)
        {
            string sourceFile = source.SteamAppsDirectory + "\\" + "appmanifest_" + appId + ".acf";
            string destinationFile = destination.SteamAppsDirectory + "\\" + "appmanifest_" + appId+ ".acf";
            return MoveFolder(sourceFile, destinationFile);
        }

        private static bool ValidateSelectedPath(string selectedPath)
        {
            if ((selectedPath == Path.GetPathRoot(selectedPath)))
            {
                MessageBox.Show("Cannot be RootFolder");
                return false;
            }

            string combinedPaths = Path.Combine(selectedPath, "SteamApps").ToLower();
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                if (combinedPaths.Contains(library.SteamAppsDirectory.ToLower()))
                {
                    MessageBox.Show("Cannot be already added library");
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
                MessageBox.Show("Selected folder must be empty.");
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool AddLibrary(Task task)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Create or select new Steam library folder:";
            bool isSelectedPathValidated = false;

            while (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                task.Wait();
                string selectedPath = folderBrowserDialog.SelectedPath;
                isSelectedPathValidated = ValidateSelectedPath(selectedPath);
                if (isSelectedPathValidated)
                {
                    break;
                }
            }
            if (isSelectedPathValidated)
            {
                //dodaj wpis do libraries.vdf
                Library library = new Library();
                library.GamesList = new SortableBindingList<Game>();
                library.LibraryDirectory = folderBrowserDialog.SelectedPath;
                //TODO: Add steamapps folder to the new library!
                Directory.CreateDirectory(folderBrowserDialog.SelectedPath + "\\steamapps");
                BindingDataContext.Instance.LibraryList.Add(library);
                SteamConfigFileWriter.WriteLibraryList();
                return true;
            }
            task.Wait();
            return false;
        }
        public static void RemoveLibrary(Library library)
        {
            //delete from libriries.vdf file
            BindingDataContext.Instance.LibraryList.Remove(library);
            SteamConfigFileWriter.WriteLibraryList();
            //wyswietl komunikat ze biblioteka dalej jest na dysku, zostala tylko usuenieta ze steam.
            MessageBox.Show("Library will still exist on harddrive. It is only removed from the list.");
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
            if (!UtilityBox.IsSteamRunning())
            {
                string gameFolder = selectedGame.GameDirectory;
                int appId = selectedGame.AppId;
                RealSizeOnDiskTask.Instance.Cancel();

                if (!MoveGameFolder(source, destination, gameFolder))
                {
                    //TODO: Reverse move game folder operation? do some cleanup?
                    return;
                }
                if (!MoveAcfFile(source, destination, appId))
                {
                    //TODO: Reverse move game folder operation? do some cleanup?
                    return;
                }
                destination.GamesList.Add(selectedGame);
                source.GamesList.Remove(selectedGame);
                destination.OnPropertyChanged("LibrarySizeOnDisk");
                source.OnPropertyChanged("LibrarySizeOnDisk");
                destination.OnPropertyChanged("FreeSpaceOnDisk");
                source.OnPropertyChanged("FreeSpaceOnDisk");

                RealSizeOnDiskTask.Instance.Start();
            }
            else
            {
                MessageBox.Show("Turn Off Steam before moving any games.");
            }
        }
    }
}
