using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SteamMoverGui
{
    class LibraryManager
    {
        // Upewnić się czy kopiowanie się zakończyło sukcesem, 2 sposoby:
        // 1. Skopiować i usunąć
        // 2. Przenieść i wycofać zmiany w razie błędu.

        // +++ Dopieścić szczegóły na temat parametrów kopiowania/przenoszenia, troche tam można poustawiać :D
        bool moveFolder(string source, string destination)
        {
            InteropSHFileOperation interopSHFileOperation = new InteropSHFileOperation();
            interopSHFileOperation.wFunc = InteropSHFileOperation.FO_Func.FO_MOVE;
            interopSHFileOperation.pFrom = source;
            interopSHFileOperation.pTo = destination;
            interopSHFileOperation.fFlags.FOF_NOCONFIRMMKDIR = true;
            return interopSHFileOperation.Execute();
        }
        public bool moveGame(Library source, Library destination, string gameFolder) {
            string src = source.libraryDirectory + "\\common\\" + gameFolder;
            string dst = destination.libraryDirectory + "\\common\\" + gameFolder;
            return moveFolder(src, dst);
        }

        public void moveACF(Library source, Library destination, int appID)
        {
            string sourceFile = source.libraryDirectory + "\\" + "appmanifest_" + appID + ".acf";
            string destinationFile = destination.libraryDirectory + "\\" + "appmanifest_" + appID+ ".acf";
            System.IO.File.Move(sourceFile, destinationFile);
        }
    }
}
