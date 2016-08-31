using SteamMoverWPF.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            string src = source.LibraryDirectory + "\\common\\" + gameFolder;
            string dst = destination.LibraryDirectory + "\\common\\" + gameFolder;
            return moveFolder(src, dst);
        }

        public static bool moveACF(Library source, Library destination, int appID)
        {
            string sourceFile = source.LibraryDirectory + "\\" + "appmanifest_" + appID + ".acf";
            string destinationFile = destination.LibraryDirectory + "\\" + "appmanifest_" + appID+ ".acf";
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
                if (combinedPaths.Contains(library.LibraryDirectory.ToLower()))
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
            //wybierz folder
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

                return true;
            } else
            {
                task.Wait();
                return false;
            }
        }
        public static void removeLibrary()
        {
            //delete from libriries.vdf file
            //wyswietl komunikat ze biblioteka dalej jest na dysku, zostala tylko usuenieta ze steam.
            //open windows explorer with library folder
        }
        public static void renameLibrary()
        {
            //rename in libraries.vdf
            //ranme library folder
        }
    }
}
