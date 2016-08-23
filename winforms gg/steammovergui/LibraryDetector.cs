using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SteamMoverGui
{
    class Game
    {
        public int appID;
        public string gameName;
        public string gameDirectory;
        public long sizeOnDisk;
        public double realSizeOnDisk;
    }
    class Library
    {
        public List<Game> gamesList;
        public string libraryDirectory;
    }
    class LibraryDetector
    {
        public List<Library> libraryList = new List<Library>();
        public string steamPath;
        string detectSteamPath()
        {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
            {
                object steamPath;
                try
                {
                    steamPath = registryKey.GetValue("SteamPath");
                }
                catch (NullReferenceException e)
                {
                    return "";
                }
                if (steamPath != null) {
                    String text = (String)steamPath;
                    text = text.Replace("/", "\\");
                    return (string)text;
                }
                return "";
            }
        }
        List<Library> detectSteamLibraries(string steamPath, List<Library> libraryList)
        {
            libraryList.Clear();
            ValveFileReader vfr = new ValveFileReader();
            vfr.readFile(steamPath + "\\steamapps\\libraryfolders.vdf");
            if (vfr.name != "LibraryFolders")
            {
                return null;
            }
            Library library = new Library();
            library.libraryDirectory = steamPath + "\\SteamApps";
            libraryList.Add(library);
            foreach (ValveFileItem vfi in vfr.itemsList)
	        {
                if (vfi.name.Equals("TimeNextStatsReport") || vfi.name.Equals("ContentStatsID"))
                {
                    continue;
                }
                library = new Library();
                library.libraryDirectory = vfi.value + "\\SteamApps";
                libraryList.Add(library);
	        }
            return libraryList;
        }
        Library detectSteamGames(Library library)
        {
            string[] filePaths = Directory.GetFiles(library.libraryDirectory, "*.acf");
            List<Game> gamesList = new List<Game>();
            foreach (string file in filePaths)
            {
                ValveFileReader vfr = new ValveFileReader();
                vfr.readFile(file);
                if (vfr.name != "AppState")
                {
                    return null;
                }
                Game game = new Game();
                foreach (ValveFileItem vfi in vfr.itemsList)
                {
                    if (vfi.name.Equals("appID"))
                    {
                        game.appID = Convert.ToInt32(vfi.value);
                    }
                    else if (vfi.name.Equals("name"))
                    {
                        game.gameName = vfi.value;
                    }
                    else if (vfi.name.Equals("installdir"))
                    {
                        game.gameDirectory = vfi.value;
                    }
                    else if (vfi.name.Equals("SizeOnDisk"))
                    {
                        game.sizeOnDisk = Convert.ToInt64(vfi.value);
                    }
                }
                //game.realSizeOnDisk = GetWSHFolderSize(library.libraryDirectory + "\\common\\" + game.gameDirectory);
                if (game.realSizeOnDisk == -1)
                {
                    // usuń z listy, oznacz jako nieaktywny, coś jest nie tak z tym folderem.
                }
                gamesList.Add(game);
            }
            library.gamesList = gamesList;
            return library;
        }
        public double GetWSHFolderSize(string Fldr)
        {
            //Reference "Windows Script Host Object Model" on the COM tab.
            IWshRuntimeLibrary.FileSystemObject FSO = new IWshRuntimeLibrary.FileSystemObject();
            double FldrSize;
            try
            {
                FldrSize = (double)FSO.GetFolder(Fldr).Size;
            }
            catch (DirectoryNotFoundException e)
            {
                return -1;
            }
            Marshal.FinalReleaseComObject(FSO);
            return FldrSize;
        }
        public void run()
        {
            libraryList = new List<Library>();
            steamPath = detectSteamPath();
            if (steamPath == null || steamPath == "")
            {
                System.Environment.Exit(1);
            }
            libraryList = detectSteamLibraries(steamPath, libraryList);
            foreach (Library library in libraryList)
            {
                detectSteamGames(library);
            }
        }
        public void detectRealSizeOnDisk(Library library, Game game)
        {

                    game.realSizeOnDisk = GetWSHFolderSize(library.libraryDirectory + "\\common\\" + game.gameDirectory);

        }
    }
}
