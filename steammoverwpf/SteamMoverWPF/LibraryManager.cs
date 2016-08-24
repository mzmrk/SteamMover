using SteamMoverWPF.Entities;

namespace SteamMoverWPF
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
            string src = source.LibraryDirectory + "\\common\\" + gameFolder;
            string dst = destination.LibraryDirectory + "\\common\\" + gameFolder;
            return moveFolder(src, dst);
        }

        public bool moveACF(Library source, Library destination, int appID)
        {
            string sourceFile = source.LibraryDirectory + "\\" + "appmanifest_" + appID + ".acf";
            string destinationFile = destination.LibraryDirectory + "\\" + "appmanifest_" + appID+ ".acf";
            return moveFolder(sourceFile, destinationFile);
        }
    }
}
