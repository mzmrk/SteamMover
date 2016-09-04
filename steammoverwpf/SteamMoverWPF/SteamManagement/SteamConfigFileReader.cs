using SteamMoverWPF.Utility;
using System.Collections.Generic;

namespace SteamMoverWPF
{
    class SteamConfigFileProperty
    {
        public string Name;
        public string Value;
    }
    class SteamConfigFile
    {
        public string configType;
        public List<SteamConfigFileProperty> steamConfigFilePropertyList = new List<SteamConfigFileProperty>();
    }
    static class SteamConfigFileReader
    {
        public static SteamConfigFile readFile(string steamPath)
        {
            SteamConfigFile steamConfigFile = new SteamConfigFile();
            string line;
            System.IO.StreamReader streamReader = new System.IO.StreamReader(steamPath);
            // "LibraryFolders"
            steamConfigFile.configType = UtilityBox.GetSubstringByString('"', '"', streamReader.ReadLine());
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
                if (!(line.IndexOf("}") == -1 && line.IndexOf("{") == -1))
                {
                    continue;
                }
                line = line.Replace("\\\\", "\\");
                SteamConfigFileProperty steamConfigFileProperty = new SteamConfigFileProperty();
                steamConfigFileProperty.Name = UtilityBox.GetSubstringByString('"', '"', line);
                line = line.Substring(line.IndexOf('"') + 1);
                line = line.Substring(line.IndexOf('"') + 1);
                steamConfigFileProperty.Value = UtilityBox.GetSubstringByString('"', '"', line);
                steamConfigFile.steamConfigFilePropertyList.Add(steamConfigFileProperty);
            }
            streamReader.Close();
            return steamConfigFile;
        }
    }
}
