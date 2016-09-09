using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using SteamMoverWPF.Entities;
using SteamMoverWPF.Utility;

namespace SteamMoverWPF.SteamManagement
{
    internal static class LibraryDetector
    {
        private static string DetectSteamPath()
        {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
            {
                string steamPath = registryKey?.GetValue("SteamPath") as string;
                try
                {
                    return steamPath.Replace("/", "\\");
                }
                catch (NullReferenceException ex)
                {
                    ErrorHandler.Instance.ShowCriticalErrorMessage("Steam path does not exist. Please run Steam atleast once.", ex);
                }            
            }
            return null;
        }

        private static BindingList<Library> DetectSteamLibraries(string steamPath, BindingList<Library> libraryList)
        {
            SteamConfigFile steamConfigFile = SteamConfigFileReader.ReadLibraryList(steamPath + "\\steamapps\\libraryfolders.vdf");
            if (steamConfigFile.ConfigType != "LibraryFolders")
            {
                ErrorHandler.Instance.ShowCriticalErrorMessage(steamPath + "\\steamapps\\libraryfolders.vdf has wrong format or it is not supported anymore. Try running steam once to fix corrupted file.");
            }
            //Adds Main Steam Library
            Library library = new Library();
            library.LibraryDirectory = steamPath;
            libraryList.Add(library);
            //Adds Other Steam Libraries
            foreach (SteamConfigFileProperty steamConfigFileProperty in steamConfigFile.SteamConfigFilePropertyList)
            {
                if (steamConfigFileProperty.Name.Equals("TimeNextStatsReport") || steamConfigFileProperty.Name.Equals("ContentStatsID"))
                {
                    continue;
                }
                if (!Directory.Exists(steamConfigFileProperty.Value + "\\SteamApps"))
                {
                    ErrorHandler.Instance.Log(steamConfigFileProperty.Value + "\\SteamApps" + "does not exist. Removed from list of libraries.");
                    continue;
                }

                library = new Library(); 
                library.LibraryDirectory = steamConfigFileProperty.Value;
                libraryList.Add(library);
            }
            bool isLibraryChaged = false;
            foreach (Library libraryLoop in libraryList)
            {
                string libraryDirectory = libraryLoop.LibraryDirectory;
                if (libraryDirectory.EndsWith("_removed"))
                {
                    libraryDirectory = libraryDirectory.Substring(0, libraryDirectory.Length - "_removed".Length);
                    FileSystem.RenameDirectory(libraryLoop.LibraryDirectory , Path.GetFileName(libraryDirectory));
                    isLibraryChaged = true;
                }
            }
            if (isLibraryChaged)
            {
                SteamConfigFileWriter.WriteLibraryList();
            }
            return libraryList;
        }
        private static void DetectSteamGames(Library library)
        {
            string[] filePaths = Directory.GetFiles(library.SteamAppsDirectory, "*.acf");
            SortableBindingList<Game> gamesList = new SortableBindingList<Game>();
            foreach (string file in filePaths)
            {
                SteamConfigFile steamConfigFile = SteamConfigFileReader.ReadACF(file);
                if (steamConfigFile.ConfigType != "AppState")
                {
                    ErrorHandler.Instance.ShowCriticalErrorMessage(file + " has wrong format or it is not supported anymore. Try running steam once to fix corrupted file.");
                }
                Game game = new Game();
                foreach (SteamConfigFileProperty steamConfigFileProperty in steamConfigFile.SteamConfigFilePropertyList)
                {
                    if (steamConfigFileProperty.Name.Equals("appID"))
                    {
                        game.AppID = Convert.ToInt32(steamConfigFileProperty.Value);
                    }
                    else if (steamConfigFileProperty.Name.Equals("name"))
                    {
                        game.GameName = steamConfigFileProperty.Value;
                    }
                    else if (steamConfigFileProperty.Name.Equals("installdir"))
                    {
                        game.GameFolder = steamConfigFileProperty.Value;
                    }
                    else if (steamConfigFileProperty.Name.Equals("SizeOnDisk"))
                    {
                        game.SizeOnDisk = Convert.ToInt64(steamConfigFileProperty.Value);
                    }
                }
                if (!Directory.Exists(library.SteamAppsDirectory + "\\common\\" + game.GameFolder))
                {
                    ErrorHandler.Instance.Log(library.SteamAppsDirectory + "\\common\\" + game.GameFolder + "does not exist. Removed from library list.");
                    continue;
                }
                game.RealSizeOnDiskIsChecked = false;
                gamesList.Add(game);
            }
            library.GamesList = gamesList;
        }
        public static void Run()
        {
            BindingDataContext.Instance.SteamPath = DetectSteamPath();
            BindingDataContext.Instance.LibraryList = DetectSteamLibraries(BindingDataContext.Instance.SteamPath, BindingDataContext.Instance.LibraryList);
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                DetectSteamGames(library);
            }
        }
        public static void Refresh()
        {
            //Rebuild libraries
            BindingList<Library> libraryList = new BindingList<Library>();
            string steamPath = DetectSteamPath();
            libraryList = DetectSteamLibraries(steamPath, libraryList);
            foreach (Library library in libraryList)
            {
                DetectSteamGames(library);
            }

            //Copy games sizes on disk
            foreach (Library libraryOld in BindingDataContext.Instance.LibraryList)
            {
                foreach (Library libraryNew in libraryList)
                {
                    if (libraryOld.SteamAppsDirectory == libraryNew.SteamAppsDirectory)
                    {
                        foreach (Game gameOld in libraryOld.GamesList)
                        {
                            foreach (Game gameNew in libraryNew.GamesList)
                            {
                                if (gameOld.AppID == gameNew.AppID)
                                {
                                    gameNew.RealSizeOnDisk = gameOld.RealSizeOnDisk;
                                    gameNew.RealSizeOnDiskIsChecked = gameOld.RealSizeOnDiskIsChecked;
                                    break;
                                }
                            }
                        }
                        break;
                    }

                }
            }
            //Sort list
            Game game = new Game();
            //  get property descriptions
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(game);
            //  get specific descriptor
            PropertyDescriptor property = properties.Find("GameName", false);
            foreach (Library library in libraryList)
            {
                library.GamesList.SortMyList(property, ListSortDirection.Ascending);
            }
            BindingDataContext.Instance.LibraryList = libraryList;
        }
    }
}
