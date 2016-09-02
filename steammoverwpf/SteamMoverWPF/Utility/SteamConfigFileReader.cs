using SteamMoverWPF.Utility;
using System.Collections.Generic;

namespace SteamMoverWPF
{
    class SteamConfigFileProperty
    {
        public string name;
        public string value;
    }
    class SteamConfigFileReader
    {
        public string configType;
        public List<SteamConfigFileProperty> steamConfigFilePropertyList = new List<SteamConfigFileProperty>();

        public void readSteamConfigFile(string steamPath)
        {
            string line;
            System.IO.StreamReader streamReader = new System.IO.StreamReader(steamPath);
            // "LibraryFolders"
            configType = UtilityBox.GetSubstringByString('"', '"', streamReader.ReadLine());
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
                steamConfigFileProperty.name = UtilityBox.GetSubstringByString('"', '"', line);
                line = line.Substring(line.IndexOf('"') + 1);
                line = line.Substring(line.IndexOf('"') + 1);
                steamConfigFileProperty.value = UtilityBox.GetSubstringByString('"', '"', line);
                steamConfigFilePropertyList.Add(steamConfigFileProperty);
            }

            streamReader.Close();
        }
    }
}
