using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamMoverGui
{
    class ValveFileItem
    {
        public string name;
        public string value;
    }
    class ValveFileReader
    {
        public string GetSubstringByString(char a, char b, string c)
        {
            if (c == "") { return ""; }
            int start = c.IndexOf(a);
            string subString = c.Substring(start+1);
            int end = subString.IndexOf(b);
            string returnString = c.Substring(start + 1, end);
            return returnString;
        }
        public string name;
        public List<ValveFileItem> itemsList = new List<ValveFileItem>();
        public void readFile(string completePath)
        {
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(completePath);
            // "LibraryFolders"
            name = GetSubstringByString('"', '"', file.ReadLine());
            //{
            file.ReadLine();
            /*
            "appID"		"33900"
	        "Universe"		"1"
	        "name"		"Arma 2"
	        "StateFlags"		"68"
	        "installdir"		"Arma 2"
	        "SizeOnDisk"		"4581412743"
            */
            while ((line = file.ReadLine()) != null)
            {
                if (!(line.IndexOf("}") == -1 && line.IndexOf("{") == -1))
                {
                    continue;
                }
                line = line.Replace("\\\\", "\\");
                ValveFileItem vfi = new ValveFileItem();
                vfi.name = GetSubstringByString('"', '"', line);
                line = line.Substring(line.IndexOf('"') + 1);
                line = line.Substring(line.IndexOf('"') + 1);
                vfi.value = GetSubstringByString('"', '"', line);
                itemsList.Add(vfi);
            }

            file.Close();
        }
    }
}
