using System;
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
        private TileBoxForm tileForm;
        private BrowseForm browseForm;
        private ScreenDict openScreens;
        private ScreenForm focusScreen;
        private List<Map> openMaps;
        private BrushForm brushForm;

        private bool drawGrid;
        private bool drawTiles;
        private bool drawBlock;

        private string recentPath = Application.StartupPath + "\\recent.ini";
        private List<string> recentFiles = new List<string>(10);
        private int untitledCount = 0;
        #endregion Private Members

        #region Properties
        public bool DrawGrid
        {
            get { return drawGrid; }
            set
            {
                drawGrid = value;
                showGridToolStripMenuItem.Checked = value;

                foreach (Dictionary<string, ScreenForm> map in openScreens.Values)
                    foreach (ScreenForm screen in map.Values)
                    {
                        screen.DrawGrid = value;
                    }
            }
        }

        public bool DrawTiles
        {
            get { return drawTiles; }
            set
            {
                drawTiles = value;
                showBackgroundsToolStripMenuItem.Checked = value;

                foreach (Dictionary<string, ScreenForm> map in openScreens.Values)
                    foreach (ScreenForm screen in map.Values)
                {
                    screen.DrawTiles = value;
                }
            }
        }

        public bool DrawBlock
        {
            get { return drawBlock; }
            set
            {
                drawBlock = value;
                showBlockingToolStripMenuItem.Checked = value;

                foreach (Dictionary<string, ScreenForm> map in openScreens.Values)
                    foreach (ScreenForm screen in map.Values)
                {
                    screen.DrawBlock = value;
                }

                tileForm.DrawBlock = value;
            }
        }
        #endregion

        public event BrushChangedHandler BrushChanged;

        public static MainForm Instance { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            Instance = this;

            openMaps = new List<Map>();
            openScreens = new ScreenDict();

            tileForm = new TileBoxForm();
            tileForm.Show();
            tileForm.SelectedChanged += new Action(tileForm_SelectedChanged);
            tileForm.Shown += (s,e) => tilesetToolStripMenuItem.Checked = true;
            tileForm.FormClosing += (s, e) => { e.Cancel = true; tileForm.Hide(); tilesetToolStripMenuItem.Checked = false; };
            tileForm.Top = 50;
            tileForm.Left = 20;

            brushForm = new BrushForm();
            brushForm.Show();
            brushForm.BrushChanged += new BrushChangedHandler(brushForm_BrushChanged);
            brushForm.Shown += (s, e) => brushesToolStripMenuItem.Checked = true;
            brushForm.FormClosing += (s, e) => { e.Cancel = true; brushForm.Hide(); brushesToolStripMenuItem.Checked = false; };
            brushForm.Top = 50;
            brushForm.Left = this.Width - brushForm.Width - 20;

            DrawGrid = false;
            DrawTiles = true;
            DrawBlock = false;

            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            LoadRecentFiles();
        }

        void brushForm_BrushChanged(BrushChangedEventArgs e)
        {
            if (BrushChanged != null) BrushChanged(e);
        }

        void tileForm_SelectedChanged()
        {
            ITileBrush brush = new SingleTileBrush(tileForm.Selected);
            BrushChangedEventArgs args = new BrushChangedEventArgs(brush);
            if (BrushChanged != null) BrushChanged(args);
        }

        void Application_ApplicationExit(object sender, EventArgs e)
        {
            CheckSaveOnQuit();
            File.WriteAllLines(recentPath, recentFiles.ToArray());
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
            if (openScreens.ContainsKey(path))
            {
                openScreens[path].First().Value.Focus();
                return;
            }

            Map map = new Map(path);
            openMaps.Add(map);
            KeyValuePair<string, MegaMan.Screen> pair = map.Screens.First();

            ScreenForm screenform = new ScreenForm();
            screenform.MdiParent = this;
            screenform.SetScreen(map, pair.Value);
            screenform.Text = map.Name + " - " + pair.Value.Name;
            screenform.GotFocus += new EventHandler(screenform_GotFocus);
            screenform.Show();

            AddRecentFile(path);

            openScreens[path] = new Dictionary<string,ScreenForm>();
            openScreens[path][pair.Value.Name] = screenform;

            tileForm.Tileset = map.Tileset;
            brushForm.ChangeTileset(map.Tileset);
        }

        void screenform_GotFocus(object sender, EventArgs e)
        {
            focusScreen = (ScreenForm)sender;
            if (tileForm != null) tileForm.Tileset = focusScreen.Map.Tileset;
            if (brushForm != null) brushForm.ChangeTileset(focusScreen.Map.Tileset);
        }

        void mapform_FormClosed(object sender, FormClosedEventArgs e)
        {
            ScreenForm form = sender as ScreenForm;
            openScreens[form.Path].Remove(form.Name);
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

        private void CheckSaveOnQuit()
        {
            foreach (Map map in openMaps)
            {
                if (map.Dirty)
                {
                    DialogResult result = MessageBox.Show("Do you want to save changes to " + map.Name + " before closing?", "Save Changes", MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Yes) map.Save();
                    else if (result == DialogResult.No) continue;
                    else return;
                }
            }
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
            if (focusScreen != null && focusScreen.Map.Loaded)
            {
                if (focusScreen.Map.FileDir != null) focusScreen.Map.Save();
                else SaveAs();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (focusScreen == null) return;

            foreach (ScreenForm screen in openScreens[focusScreen.Map.FileDir].Values)
                screen.Close();
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

            openMaps.Add(map);

            openScreens[map.Name] = new Dictionary<string, ScreenForm>();

            LevelProp propForm = new LevelProp();
            propForm.LoadMap(map);
            propForm.Show();

            propForm.FormClosed += (s, ev) =>
            {
                AddScreen(map);
            };
        }

        private void browseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (browseForm == null)
            {
                browseForm = new BrowseForm();
                //browseForm.MdiParent = this;
                browseForm.ScreenSelected += new Action<Map, string>(browseForm_ScreenChanged);
            }
            browseForm.SetMap(focusScreen.Map); // forces a refresh of screens
            browseForm.Focus();
            browseForm.Show();
        }

        void browseForm_ScreenChanged(Map map, string screenId)
        {
            ShowScreen(map, screenId);
        }

        private void ShowScreen(Map map, string name)
        {
            if (openScreens[UniqueName(map)].ContainsKey(name))
            {
                openScreens[UniqueName(map)][name].Show();
                openScreens[UniqueName(map)][name].Focus();
            }
            else
            {
                ScreenForm screenform = new ScreenForm();
                screenform.MdiParent = this;
                screenform.SetScreen(map, map.Screens[name]);
                screenform.Text = map.Name + " - " + name;
                screenform.GotFocus += new EventHandler(screenform_GotFocus);
                screenform.Show();

                openScreens[UniqueName(map)][name] = screenform;
            }
        }

        private void SaveAs()
        {
            if (focusScreen == null) return;
            DialogResult result = folderDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = folderDialog.SelectedPath;

                string oldkey = UniqueName(focusScreen.Map);
                focusScreen.Map.Save(path);
                Dictionary<string, ScreenForm> screens = openScreens[oldkey];
                openScreens.Remove(oldkey);
                openScreens[UniqueName(focusScreen.Map)] = screens;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }
        #endregion Menu Handlers

        private string UniqueName(Map map)
        {
            if (map.FileDir != null && map.FileDir.Length > 0) return map.FileDir;
            return map.Name;
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LevelProp propForm = new LevelProp();
            propForm.LoadMap(focusScreen.Map);

            propForm.Show();
        }

        private void addScreenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddScreen(focusScreen.Map);
        }

        private void AddScreen(Map map)
        {
            MegaMan.Screen screen = new MegaMan.Screen(16, 14, map.Tileset);
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
                map.Screens.Add(screen.Name, screen);

                ScreenForm screenform = new ScreenForm();
                screenform.MdiParent = this;
                screenform.SetScreen(map, screen);
                screenform.Text = map.Name + " - " + screen.Name;
                screenform.GotFocus += new EventHandler(screenform_GotFocus);
                screenform.Show();

                openScreens[UniqueName(map)][screen.Name] = screenform;
            };
        }

        private void tilesetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tileForm.Visible) tileForm.Hide();
            else tileForm.Show();
        }

        private void brushesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (brushForm.Visible) brushForm.Hide();
            else brushForm.Show();
        }

        private void animateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Animated = !Program.Animated;
            animateToolStripMenuItem.Checked = Program.Animated;
        }
    }
}
