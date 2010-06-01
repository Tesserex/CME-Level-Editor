using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ScreenDict = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, MegaMan_Level_Editor.StageForm>>;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public partial class MainForm : Form
    {
        #region Public Members
        /* *
         * rootPath - The path of the current project
         * */
        public String rootPath;

        private TilesetStrip tilestrip;
        private MapDocument activeMap;

        public Dictionary<string, MapDocument> stages = new Dictionary<string, MapDocument>();
        public Dictionary<string, StageForm> stageForms = new Dictionary<string, StageForm>();

        public BrushForm brushForm;
        public ProjectForm projectForm;
        public HistoryForm historyForm;

        private bool drawGrid;
        private bool drawTiles;
        private bool drawBlock;

        private string recentPath = Application.StartupPath + "\\recent.ini";
        private List<string> recentFiles = new List<string>(10);
        private int untitledCount = 0;
        #endregion Private Members

        #region Properties
        public MapDocument ActiveMap
        {
            get { return activeMap; }
            set
            {
                activeMap = value;
                saveToolStripMenuItem.Enabled = saveAsToolStripMenuItem.Enabled =
                    closeToolStripMenuItem.Enabled = propertiesToolStripMenuItem.Enabled =
                    newScreenMenuItem.Enabled =
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

                foreach (MapDocument map in stages.Values)
                    map.DrawGrid = value;
            }
        }

        public bool DrawTiles
        {
            get { return drawTiles; }
            set
            {
                drawTiles = value;
                showBackgroundsToolStripMenuItem.Checked = value;

                foreach (MapDocument map in stages.Values) map.DrawTiles = value;
            }
        }

        public bool DrawBlock
        {
            get { return drawBlock; }
            set
            {
                drawBlock = value;
                showBlockingToolStripMenuItem.Checked = value;

                foreach (MapDocument map in stages.Values) map.DrawBlock = value;
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

            CreateBrushForm();
            CreateProjectForm();
            CreateHistoryForm();

            DrawGrid = false;
            DrawTiles = true;
            DrawBlock = false;

            LoadRecentFiles();
            LoadWindowPositions();
        }

        public void LoadWindowPositions()
        {
            MessageBox.Show("TODO: Load the window positions from last time");
        }

        void CreateBrushForm()
        {
            brushForm = new BrushForm();
            brushForm.Show();
            brushForm.BrushChanged += new BrushChangedHandler(brushForm_BrushChanged);
            brushForm.Shown += (s, e) => brushesToolStripMenuItem.Checked = true;
            brushForm.FormClosing += (s, e) =>
            {
                e.Cancel = true;
                brushForm.Hide();
                brushesToolStripMenuItem.Checked = false;
            };
            brushForm.Anchor = AnchorStyles.Left;
            brushForm.Owner = this;
        }

        void CreateProjectForm()
        {
            projectForm = new ProjectForm();
            projectForm.Show();
            projectForm.Shown += (s, e) => projectToolStripMenuItem.Checked = true;
            projectForm.FormClosing += (s, e) =>
            {
                e.Cancel = true;
                projectForm.Hide();
                projectToolStripMenuItem.Checked = false;
            };
            projectForm.Anchor = AnchorStyles.Left;
            projectForm.Owner = this;
        }

        void CreateHistoryForm()
        {
            historyForm = new HistoryForm();
            historyForm.Show();
            historyForm.Shown += (s, e) => historyToolStripMenuItem.Checked = true;
            historyForm.FormClosing += (s, e) =>
            {
                e.Cancel = true;
                historyForm.Hide();
                historyToolStripMenuItem.Checked = false;
            };
            historyForm.Anchor = AnchorStyles.Left;
            historyForm.Owner = this;
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
            MessageBox.Show("TODO: Save window positions");
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

        /* *
         * OpenStage - Open up a stage from the /stages/ directory
         * path = relative path to the root project directory
         * */
        public List<MegaMan.Screen> OpenStage(string stageName, string path)
        {
            // TODO: Start adding a Util class to MegaMan common so we can
            // refactor out stuff like "StagePathFor(path)", instead of
            // what we have here..

            var stagePath = StagePathFor(stageName);
            if (File.Exists(stagePath))
            {
                return LoadStageFromPath(stageName, path);
            }
            else
            {
                MessageBox.Show("Sorry, but this is not a stage directory! Stage path was : " + stagePath);
                return new List<MegaMan.Screen>();
            }
        }

        public static string StagePathFor(string stageName)
        {
            var stagePath = Path.Combine(Path.Combine(MainForm.Instance.rootPath, "stages"), stageName);
            return Path.Combine(stagePath, "map.xml");
        }

        public static string ScreenPathFor(string stageName, string screenName)
        {
            var stagePath = Path.Combine(Path.Combine(MainForm.Instance.rootPath, "stages"), stageName);
            return Path.Combine(stagePath, screenName + ".scn");
        }

        /* *
         * LoadMapFromPath - Load the stage based on the path (underlying implementation of OpenMap)
         * path = relative path to the root project directory
         * */
        public List<MegaMan.Screen> LoadStageFromPath(String stageName, String path)
        {
            foreach (var mapdoc in stages.Values)
            {
                if (Path.GetFullPath(mapdoc.Map.FileDir) == Path.GetFullPath(path))
                {
                    // only focus it if it's not already focused
                    if (ActiveMap.Map != mapdoc.Map)
                        mapdoc.ReFocus();
                }
            }

            var stage = new MapDocument(path, this);
            stages[stage.Map.Name] = stage;

            stage.DrawBlock = this.drawBlock;
            stage.DrawGrid = this.drawGrid;
            stage.DrawTiles = this.drawTiles;

            stage.Closed += new Action<MapDocument>(map_Closed);
            stage.ReFocus();

            return stage.Map.Screens.Select((pair) => { return pair.Value; }).ToList();
        }

        private void map_Closed(MapDocument mapdoc)
        {
            stages.Remove(mapdoc.Map.Name);

            // if the tile form is showing this map's tileset, remove it from the form
            if (ActiveMap == mapdoc)
            {
                tilestrip.ChangeTileset(null);
            }
        }
        #endregion Private Methods


        /* *
         * OpenProject - Set root path for maps and show list of maps to user
         * */
        void OpenProject(String rootPath)
        {
            //TODO: Make sure path contains "game.xml"
            if (File.Exists(System.IO.Path.Combine(rootPath, "game.xml")))
            {
                this.rootPath = rootPath;
                AddRecentFile(rootPath);
                projectForm.OpenProject(rootPath);
            }
            else
            {
                MessageBox.Show("Sorry, but this is not a CME project!");
            }
        }

        void ShowStages()
        {
            projectForm.Show();
        }

        private bool CheckSaveOnClose()
        {
            foreach (MapDocument map in stages.Values)
            {
                if (!map.ConfirmSave()) return false;
            }
            return true;
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


        //***********************
        // Click Event Handlers *
        //***********************

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = folderDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = folderDialog.SelectedPath;
                OpenProject(path);
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
            //OpenMap(t.Text);
            OpenProject(t.Text);
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
                if (ActiveMap.Map.FileDir != null)
                    ActiveMap.Map.Save();
                else
                    SaveAs();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMap == null)
                return;
            ActiveMap.Close();
        }

        //*****************
        // Undo/Redo Menu *
        //*****************

        public void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.activeMap.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.activeMap.Redo();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Map map = new Map();
            untitledCount++;
            map.Name = "Untitled" + untitledCount.ToString();

            StageProp propForm = new StageProp();
            propForm.LoadMap(map);
            propForm.Text = "New Stage Properties";
            propForm.Show();

            propForm.OkPressed += () =>
            {
                MapDocument document = new MapDocument(map, this);
                // document.NewScreen();
                document.NewStage();
                stages.Add(map.Name, document);
            };
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMap == null) return;

            MapDocument propDoc = ActiveMap;
            StageProp propForm = new StageProp();
            propForm.LoadMap(propDoc.Map);
            propForm.Text = ActiveMap.Map.Name + " Properties";

            propForm.Saved += () => propDoc.RefreshInfo();

            propForm.Show();
        }

        //***************
        // Tool Windows *
        //***************

        private void newScreenStripMenuItem_Click(object sender, EventArgs e)
        {
            var screenPropForm = new ScreenProp();
            screenPropForm.OK += new Action<ScreenProp>(screenPropForm_OK);
            screenPropForm.Show();
        }

        private void screenPropForm_OK(ScreenProp prop)
        {
            if (prop.Screen == null)
            {
                var screen = new MegaMan.Screen(prop.Width, prop.Height, this.activeMap.Map);
                screen.Name = prop.ScreenName;
                this.activeMap.Map.Screens.Add(prop.ScreenName, screen);
                screen.Save(ScreenPathFor(MainForm.Instance.ActiveMap.Map.Name, prop.ScreenName));
            }
            else
            {
                // Rename the screen
                var screen = prop.Screen;
                string oldName = screen.Name;

                // this DEFINITELY needs to be a method of Map - "RenameScreen" or something
                // it's dangerously risky doing it this way in the client code if we forget one time
                // this is why "everything public" == bad, encapsulation == good
                screen.Name = prop.ScreenName;
                screen.Map.Screens.Remove(oldName);
                screen.Map.Screens.Add(prop.ScreenName, screen);

                screen.Resize(prop.ScreenWidth, prop.ScreenHeight);

                // Update the project tree
                var projectForm = MainForm.Instance.projectForm;
                var stageNode = projectForm.projectView.Nodes.Find(screen.Map.Name, true).First();
                var screens = MainForm.GetStage(screen.Map.Name).Screens.Select((pair) => { return pair.Value; }).ToList();
                projectForm.LoadScreenSubtree(stageNode, screens);

                var stageForm = MainForm.Instance.stageForms[screen.Map.Name];
                var surface = stageForm.GetSurface(oldName);

                // Update the screen surfaces
                if (oldName != prop.ScreenName)
                {
                    stageForm.RenameSurface(oldName, prop.ScreenName);
                }

                surface.ResizeLayers();
                stageForm.AlignScreenSurfaces();
            }
        }

        private void brushesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (brushForm.Visible)
                brushForm.Hide();
            else
                brushForm.Show();
            brushesToolStripMenuItem.Checked = brushForm.Visible;
        }

        private void animateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Animated = !Program.Animated;
            animateToolStripMenuItem.Checked = Program.Animated;
        }

        private void projectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectForm.Visible)
                projectForm.Hide();
            else
                projectForm.Show();

            projectToolStripMenuItem.Checked = projectForm.Visible;
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (historyForm.Visible)
                historyForm.Hide();
            else
                historyForm.Show();

            historyToolStripMenuItem.Checked = historyForm.Visible;
        }

        private void windowsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /* *
         * Utility methods... smells like they belong in common library
         * */
        public static MegaMan.Map GetStage(string stageName)
        {
            return MainForm.Instance.stages[stageName].Map;
        }

        public static MegaMan.Screen GetScreen(string stageName, string screenName)
        {
            return GetStage(stageName).Screens[screenName];
        }

        public static void UpdateScreenName(string stageName, string oldScreenName, string newScreenName)
        {
            if (oldScreenName != newScreenName)
            {
                var screen = GetScreen(stageName, oldScreenName);
                screen.Name = newScreenName;
                MainForm.Instance.stages[stageName].Map.Screens.Add(newScreenName, screen);
                MainForm.Instance.stages[stageName].Map.Screens.Remove(oldScreenName);
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }


    }
}

