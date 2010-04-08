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

        public void CloseAll()
        {
            foreach (ScreenForm screen in this.openScreens.Values) CloseScreen(screen);
        }

        public void NewScreen()
        {
            MegaMan.Screen screen = new MegaMan.Screen(16, 14, Map.Tileset);
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

                screenform.DrawBlock = this.drawBlock;
                screenform.DrawGrid = this.drawGrid;
                screenform.DrawTiles = this.drawTiles;

                openScreens[name] = screenform;
            }

            openScreens[name].Show();
            openScreens[name].Focus();
        }

        private void CloseScreen(ScreenForm screenform)
        {
            screenform.GotFocus -= new EventHandler(screenform_GotFocus);
            screenform.Close();

            screenform.Dispose();
        }

        private void screenform_GotFocus(object sender, EventArgs e)
        {
            parent.FocusScreen(this);
        }
    }
}
