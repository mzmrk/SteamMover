using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace SteamMoverWPF.Utility
{
    //source: https://social.msdn.microsoft.com/Forums/en-US/b7db7ec7-34a5-4ca6-89e7-947190c4e043/get-free-space-on-network-share?forum=csharpgeneral
    static class GetDiskFreeSpace
    {
        public static long FreeSpace(string folderName)
        {
            if (!folderName.EndsWith("\\"))
            {
                folderName += '\\';
            }
            long free = 0;
            long dummy1 = 0;
            long dummy2 = 0;
            if (GetDiskFreeSpaceEx(folderName, ref free, ref dummy1, ref dummy2))
            {
                return free;
            }
            return -1;
        }
        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("Kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool GetDiskFreeSpaceEx
        (
            string lpszPath,                    // Must name a folder, must end with '\'.
            ref long lpFreeBytesAvailable,
            ref long lpTotalNumberOfBytes,
            ref long lpTotalNumberOfFreeBytes
        );
    }
}
