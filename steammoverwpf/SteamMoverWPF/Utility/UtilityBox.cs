using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace SteamMoverWPF.Utility
{
    internal static class UtilityBox
    {
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
        public static double GetWshFolderSize(string fldr)
        {
            //Reference "Windows Script Host Object Model" on the COM tab.
            IWshRuntimeLibrary.FileSystemObject fso = new IWshRuntimeLibrary.FileSystemObject();
            double size = (double)fso.GetFolder(fldr).Size;
            Marshal.FinalReleaseComObject(fso);
            return size;
        }
        public static bool IsSteamRunning()
        {
            return Process.GetProcessesByName("Steam").Length > 0;
        }
    }
}
