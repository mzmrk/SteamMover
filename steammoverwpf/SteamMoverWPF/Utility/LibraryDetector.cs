﻿using Microsoft.Win32;
using SteamMoverWPF.Entities;
using SteamMoverWPF.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace SteamMoverWPF
{


    static class LibraryDetector
    {
        private static string detectSteamPath()
        {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
            {
                object steamPath;
                try
                {
                    steamPath = registryKey.GetValue("SteamPath");
                }
                catch (NullReferenceException)
                {
                    return "";
                }
                if (steamPath != null)
                {
                    String text = (String)steamPath;
                    text = text.Replace("/", "\\");
                    return (string)text;
                }
                return "";
            }
        }
        private static BindingList<Library> detectSteamLibraries(string steamPath, BindingList<Library> libraryList)
        {
            ValveFileReader vfr = new ValveFileReader();
            vfr.readFile(steamPath + "\\steamapps\\libraryfolders.vdf");
            if (vfr.name != "LibraryFolders")
            {
                //TODO: Implement error handling.
                throw new Exception("libraryfolder.vdf has wrong format.");
            }
            Library library = new Library();
            library.LibraryDirectory = steamPath + "\\SteamApps";
            libraryList.Add(library);
            foreach (ValveFileItem vfi in vfr.itemsList)
            {
                if (vfi.name.Equals("TimeNextStatsReport") || vfi.name.Equals("ContentStatsID"))
                {
                    continue;
                }
                //TODO: Temporary workarround for not existing library folders. In the future add possibility to manage library folders list directly in the tool.
                if (!Directory.Exists(vfi.value + "\\SteamApps"))
                {
                    continue;
                }
                library = new Library();
                library.LibraryDirectory = vfi.value + "\\SteamApps";
                libraryList.Add(library);
            }
            return libraryList;
        }
        private static Library detectSteamGames(Library library)
        {
            string[] filePaths = Directory.GetFiles(library.LibraryDirectory, "*.acf");
            SortableBindingList<Game> gamesList = new SortableBindingList<Game>();
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
                        game.AppID = Convert.ToInt32(vfi.value);
                    }
                    else if (vfi.name.Equals("name"))
                    {
                        game.GameName = vfi.value;
                    }
                    else if (vfi.name.Equals("installdir"))
                    {
                        game.GameDirectory = vfi.value;
                    }
                    else if (vfi.name.Equals("SizeOnDisk"))
                    {
                        game.SizeOnDisk = Convert.ToInt64(vfi.value);
                    }
                }
                //game.realSizeOnDisk = GetWSHFolderSize(library.libraryDirectory + "\\common\\" + game.gameDirectory);
                if (game.RealSizeOnDisk == -1)
                {
                    // usuń z listy, oznacz jako nieaktywny, coś jest nie tak z tym folderem.
                }
                game.RealSizeOnDisk_isChecked = false;
                gamesList.Add(game);
            }
            library.GamesList = gamesList;
            return library;
        }
        private static double GetWSHFolderSize(string Fldr)
        {
            //Reference "Windows Script Host Object Model" on the COM tab.
            IWshRuntimeLibrary.FileSystemObject FSO = new IWshRuntimeLibrary.FileSystemObject();
            double FldrSize;
            try
            {
                FldrSize = (double)FSO.GetFolder(Fldr).Size;
            }
            catch (DirectoryNotFoundException)
            {
                return -1;
            }
            Marshal.FinalReleaseComObject(FSO);
            return FldrSize;
        }
        public static void run()
        {
            BindingDataContext.Instance.SteamPath = detectSteamPath();
            if (BindingDataContext.Instance.SteamPath == null || BindingDataContext.Instance.SteamPath == "")
            {
                //TODO: Show error message!
                throw new Exception("Steam path does not exist. Please run Steam atleast once.");
            }
            BindingDataContext.Instance.LibraryList = detectSteamLibraries(BindingDataContext.Instance.SteamPath, BindingDataContext.Instance.LibraryList);
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                detectSteamGames(library);
            }
        }
        public static void refresh()
        {
            //Rebuild libraries
            BindingList<Library> libraryList = new BindingList<Library>();
            string steamPath = detectSteamPath();

            if (steamPath == null || steamPath == "")
            {
                //TODO: Show error message!
                throw new Exception("Steam path does not exist. Please run Steam atleast once.");
            }
            libraryList = detectSteamLibraries(steamPath, libraryList);
            foreach (Library library in libraryList)
            {
                detectSteamGames(library);
            }

            //Copy games sizes on disk
            foreach (Library libraryOld in BindingDataContext.Instance.LibraryList)
            {
                foreach (Library libraryNew in libraryList)
                {
                    if (libraryOld.LibraryDirectory == libraryNew.LibraryDirectory)
                    {
                        foreach (Game gameOld in libraryOld.GamesList)
                        {
                            foreach (Game gameNew in libraryNew.GamesList)
                            {
                                if (gameOld.AppID == gameNew.AppID)
                                {
                                    gameNew.RealSizeOnDisk = gameOld.RealSizeOnDisk;
                                    gameNew.RealSizeOnDisk_isChecked = gameOld.RealSizeOnDisk_isChecked;
                                    break;
                                }
                            }
                        }
                        break;
                    }

                }
            }


            //overwrite libraries in bindingdatacontext
            BindingDataContext.Instance.LibraryList = libraryList;
        }
        public static void detectRealSizeOnDisk(Library library, Game game)
        {
            game.RealSizeOnDisk = GetWSHFolderSize(library.LibraryDirectory + "\\common\\" + game.GameDirectory);
            game.RealSizeOnDisk_isChecked = true;
        }
    }
}