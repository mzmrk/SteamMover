using SteamMoverWPF.Entities;
using System;
using System.Collections.Generic;
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
        public static void addLibrary()
        {
            //wybierz folder
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Create or select new Steam library folder:";


            ////=========== Refresh library list before adding


            while (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog.SelectedPath;
                //walidacja
                //sprawdz czy biblioteka jest juz dodana w pliku libiraries.vdf
                /*LibraryDetector libraryDetector = new LibraryDetector();
                string steamPath = libraryDetector.detectSteamPath();
                if (steamPath == null || steamPath == "")
                {
                    //TODO: Show error message!
                    throw new Exception("Steam path does not exist. Please run Steam atleast once.");
                }
                List<Library> libraryList = libraryDetector.detectSteamLibraries(steamPath);
                */
                //if (BindingDataContext.Instance.LibraryList.Exists(x => x.LibraryDirectory.Equals(selectedPath + "\\SteamApps", StringComparison.CurrentCultureIgnoreCase))) break;
                //sprawdz czy biblioteka jest juz dodana w klasie przechowującej biblioteki
                //MainWindow mainWindow;

                
                //sprawdz czy folder jest istniejącą biblioteką(w sensie folder steamapps musi byc obecny)
                //sprawdz czy folder jest pusty
                //cannot be drive root
                //cannot be current steam folder
                //cannot be in steamapps in one of current libraries

            }


            //add to libraries.vdf file
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
