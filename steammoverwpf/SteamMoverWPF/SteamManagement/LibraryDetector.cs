using System;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using SteamMoverWPF.Entities;
using SteamMoverWPF.Utility;

namespace SteamMoverWPF.SteamManagement
{
    internal static class LibraryDetector
    {
        private static string DetectSteamPath()
        {
            string steamPath = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam")?.GetValue("SteamPath") as string;
            if (string.IsNullOrEmpty(steamPath))
            {
                //TODO: Show error when steam path cannot be found.
                throw new Exception("Steam path does not exist. Please run Steam atleast once.");
            }
            return steamPath.Replace("/", "\\");
        }

        private static BindingList<Library> DetectSteamLibraries(string steamPath, BindingList<Library> libraryList)
        {
            SteamConfigFile steamConfigFile = SteamConfigFileReader.ReadFile(steamPath + "\\steamapps\\libraryfolders.vdf");
            if (steamConfigFile.ConfigType != "LibraryFolders")
            {
                //TODO: Implement error handling.
                throw new Exception("libraryfolder.vdf has wrong format.");
            }
            Library library = new Library();
            library.LibraryDirectory = steamPath;
            libraryList.Add(library);
            foreach (SteamConfigFileProperty steamConfigFileProperty in steamConfigFile.SteamConfigFilePropertyList)
            {
                if (steamConfigFileProperty.Name.Equals("TimeNextStatsReport") || steamConfigFileProperty.Name.Equals("ContentStatsID"))
                {
                    continue;
                }
                //TODO: Temporary workarround for not existing library folders. In the future add possibility to manage library folders list directly in the tool.
                if (!Directory.Exists(steamConfigFileProperty.Value + "\\SteamApps"))
                {
                    continue;
                }
                library = new Library(); 
                library.LibraryDirectory = steamConfigFileProperty.Value;
                libraryList.Add(library);
            }
            return libraryList;
        }
        private static void DetectSteamGames(Library library)
        {
            string[] filePaths = Directory.GetFiles(library.SteamAppsDirectory, "*.acf");
            SortableBindingList<Game> gamesList = new SortableBindingList<Game>();
            foreach (string file in filePaths)
            {
                SteamConfigFile steamConfigFile = SteamConfigFileReader.ReadFile(file);
                Game game = new Game();
                foreach (SteamConfigFileProperty steamConfigFileProperty in steamConfigFile.SteamConfigFilePropertyList)
                {
                    if (steamConfigFileProperty.Name.Equals("appID"))
                    {
                        game.AppId = Convert.ToInt32(steamConfigFileProperty.Value);
                    }
                    else if (steamConfigFileProperty.Name.Equals("name"))
                    {
                        game.GameName = steamConfigFileProperty.Value;
                    }
                    else if (steamConfigFileProperty.Name.Equals("installdir"))
                    {
                        game.GameDirectory = steamConfigFileProperty.Value;
                    }
                    else if (steamConfigFileProperty.Name.Equals("SizeOnDisk"))
                    {
                        game.SizeOnDisk = Convert.ToInt64(steamConfigFileProperty.Value);
                    }
                }
                //game.realSizeOnDisk = GetWSHFolderSize(library.libraryDirectory + "\\common\\" + game.gameDirectory);
                if (game.RealSizeOnDisk == -1)
                {
                    // usuń z listy, oznacz jako nieaktywny, coś jest nie tak z tym folderem.
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
                                if (gameOld.AppId == gameNew.AppId)
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
