using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan;
using System.Windows.Forms;

namespace MegaMan_Level_Editor
{
    public class MapDocument
    {
        private MainForm parent;
        public Map Map { get; private set; }

        private Dictionary<string, ScreenForm> openScreens;

        private bool drawTiles;
        private bool drawGrid;
        private bool drawBlock;

        public bool DrawGrid
        {
            get { return drawGrid; }
            set
            {
                drawGrid = value;
                foreach (ScreenForm screen in this.openScreens.Values) screen.DrawGrid = value;
            }
        }

        public bool DrawTiles
        {
            get { return drawTiles; }
            set
            {
                drawTiles = value;
                foreach (ScreenForm screen in this.openScreens.Values) screen.DrawTiles = value;
            }
        }

        public bool DrawBlock
        {
            get { return drawBlock; }
            set
            {
                drawBlock = value;
                foreach (ScreenForm screen in this.openScreens.Values) screen.DrawBlock = value;
            }
        }

        public event Action<MapDocument> Closed;

        public MapDocument(Map map, MainForm parent)
        {
            this.parent = parent;
            this.Map = map;
        }

        public MapDocument(string path, MainForm parent)
        {
            this.parent = parent;

            this.Map = new Map(path);

            openScreens = new Dictionary<string, ScreenForm>();
        }

        public void ReFocus()
        {
            ShowScreen(Map.Screens.First().Key);
        }

        public void NewScreen()
        {
            MegaMan.Screen screen = new MegaMan.Screen(16, 14, Map);
            ScreenProp propForm = new ScreenProp();
            propForm.LoadScreen(screen);
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

                ShowScreen(screen.Name);
            };
        }

        public void ShowScreen(string name)
        {
            if (!openScreens.ContainsKey(name))
            {
                ScreenForm screenform = new ScreenForm();
                screenform.MdiParent = parent;
                screenform.SetScreen(Map.Screens[name]);
                screenform.GotFocus += new EventHandler(screenform_GotFocus);
                screenform.FormClosing += new FormClosingEventHandler(screenform_FormClosing);

                screenform.DrawBlock = this.drawBlock;
                screenform.DrawGrid = this.drawGrid;
                screenform.DrawTiles = this.drawTiles;

                openScreens[name] = screenform;
            }

            openScreens[name].Show();
            openScreens[name].Focus();
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

        private void CloseAll()
        {
            foreach (ScreenForm screen in this.openScreens.Values)
            {
                screen.FormClosing -= screenform_FormClosing;
                CloseScreen(screen);
            }
        }

        void screenform_FormClosing(object sender, FormClosingEventArgs e)
        {
            // closing all screens means we should close completely
            if (openScreens.Count == 1) // this is the last one
            {
                e.Cancel = (!ConfirmSave());
                if (!e.Cancel && Closed != null) Closed(this);
            }
        }

        private void CloseScreen(ScreenForm screenform)
        {
            screenform.GotFocus -= new EventHandler(screenform_GotFocus);
            screenform.Close();
        }

        private void screenform_GotFocus(object sender, EventArgs e)
        {
            parent.FocusScreen(this);
        }
    }
}
