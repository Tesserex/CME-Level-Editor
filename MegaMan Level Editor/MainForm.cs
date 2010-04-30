﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ScreenDict = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, MegaMan_Level_Editor.ScreenForm>>;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public partial class MainForm : Form
    {
        #region Private Members
        private BrowseForm browseForm;
        private MapDocument activeMap;
        private List<MapDocument> openMaps;
        private BrushForm brushForm;

        private bool drawGrid;
        private bool drawTiles;
        private bool drawBlock;

        private string recentPath = Application.StartupPath + "\\recent.ini";
        private List<string> recentFiles = new List<string>(10);
        private int untitledCount = 0;

        TilesetStrip tilestrip;
        #endregion Private Members

        #region Properties
        private MapDocument ActiveMap
        {
            get { return activeMap; }
            set
            {
                activeMap = value;
                saveToolStripMenuItem.Enabled = saveAsToolStripMenuItem.Enabled =
                    closeToolStripMenuItem.Enabled = propertiesToolStripMenuItem.Enabled =
                    addScreenToolStripMenuItem1.Enabled = browseToolStripMenuItem.Enabled =
                    manageEnemiesToolStripMenuItem.Enabled = mergeScreenToolStripMenuItem.Enabled =
                    splitScreenToolStripMenuItem.Enabled = addEnemyToolStripMenuItem.Enabled = (value != null);
            }
        }

        public bool DrawGrid
        {
            get { return drawGrid; }
            set
            {
                drawGrid = value;
                showGridToolStripMenuItem.Checked = value;

                foreach (MapDocument map in openMaps) map.DrawGrid = value;
            }
        }

        public bool DrawTiles
        {
            get { return drawTiles; }
            set
            {
                drawTiles = value;
                showBackgroundsToolStripMenuItem.Checked = value;

                foreach (MapDocument map in openMaps) map.DrawTiles = value;
            }
        }

        public bool DrawBlock
        {
            get { return drawBlock; }
            set
            {
                drawBlock = value;
                showBlockingToolStripMenuItem.Checked = value;

                foreach (MapDocument map in openMaps) map.DrawBlock = value;

                //tileForm.DrawBlock = value;
            }
        }
        #endregion

        public event BrushChangedHandler BrushChanged;

        public static MainForm Instance { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            tilestrip = new TilesetStrip();
            this.Controls.Add(tilestrip);
            tilestrip.BringToFront();
            tilestrip.TileChanged += TileChanged;

            Instance = this;

            openMaps = new List<MapDocument>();

            brushForm = new BrushForm();
            brushForm.Show();
            brushForm.BrushChanged += new BrushChangedHandler(brushForm_BrushChanged);
            brushForm.Shown += (s, e) => brushesToolStripMenuItem.Checked = true;
            brushForm.FormClosing += (s, e) => { e.Cancel = true; brushForm.Hide(); brushesToolStripMenuItem.Checked = false; };
            brushForm.Anchor = AnchorStyles.Left;
            brushForm.Owner = this;

            DrawGrid = false;
            DrawTiles = true;
            DrawBlock = false;

            LoadRecentFiles();
        }

        public void FocusScreen(MapDocument map)
        {
            activeMap = map;
            ChangeTileset(map.Map.Tileset);
        }

        private void ChangeTileset(Tileset tileset)
        {
            if (brushForm != null) brushForm.ChangeTileset(activeMap.Map.Tileset);

            tilestrip.ChangeTileset(tileset);
        }

        private void brushForm_BrushChanged(BrushChangedEventArgs e)
        {
            if (BrushChanged != null) BrushChanged(e);
        }

        private void TileChanged(Tile tile)
        {
            ITileBrush brush = new SingleTileBrush(tile);
            BrushChangedEventArgs args = new BrushChangedEventArgs(brush);
            if (BrushChanged != null) BrushChanged(args);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!CheckSaveOnClose())
            {
                e.Cancel = true;
                return;
            }
            File.WriteAllLines(recentPath, recentFiles.ToArray());
            e.Cancel = false;
            base.OnClosing(e);
        }

        #region Private Methods
        private void AddRecentFile(string path)
        {
            if (recentFiles.Contains(path)) // move to top
            {
                recentFiles.Remove(path);
            }
            recentFiles.Add(path);
            if (recentFiles.Count > 10) recentFiles.RemoveRange(0, recentFiles.Count - 10);
        }

        private void LoadRecentFiles()
        {
            try
            {
                string[] recent = File.ReadAllLines(recentPath);
                int i = 0;
                foreach (string path in recent)
                {
                    recentFiles.Add(path);
                    ToolStripMenuItem r = new ToolStripMenuItem(path);
                    r.Click += new EventHandler(RecentMenu_Click);
                    Keys key = (Keys)Enum.Parse(typeof(Keys), ("D" + i.ToString()));
                    r.ShortcutKeys = Keys.Control | key;
                    recentMenuItem.DropDownItems.Insert(0, r);
                    i++;
                    if (i >= 10) break;
                }
                if (recentFiles.Count > 0)
                {
                    string path = recentFiles[0].Remove(recentFiles[0].LastIndexOf('\\'));
                    folderDialog.SelectedPath = path;
                }
            }
            catch (FileNotFoundException)
            {
                File.Create(recentPath);
            }
        }

        private void OpenMap(string path)
        {
            foreach (MapDocument mapdoc in openMaps)
            {
                if (Path.GetFullPath(mapdoc.Map.FileDir) == Path.GetFullPath(path))
                {
                    // only focus it if it's not already focused
                    if (ActiveMap.Map != mapdoc.Map) mapdoc.ReFocus();
                    return;
                }
            }

            MapDocument map = new MapDocument(path, this);

            map.DrawBlock = this.drawBlock;
            map.DrawGrid = this.drawGrid;
            map.DrawTiles = this.drawTiles;

            map.Closed += new Action<MapDocument>(map_Closed);
            openMaps.Add(map);
            map.ReFocus();

            AddRecentFile(path);            
        }

        private void map_Closed(MapDocument mapdoc)
        {
            openMaps.Remove(mapdoc);

            // if the tile form is showing this map's tileset, remove it from the form
            if (ActiveMap == mapdoc)
            {
                tilestrip.ChangeTileset(null);
            }
        }
        #endregion Private Methods
        
        #region Menu Handlers
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = folderDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = folderDialog.SelectedPath;

                OpenMap(path);
            }
        }

        private bool CheckSaveOnClose()
        {
            foreach (MapDocument map in openMaps)
            {
                if (!map.ConfirmSave()) return false;
            }
            return true;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawGrid = !DrawGrid;
        }

        private void RecentMenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = sender as ToolStripMenuItem;
            OpenMap(t.Text);
        }

        private void showBackgroundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawTiles = !DrawTiles;
        }

        private void showBlockingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawBlock = !DrawBlock;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMap != null && ActiveMap.Map.Loaded)
            {
                if (ActiveMap.Map.FileDir != null) ActiveMap.Map.Save();
                else SaveAs();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMap == null) return;

            ActiveMap.Close();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Map map = new Map();
            untitledCount++;
            map.Name = "Untitled" + untitledCount.ToString();

            LevelProp propForm = new LevelProp();
            propForm.LoadMap(map);
            propForm.Text = "New Level Properties";
            propForm.Show();

            propForm.OkPressed += () => {
                MapDocument document = new MapDocument(map, this);
                document.NewScreen();
                openMaps.Add(document);
            };
        }

        private void browseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (browseForm == null)
            {
                browseForm = new BrowseForm();
                browseForm.FormClosed += (s, ev) => { browseForm = null; };
            }
            browseForm.SetMap(ActiveMap); // forces a refresh of screens
            browseForm.Focus();
            browseForm.Show();
        }

        private void SaveAs()
        {
            if (ActiveMap == null) return;
            DialogResult result = folderDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = folderDialog.SelectedPath;
                ActiveMap.Map.Save(path);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }
        #endregion Menu Handlers

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMap == null) return;

            MapDocument propDoc = ActiveMap;
            LevelProp propForm = new LevelProp();
            propForm.LoadMap(propDoc.Map);
            propForm.Text = ActiveMap.Map.Name + " Properties";

            propForm.Saved += () => propDoc.RefreshInfo();

            propForm.Show();
        }

        private void addScreenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ActiveMap != null) ActiveMap.NewScreen();
        }

        private void brushesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (brushForm.Visible) brushForm.Hide();
            else brushForm.Show();
            brushesToolStripMenuItem.Checked = brushForm.Visible;
        }

        private void animateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Animated = !Program.Animated;
            animateToolStripMenuItem.Checked = Program.Animated;
        }
    }
}
