using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SteamMoverWPF.Utility
{
    internal static class UtilityBox
    {
        public static long GetWshFolderSize(string fldr)
        {
            IWshRuntimeLibrary.FileSystemObject fso = new IWshRuntimeLibrary.FileSystemObject();
            long size = (long)fso.GetFolder(fldr).Size;
            Marshal.FinalReleaseComObject(fso);
            return size;
        }
        public static bool IsSteamRunning()
        {
            return Process.GetProcessesByName("Steam").Length > 0;
        }
    }
}
