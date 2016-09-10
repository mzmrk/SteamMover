using System.IO;
using System.Text;
using SteamMoverWPF.Entities;
using SteamMoverWPF.Utility;

namespace SteamMoverWPF.SteamManagement
{
    internal static class SteamConfigFileWriter
    {
        public static void WriteLibraryList()
        {
            StringBuilder writer = new StringBuilder();
            using (StreamReader streamReader = new StreamReader(BindingDataContext.Instance.SteamPath + "\\steamapps\\libraryfolders.vdf"))
            {
                writer.AppendLine(streamReader.ReadLine());
                writer.AppendLine(streamReader.ReadLine());
                writer.AppendLine(streamReader.ReadLine());
                writer.AppendLine(streamReader.ReadLine());
            }
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
            using (StreamWriter streamWriter = new StreamWriter(BindingDataContext.Instance.SteamPath + "\\steamapps\\libraryfolders.vdf"))
            {
                streamWriter.Write(writer);
            }
        }

        public static void WriteRealSizeOnDisk(string acfPath, double realSizeOnDisk)
        {
            StringBuilder writer = new StringBuilder();
            using (StreamReader streamReader = new StreamReader(acfPath))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    string propertyName = StringOperations.GetSubstringByString('"', '"', line);
                    if (propertyName != null && propertyName.Equals("SizeOnDisk"))
                    {
                        writer.AppendLine("\t\"SizeOnDisk\"\t\t\"" + realSizeOnDisk + "\"");
                    }
                    else
                    {
                        writer.AppendLine(line);
                    }

                }
            }
            using (StreamWriter streamWriter = new StreamWriter(acfPath))
            {
                streamWriter.Write(writer);
            }
        }
    }
}
