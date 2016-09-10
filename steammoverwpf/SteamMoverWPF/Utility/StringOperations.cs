using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamMoverWPF.Utility
{
    class StringOperations
    {
        public static string removeStringAtEnd(string s, string stringEnding)
        {
            return s.Substring(0, s.Length - stringEnding.Length);
        }
        public static string GetSubstringByString(char a, char b, string c)
        {
            if (c == "") return "";
            int start = c.IndexOf(a);
            if (start == -1) return null;
            string subString = c.Substring(start + 1);
            int end = subString.IndexOf(b);
            if (end == -1) return null;
            string returnString = c.Substring(start + 1, end);
            return returnString;
        }

        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }


        private static string RenameWhenPathExistsCommon(string fullPath, string fileNameOnly, string extension)
        {
            int count = 1;

            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;
            if (fileNameOnly.EndsWith(")"))
            {
                int start = fileNameOnly.LastIndexOf("(");
                int end = fileNameOnly.Length - 1;
                string number = fileNameOnly.Substring(start + 1, end - start - 1);
                try
                {
                    count = int.Parse(number);
                    fileNameOnly = fileNameOnly.Substring(0, start);
                }
                catch (FormatException ex)
                {
                    ErrorHandler.Instance.Log("Format exception when trying to parse number inside ()", ex);
                }
            }
            while (Directory.Exists(newFullPath) || File.Exists(newFullPath))
            {
                string tempFileName = $"{fileNameOnly}({count++})";
                newFullPath = Path.Combine(path, tempFileName + extension);
            }
            return newFullPath;
        }

        public static string RenamePathWhenExists(string fullPath)
        {

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            return RenameWhenPathExistsCommon(fullPath, fileNameOnly, extension);
        }

        public static string RenamePathWhenExists(string fullPath, string extraWordAtEnd)
        {
            if (!fullPath.EndsWith(extraWordAtEnd))
            {
                return RenamePathWhenExists(fullPath);
            }
            string fileNameOnly = removeStringAtEnd(Path.GetFileNameWithoutExtension(fullPath),extraWordAtEnd);
            string extension = extraWordAtEnd + Path.GetExtension(fullPath);
            return RenameWhenPathExistsCommon(fullPath, fileNameOnly, extension);
        }
    }
}
