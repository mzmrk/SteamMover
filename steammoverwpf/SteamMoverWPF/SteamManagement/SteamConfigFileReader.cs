using System;
using System.Collections.Generic;
using System.IO;
using SteamMoverWPF.Utility;

namespace SteamMoverWPF.SteamManagement
{
    internal class SteamConfigFileProperty
    {
        public string Name;
        public string Value;
    }

    internal class SteamConfigFile
    {
        public string ConfigType;
        public List<SteamConfigFileProperty> SteamConfigFilePropertyList = new List<SteamConfigFileProperty>();
    }

    internal static class SteamConfigFileReader
    {
        public static SteamConfigFile ReadLibraryList(string steamPath)
        {
            SteamConfigFile steamConfigFile = new SteamConfigFile();
            StreamReader streamReader = null;
            try
            {
                streamReader = new StreamReader(steamPath);
                // "LibraryFolders"
                steamConfigFile.ConfigType = StringOperations.GetSubstringByString('"', '"', streamReader.ReadLine());
                //{
                /*
                {
	                "TimeNextStatsReport"		"1473422083"
	                "ContentStatsID"		"-3627002011478449057"
	                "1"		"D:\\Games\\SteamLibrary"
                }
                */
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (!(line.IndexOf("}", StringComparison.Ordinal) == -1 && line.IndexOf("{", StringComparison.Ordinal) == -1))
                    {
                        continue;
                    }
                    line = line.Replace("\\\\", "\\");
                    SteamConfigFileProperty steamConfigFileProperty = new SteamConfigFileProperty();
                    steamConfigFileProperty.Name = StringOperations.GetSubstringByString('"', '"', line);
                    line = line.Substring(line.IndexOf('"') + 1);
                    line = line.Substring(line.IndexOf('"') + 1);
                    steamConfigFileProperty.Value = StringOperations.GetSubstringByString('"', '"', line);
                    steamConfigFile.SteamConfigFilePropertyList.Add(steamConfigFileProperty);
                }
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                ErrorHandler.Instance.ShowCriticalErrorMessage("File with list of libraries does not exist. Please run Steam atleast once.", ex);
            }
            finally
            {
                streamReader?.Close();
            }
            return steamConfigFile;
        }
        // ReSharper disable once InconsistentNaming
        public static SteamConfigFile ReadACF(string steamPath)
        {
            SteamConfigFile steamConfigFile = new SteamConfigFile();
            using (StreamReader streamReader = new StreamReader(steamPath))
            {
                // "AppState"
                steamConfigFile.ConfigType = StringOperations.GetSubstringByString('"', '"', streamReader.ReadLine());
                //{
                /*
                "appID"		"33900"
                "Universe"		"1"
                "name"		"Arma 2"
                "StateFlags"		"68"
                "installdir"		"Arma 2"
                "SizeOnDisk"		"4581412743"
                */
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (!(line.IndexOf("}", StringComparison.Ordinal) == -1 && line.IndexOf("{", StringComparison.Ordinal) == -1))
                    {
                        continue;
                    }
                    line = line.Replace("\\\\", "\\");
                    SteamConfigFileProperty steamConfigFileProperty = new SteamConfigFileProperty();
                    steamConfigFileProperty.Name = StringOperations.GetSubstringByString('"', '"', line);
                    line = line.Substring(line.IndexOf('"') + 1);
                    line = line.Substring(line.IndexOf('"') + 1);
                    steamConfigFileProperty.Value = StringOperations.GetSubstringByString('"', '"', line);
                    steamConfigFile.SteamConfigFilePropertyList.Add(steamConfigFileProperty);
                }
            }
            return steamConfigFile;
        }
    }
}
