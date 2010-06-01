using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan;
using System.Windows.Forms;
using System.IO;

namespace MegaMan_Level_Editor
{
    public class MapDocument
    {
        public MainForm parent;
        public Map Map { get; private set; }

        private Dictionary<string, StageForm> openScreens;
        private StageForm stageForm;

        private bool drawTiles;
        private bool drawGrid;
        private bool drawBlock;

        public bool DrawGrid
        {
            get { return drawGrid; }
            set
            {
                drawGrid = value;
                foreach (StageForm screen in this.openScreens.Values) screen.DrawGrid = value;
            }
        }

        public bool DrawTiles
        {
            get { return drawTiles; }
            set
            {
                drawTiles = value;
                foreach (StageForm screen in this.openScreens.Values) screen.DrawTiles = value;
            }
        }

        public bool DrawBlock
        {
            get { return drawBlock; }
            set
            {
                drawBlock = value;
                foreach (StageForm screen in this.openScreens.Values) screen.DrawBlock = value;
            }
        }

        public event Action<MapDocument> Closed;

        public MapDocument(Map map, MainForm parent)
        {
            this.parent = parent;
            this.Map = map;

            openScreens = new Dictionary<string, StageForm>();
        }

        // TODO : Rename Map to Stages.. More consistent naming
        public MapDocument(string path, MainForm parent)
        {
            this.parent = parent;
            this.Map = new Map(MainForm.Instance.rootPath, path);
            openScreens = new Dictionary<string, StageForm>();
        }

        public void RefreshInfo()
        {
            foreach (StageForm screen in this.openScreens.Values) screen.SetText();
        }

        public void ReFocus()
        {
            ShowStage(Map);
        }

        //TODO Write NewStage
        public void NewStage()
        {
            MessageBox.Show("I don't do anything yet! Fix this!");
        }

        public void NewScreen()
        {
            MegaMan.Screen screen = new MegaMan.Screen(16, 14, Map);
            ScreenProp propForm = new ScreenProp();
            //propForm.LoadScreen(screen);
            propForm.Show();

            propForm.FormClosing += (s, ev) =>
            {
                if (!propForm.Confirmed) return;

                if (screen.Name == null)
                {
                    MessageBox.Show("You must give the screen a name.", "Add Screen Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Map.Screens.Add(screen.Name, screen);

                //ShowStage(screen);

            };
        }

        public void ShowStage(Map stage)
        {
            if (this.stageForm == null)
            {
                this.stageForm = new StageForm(stage);
                stageForm.MdiParent = parent;
                stageForm.GotFocus += new EventHandler(StageForm_GotFocus);
                stageForm.FormClosing += new FormClosingEventHandler(StageForm_FormClosing);

                stageForm.DrawBlock = this.drawBlock;
                stageForm.DrawGrid = this.drawGrid;
                stageForm.DrawTiles = this.drawTiles;
            }

            stageForm.Show();
            stageForm.Focus();
        }

        public void Close()
        {
            if (Map.Dirty)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes to " + Map.Name + " before closing?", "Save Changes", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes) Map.Save();
                else if (result == DialogResult.Cancel) return;
            }

            CloseAll();
            if (Closed != null) Closed(this);
        }

        public bool ConfirmSave()
        {
            if (Map.Dirty)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes to " + Map.Name + " before closing?", "Save Changes", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes) Map.Save();
                else if (result == DialogResult.Cancel) return false;
            }
            return true;
        }

        public void Undo()
        {
            if (stageForm != null) stageForm.Undo();
        }

        public void Redo()
        {
            if (stageForm != null) stageForm.Redo();
        }

        private void CloseAll()
        {
            foreach (StageForm screen in this.openScreens.Values)
            {
                screen.FormClosing -= StageForm_FormClosing;
                CloseScreen(screen);
            }
        }

        void StageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // closing all screens means we should close completely
            if (openScreens.Count == 1) // this is the last one
            {
                e.Cancel = (!ConfirmSave());
                if (!e.Cancel) (sender as StageForm).Dispose();
                if (!e.Cancel && Closed != null) Closed(this);
            }
        }

        private void CloseScreen(StageForm StageForm)
        {
            StageForm.GotFocus -= new EventHandler(StageForm_GotFocus);
            StageForm.Close();
        }

        private void StageForm_GotFocus(object sender, EventArgs e)
        {
            parent.FocusScreen(this);
        }
    }
}
