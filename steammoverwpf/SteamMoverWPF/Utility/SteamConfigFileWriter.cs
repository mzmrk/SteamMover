﻿using SteamMoverWPF.Entities;
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
        public static void writeRealSizeOnDisk(string acfPath, double realSizeOnDisk)
        {
            StringBuilder writer = new StringBuilder();
            StreamReader streamReader = new StreamReader(acfPath);

            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                string propertyName = UtilityBox.GetSubstringByString('"', '"', line);
                if (propertyName != null && propertyName.Equals("SizeOnDisk"))
                {
                    writer.AppendLine("\t\"SizeOnDisk\"\t\t\"" + realSizeOnDisk + "\"");
                } else
                {
                    writer.AppendLine(line);
                }
                
            }
            streamReader.Close();

            StreamWriter streamWriter = new StreamWriter(acfPath);
            streamWriter.Write(writer);
            streamWriter.Close();
        }
    }
}
