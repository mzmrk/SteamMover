using System;
using System.Collections.Generic;
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
        public static SteamConfigFile ReadFile(string steamPath)
        {
            SteamConfigFile steamConfigFile = new SteamConfigFile();
            string line;
            System.IO.StreamReader streamReader = new System.IO.StreamReader(steamPath);
            // "LibraryFolders"
            steamConfigFile.ConfigType = UtilityBox.GetSubstringByString('"', '"', streamReader.ReadLine());
            //{
            /*
            "appID"		"33900"
	        "Universe"		"1"
	        "name"		"Arma 2"
	        "StateFlags"		"68"
	        "installdir"		"Arma 2"
	        "SizeOnDisk"		"4581412743"
            */
            while ((line = streamReader.ReadLine()) != null)
            {
                if (!(line.IndexOf("}", StringComparison.Ordinal) == -1 && line.IndexOf("{", StringComparison.Ordinal) == -1))
                {
                    continue;
                }
                line = line.Replace("\\\\", "\\");
                SteamConfigFileProperty steamConfigFileProperty = new SteamConfigFileProperty();
                steamConfigFileProperty.Name = UtilityBox.GetSubstringByString('"', '"', line);
                line = line.Substring(line.IndexOf('"') + 1);
                line = line.Substring(line.IndexOf('"') + 1);
                steamConfigFileProperty.Value = UtilityBox.GetSubstringByString('"', '"', line);
                steamConfigFile.SteamConfigFilePropertyList.Add(steamConfigFileProperty);
            }
            streamReader.Close();
            return steamConfigFile;
        }
    }
}
