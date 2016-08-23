using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteamMoverGui
{
    public partial class SteamMover : Form
    {
        volatile LibraryDetector libraryDetector = new LibraryDetector();
        LibraryManager libraryManager = new LibraryManager();
        Thread threadRealSizeOnDisk;

        int comboBoxLeftSelectedIndex = 0;
        int comboBoxRightSelectedIndex = 0;
        public void reset() {
            libraryDetector.run();
            comboBoxLeft.Items.Clear();
            comboBoxRight.Items.Clear();
            foreach (Library library in libraryDetector.libraryList)
            {
                comboBoxLeft.Items.Add(library.libraryDirectory);
                comboBoxRight.Items.Add(library.libraryDirectory);
            }
            comboBoxLeft.SelectedIndex = 0;
            comboBoxRight.SelectedIndex = 0;
        }
        public void WorkThreadHelperReloadList()
        {
            // do we need to switch threads?
            if (InvokeRequired)
            {

                // slightly different now, as we dont need params
                // we can just use MethodInvoker
                MethodInvoker method = new MethodInvoker(WorkThreadHelperReloadList);
                Invoke(method);
                return;
            }
            //dataGridViewLeft.Rows[0].Cells[1].Value
            List<Game> gamesList;
            gamesList = libraryDetector.libraryList[comboBoxLeftSelectedIndex].gamesList;
            for (int i = 0; i < gamesList.Count; i++)
            {
                Game game = gamesList[i];
                dataGridViewLeft[1, i].Value = ((int)(game.realSizeOnDisk / 1024 / 1024)).ToString() + " MB";
            }
            gamesList = libraryDetector.libraryList[comboBoxRightSelectedIndex].gamesList;
            for (int i = 0; i < gamesList.Count; i++)
            {
                Game game = gamesList[i];
                string sizeOnDisk = ((int)(game.realSizeOnDisk / 1024 / 1024)).ToString();
                dataGridViewRight[1, i].Value = sizeOnDisk + " MB";
            }
        }
        public void WorkThreadRealSizeOnDisk()
        {
            try
            {
                foreach (Library library in libraryDetector.libraryList)
                {
                    foreach (Game game in library.gamesList)
                    {
                        libraryDetector.detectRealSizeOnDisk(library, game);
                        if (game.realSizeOnDisk == -1)
                        {
                            // usuń z listy, oznacz jako nieaktywny, coś jest nie tak z tym folderem.
                        }
                        WorkThreadHelperReloadList();
                    }
                }
            }
            catch (Exception ex)
            {
                // log errors
            }
        }
        public SteamMover()
        {
            InitializeComponent();
            reset();
            threadRealSizeOnDisk = new Thread(new ThreadStart(WorkThreadRealSizeOnDisk));
            threadRealSizeOnDisk.Start();
        }

        private void comboBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxLeftSelectedIndex = comboBoxLeft.SelectedIndex;
            dataGridViewLeft.Rows.Clear();
            foreach (Game game in libraryDetector.libraryList[comboBoxLeft.SelectedIndex].gamesList)
            {
                string[] row = { (string)game.gameName, ((int)(game.realSizeOnDisk / 1024 / 1024)).ToString() + " MB" };
                dataGridViewLeft.Rows.Add(row);
                
            }
        }
        //private delegate void ObjectDelegate(object sender, EventArgs e);   
        private void comboBoxRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*if (InvokeRequired)
            {
                // we then create the delegate again
                // if you've made it global then you won't need to do this
                ObjectDelegate method = new ObjectDelegate(comboBoxRight_SelectedIndexChanged);
  
                // we then simply invoke it and return
                this.Invoke(method, sender, e);
                return;
            }*/
            comboBoxRightSelectedIndex = comboBoxRight.SelectedIndex;
            dataGridViewRight.Rows.Clear();
            foreach (Game game in libraryDetector.libraryList[comboBoxRight.SelectedIndex].gamesList)
            {
                string[] row = { (string)game.gameName, ((int)(game.realSizeOnDisk / 1024 / 1024)).ToString() + " MB" };
                dataGridViewRight.Rows.Add(row);
            }
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            Library source = libraryDetector.libraryList[comboBoxRight.SelectedIndex];
            Library destination = libraryDetector.libraryList[comboBoxLeft.SelectedIndex];
            string gameFolder = libraryDetector.libraryList[comboBoxRight.SelectedIndex].gamesList[dataGridViewRight.CurrentCell.RowIndex].gameDirectory;
            int appID = libraryDetector.libraryList[comboBoxRight.SelectedIndex].gamesList[dataGridViewRight.CurrentCell.RowIndex].appID;

            libraryManager.moveGame(source, destination, gameFolder);
            libraryManager.moveACF(source, destination, appID);
            reset();
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {
            Library source = libraryDetector.libraryList[comboBoxLeft.SelectedIndex];
            Library destination = libraryDetector.libraryList[comboBoxRight.SelectedIndex];
            string gameFolder = libraryDetector.libraryList[comboBoxLeft.SelectedIndex].gamesList[dataGridViewLeft.CurrentCell.RowIndex].gameDirectory;
            int appID = libraryDetector.libraryList[comboBoxLeft.SelectedIndex].gamesList[dataGridViewLeft.CurrentCell.RowIndex].appID;

            libraryManager.moveGame(source, destination, gameFolder);
            libraryManager.moveACF(source, destination, appID);
            reset();
        }
    }
}
