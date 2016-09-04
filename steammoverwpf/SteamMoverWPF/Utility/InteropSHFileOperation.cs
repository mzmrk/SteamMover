using System;
using System.Runtime.InteropServices;

namespace SteamMoverWPF.Utility
{
    // SOURCE: http://www.pinvoke.net/default.aspx/shell32.shfileoperation
    public class InteropShFileOperation
    {
        public enum FoFunc : uint
        {
            FoMove = 0x0001,
            FoCopy = 0x0002,
            FoDelete = 0x0003,
            FoRename = 0x0004,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
        private struct Shfileopstruct
        {
            public IntPtr hwnd;
            public FoFunc wFunc;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pFrom;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pTo;
            public ushort fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;

        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHFileOperation([In, Out] ref Shfileopstruct lpFileOp);

        private Shfileopstruct _shFile;
        public FileopFlags FFlags;

        public IntPtr Hwnd
        {
            set
            {
                this._shFile.hwnd = value;
            }
        }
        public FoFunc WFunc
        {
            set
            {
                this._shFile.wFunc = value;
            }
        }

        public string PFrom
        {
            set
            {
                this._shFile.pFrom = value + '\0' + '\0';
            }
        }
        public string PTo
        {
            set
            {
                this._shFile.pTo = value + '\0' + '\0';
            }
        }

        public bool FAnyOperationsAborted
        {
            set
            {
                this._shFile.fAnyOperationsAborted = value;
            }
        }
        public IntPtr HNameMappings
        {
            set
            {
                this._shFile.hNameMappings = value;
            }
        }
        public string LpszProgressTitle
        {
            set
            {
                this._shFile.lpszProgressTitle = value + '\0';
            }
        }

        public InteropShFileOperation()
        {

            this.FFlags = new FileopFlags();
            this._shFile = new Shfileopstruct();
            this._shFile.hwnd = IntPtr.Zero;
            this._shFile.wFunc = FoFunc.FoCopy;
            this._shFile.pFrom = "";
            this._shFile.pTo = "";
            this._shFile.fAnyOperationsAborted = false;
            this._shFile.hNameMappings = IntPtr.Zero;
            this._shFile.lpszProgressTitle = "";

        }

        public bool Execute()
        {
            this._shFile.fFlags = this.FFlags.Flag;
            return SHFileOperation(ref this._shFile) == 0;//true if no errors
        }

        public class FileopFlags
        {
            [Flags]
            private enum FileopFlagsEnum : ushort
            {
                FofMultidestfiles = 0x0001,
                FofConfirmmouse = 0x0002,
                FofSilent = 0x0004,  // don't create progress/report
                FofRenameoncollision = 0x0008,
                FofNoconfirmation = 0x0010,  // Don't prompt the user.
                FofWantmappinghandle = 0x0020,  // Fill in SHFILEOPSTRUCT.hNameMappings
                // Must be freed using SHFreeNameMappings
                FofAllowundo = 0x0040,
                FofFilesonly = 0x0080,  // on *.*, do only files
                FofSimpleprogress = 0x0100,  // means don't show names of files
                FofNoconfirmmkdir = 0x0200,  // don't confirm making any needed dirs
                FofNoerrorui = 0x0400,  // don't put up error UI
                FofNocopysecurityattribs = 0x0800,  // dont copy NT file Security Attributes
                FofNorecursion = 0x1000,  // don't recurse into directories.
                FofNoConnectedElements = 0x2000,  // don't operate on connected elements.
                FofWantnukewarning = 0x4000,  // during delete operation, warn if nuking instead of recycling (partially overrides FOF_NOCONFIRMATION)
                FofNorecursereparse = 0x8000,  // treat reparse points as objects, not containers
            }

            public bool FofMultidestfiles = false;
            public bool FofConfirmmouse = false;
            public bool FofSilent = false;
            public bool FofRenameoncollision = false;
            public bool FofNoconfirmation = false;
            public bool FofWantmappinghandle = false;
            public bool FofAllowundo = false;
            public bool FofFilesonly = false;
            public bool FofSimpleprogress = false;
            public bool FofNoconfirmmkdir = false;
            public bool FofNoerrorui = false;
            public bool FofNocopysecurityattribs = false;
            public bool FofNorecursion = false;
            public bool FofNoConnectedElements = false;
            public bool FofWantnukewarning = false;
            public bool FofNorecursereparse = false;

            public ushort Flag
            {
                get
                {
                    ushort returnValue = 0;

                    if (this.FofMultidestfiles)
                        returnValue |= (ushort)FileopFlagsEnum.FofMultidestfiles;
                    if (this.FofConfirmmouse)
                        returnValue |= (ushort)FileopFlagsEnum.FofConfirmmouse;
                    if (this.FofSilent)
                        returnValue |= (ushort)FileopFlagsEnum.FofSilent;
                    if (this.FofRenameoncollision)
                        returnValue |= (ushort)FileopFlagsEnum.FofRenameoncollision;
                    if (this.FofNoconfirmation)
                        returnValue |= (ushort)FileopFlagsEnum.FofNoconfirmation;
                    if (this.FofWantmappinghandle)
                        returnValue |= (ushort)FileopFlagsEnum.FofWantmappinghandle;
                    if (this.FofAllowundo)
                        returnValue |= (ushort)FileopFlagsEnum.FofAllowundo;
                    if (this.FofFilesonly)
                        returnValue |= (ushort)FileopFlagsEnum.FofFilesonly;
                    if (this.FofSimpleprogress)
                        returnValue |= (ushort)FileopFlagsEnum.FofSimpleprogress;
                    if (this.FofNoconfirmmkdir)
                        returnValue |= (ushort)FileopFlagsEnum.FofNoconfirmmkdir;
                    if (this.FofNoerrorui)
                        returnValue |= (ushort)FileopFlagsEnum.FofNoerrorui;
                    if (this.FofNocopysecurityattribs)
                        returnValue |= (ushort)FileopFlagsEnum.FofNocopysecurityattribs;
                    if (this.FofNorecursion)
                        returnValue |= (ushort)FileopFlagsEnum.FofNorecursion;
                    if (this.FofNoConnectedElements)
                        returnValue |= (ushort)FileopFlagsEnum.FofNoConnectedElements;
                    if (this.FofWantnukewarning)
                        returnValue |= (ushort)FileopFlagsEnum.FofWantnukewarning;
                    if (this.FofNorecursereparse)
                        returnValue |= (ushort)FileopFlagsEnum.FofNorecursereparse;

                    return returnValue;
                }
            }
        }

    }
}
