using SteamMoverWPF.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamMoverWPF.Utility
{
    static class SteamConfigFileWriter
    {
        public static void writeLibraryList()
        {
            StringBuilder writer = new StringBuilder();
            StreamReader streamReader = new StreamReader(BindingDataContext.Instance.SteamPath + "\\steamapps\\libraryfolders.vdf");

            writer.AppendLine(streamReader.ReadLine());
            writer.AppendLine(streamReader.ReadLine());
            writer.AppendLine(streamReader.ReadLine());
            writer.AppendLine(streamReader.ReadLine());

            streamReader.Close();

            int i = 0;
            foreach (Library library in BindingDataContext.Instance.LibraryList)
            {
                if (i != 0)
                {
                    string libraryDirectory = library.LibraryDirectory;
                    libraryDirectory = libraryDirectory.Replace("\\", "\\\\");
                    writer.AppendLine("\t\"" + i + "\"\t\t\"" + libraryDirectory + "\"");
                }
                i++;
            }
            writer.AppendLine("}");
            StreamWriter streamWriter = new StreamWriter(BindingDataContext.Instance.SteamPath + "\\steamapps\\libraryfolders.vdf");
            streamWriter.Write(writer);
            streamWriter.Close();
        }
        public static void writeRealSizeOnDisk()
        {
            throw new NotImplementedException();
        }
    }
}
