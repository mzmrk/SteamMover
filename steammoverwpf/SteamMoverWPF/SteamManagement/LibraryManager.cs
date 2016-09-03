using Microsoft.VisualBasic;
using SteamMoverWPF.Entities;
using SteamMoverWPF.Util;
using SteamMoverWPF.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteamMoverWPF
{
    public static class LibraryManager
    {
        static bool moveFolder(string source, string destination)
        {
            InteropSHFileOperation interopSHFileOperation = new InteropSHFileOperation();
            interopSHFileOperation.wFunc = InteropSHFileOperation.FO_Func.FO_MOVE;
            interopSHFileOperation.pFrom = source;
            interopSHFileOperation.pTo = destination;
            interopSHFileOperation.fFlags.FOF_NOCONFIRMMKDIR = true;
            return interopSHFileOperation.Execute();
        }
        public static bool moveGame(Library source, Library destination, string gameFolder) {
            string src = source.SteamAppsDirectory + "\\common\\" + gameFolder;
            string dst = destination.SteamAppsDirectory + "\\common\\" + gameFolder;
            return moveFolder(src, dst);
        }

        public static bool moveACF(Library source, Library destination, int appID)
        {
            string sourceFile = source.SteamAppsDirectory + "\\" + "appmanifest_" + appID + ".acf";
            string destinationFile = destination.SteamAppsDirectory + "\\" + "appmanifest_" + appID+ ".acf";
            return moveFolder(sourceFile, destinationFile);
        }

        private static bool validateSelectedPath(string selectedPath)
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

            foreach (string directory in System.IO.Directory.GetDirectories(selectedPath))
            {
                string directoryTmp = Path.GetFileName(directory);
                if (string.Equals(directoryTmp, "steamapps", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            int countFiles = System.IO.Directory.GetFiles(selectedPath).Length;
            int countDirs = System.IO.Directory.GetDirectories(selectedPath).Length;
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

        public static bool addLibrary(Task task)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Create or select new Steam library folder:";
            bool isSelectedPathValidated = false;

            while (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                task.Wait();
                string selectedPath = folderBrowserDialog.SelectedPath;
                isSelectedPathValidated = validateSelectedPath(selectedPath);
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
                SteamConfigFileWriter.writeLibraryList();
                return true;
            }
            task.Wait();
            return false;
        }
        public static void removeLibrary(Library library)
        {
            //delete from libriries.vdf file
            BindingDataContext.Instance.LibraryList.Remove(library);
            SteamConfigFileWriter.writeLibraryList();
            //wyswietl komunikat ze biblioteka dalej jest na dysku, zostala tylko usuenieta ze steam.
            MessageBox.Show("Library will still exist on harddrive. It is only removed from the list.");
            //open windows explorer with library folder
            Process.Start(library.LibraryDirectory);
        }
        public static void renameLibrary(Library library)
        {
            string newLibraryName = Interaction.InputBox("type new library folder name", "Title", "Default Text");

            //library.LibraryDirectory
            //rename in libraries.vdf
            //ranme library folder

        }
    }
}
