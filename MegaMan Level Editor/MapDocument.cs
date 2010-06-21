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

        private StageForm stageForm;

        private bool drawTiles;
        private bool drawGrid;
        private bool drawBlock;
        private bool drawJoins;

        public bool DrawGrid
        {
            get { return drawGrid; }
            set
            {
                drawGrid = value;
                if (stageForm != null) stageForm.DrawGrid = value;
            }
        }

        public bool DrawTiles
        {
            get { return drawTiles; }
            set
            {
                drawTiles = value;
                if (stageForm != null) stageForm.DrawTiles = value;
            }
        }

        public bool DrawBlock
        {
            get { return drawBlock; }
            set
            {
                drawBlock = value;
                if (stageForm != null) stageForm.DrawBlock = value;
            }
        }

        public event Action<MapDocument> Closed;

        public MapDocument(Map map, MainForm parent)
        {
            this.parent = parent;
            this.Map = map;
        }

        // TODO : Rename Map to Stages.. More consistent naming
        public MapDocument(string path, MainForm parent)
        {
            this.parent = parent;
            this.Map = new Map(MainForm.Instance.rootPath, path);
        }

        public void RefreshInfo()
        {
            stageForm.SetText();
        }

        public void ReFocus()
        {
            ShowStage();
        }

        //TODO Write NewStage
        public void NewStage()
        {
            MessageBox.Show("I don't do anything yet! Fix this!");
        }

        public void ShowStage()
        {
            if (this.stageForm == null)
            {
                this.stageForm = new StageForm(this.Map);
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
            if (!ConfirmSave()) return;

            stageForm.FormClosing -= StageForm_FormClosing;
            stageForm.Close();
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

        void StageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (!ConfirmSave()) return;

            stageForm.Hide();
            if (Closed != null) Closed(this);
        }

        private void StageForm_GotFocus(object sender, EventArgs e)
        {
            parent.FocusScreen(this);
        }
    }
}
