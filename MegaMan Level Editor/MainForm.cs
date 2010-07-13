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

        private Dictionary<string, MapDocument> stages = new Dictionary<string, MapDocument>();

        public BrushForm brushForm;
        public ProjectForm projectForm;
        public HistoryForm historyForm;

        private bool drawGrid;
        private bool drawTiles;
        private bool drawBlock;
        private bool drawJoins;

        private string recentPath = Path.Combine(Application.StartupPath, "recent.ini");
        private List<string> recentFiles = new List<string>(10);

        private ITileBrush currentBrush;
        private ToolType currentToolType;
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
                if (DrawOptionToggled != null) DrawOptionToggled();
            }
        }

        public bool DrawTiles
        {
            get { return drawTiles; }
            set
            {
                drawTiles = value;
                showBackgroundsToolStripMenuItem.Checked = value;
                if (DrawOptionToggled != null) DrawOptionToggled();
            }
        }

        public bool DrawBlock
        {
            get { return drawBlock; }
            set
            {
                drawBlock = value;
                showBlockingToolStripMenuItem.Checked = value;
                if (DrawOptionToggled != null) DrawOptionToggled();
            }
        }

        public bool DrawJoins
        {
            get { return drawJoins; }
            set
            {
                drawJoins = value;
                joinsToolStripMenuItem.Checked = value;
                if (DrawOptionToggled != null) DrawOptionToggled();
            }
        }

        public ITool CurrentTool { get; private set; }
        #endregion

        public event Action DrawOptionToggled;
        public event EventHandler<ToolChangedEventArgs> ToolChanged;

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
            LoadFormSettings(this, this.projectForm, this.historyForm, this.brushForm);
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
            ChangeTileset(map.Tileset);
        }

        private void ChangeTileset(Tileset tileset)
        {
            if (brushForm != null) brushForm.ChangeTileset(activeMap.Tileset);

            tilestrip.ChangeTileset(tileset);
        }

        private void brushForm_BrushChanged(BrushChangedEventArgs e)
        {
            currentBrush = e.Brush;
            AssembleTool();
        }

        private void TileChanged(Tile tile)
        {
            this.currentBrush = new SingleTileBrush(tile);
            AssembleTool();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!CheckSaveOnClose())
            {
                e.Cancel = true;
                return;
            }
            File.WriteAllLines(recentPath, recentFiles.ToArray());

            SaveFormSettings(this, this.historyForm, this.brushForm, this.projectForm);

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
        public MapDocument OpenStage(string stageName, string path)
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
                return null;
            }
        }

        public static string StagePathFor(string stageName)
        {
            var stagePath = Path.Combine(Path.Combine(MainForm.Instance.rootPath, "stages"), stageName);
            return Path.Combine(stagePath, "map.xml");
        }

        /* *
         * LoadMapFromPath - Load the stage based on the path (underlying implementation of OpenMap)
         * path = relative path to the root project directory
         * */
        // THIS MUST DIE
        public MapDocument LoadStageFromPath(String stageName, String path)
        {
            foreach (var mapdoc in stages.Values)
            {
                if (mapdoc.Path == Path.Combine(this.rootPath, path))
                {
                    return mapdoc;
                }
            }

            var stage = new MapDocument(this.rootPath, path);
            stages[stage.Name] = stage;

            stage.Closed += new Action<MapDocument>(map_Closed);

            return stage;
        }

        private void map_Closed(MapDocument mapdoc)
        {
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
        private void OpenProject(string gamefile)
        {
            AddRecentFile(gamefile);
            var project = ProjectEditor.FromFile(gamefile);
            projectForm.AddProject(project);
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
                ActiveMap.Save(path);
            }
        }

        private void AssembleTool()
        {
            switch (currentToolType)
            {
                case ToolType.Brush:
                    if (currentBrush != null) this.CurrentTool = new BrushTool(currentBrush);
                    break;

                case ToolType.Bucket:
                    if (currentBrush != null) this.CurrentTool = new Bucket(currentBrush);
                    break;

                case ToolType.Join:
                    this.CurrentTool = new JoinTool();
                    break;
            }
            
            if (ToolChanged != null) ToolChanged(this, new ToolChangedEventArgs(CurrentTool));
        }

        //***********************
        // Click Event Handlers *
        //***********************

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Select a CME Game Project File";
            dialog.Filter = "Game Project (XML)|*.xml";
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file = dialog.FileName;
                OpenProject(file);
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
            if (ActiveMap != null)
            {
                if (ActiveMap.Path != null)
                    ActiveMap.Save();
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
            var projectForm = new ProjectProperties();
            projectForm.Owner = this;
            projectForm.Location = new Point((this.Width - projectForm.Width) / 2, (this.Height - projectForm.Height) / 2);
            projectForm.Show();
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
            propForm.LoadMap(propDoc);
            propForm.Show();
        }

        //***************
        // Tool Windows *
        //***************

        private void newScreenStripMenuItem_Click(object sender, EventArgs e)
        {
            var screenPropForm = new ScreenProp(this.ActiveMap);
            screenPropForm.Show();
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

        /* *
         * Utility methods... smells like they belong in common library
         * */
        public static ScreenDocument GetScreen(string stageName, string screenName)
        {
            return MainForm.Instance.stages[stageName].GetScreen(screenName);
        }

        private void brushToolButton_Click(object sender, EventArgs e)
        {
            currentToolType = ToolType.Brush;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            brushToolButton.Checked = true;
            this.DrawJoins = false;
            this.DrawTiles = true;
        }

        private void bucketToolButton_Click(object sender, EventArgs e)
        {
            currentToolType = ToolType.Bucket;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            bucketToolButton.Checked = true;
            this.DrawJoins = false;
            this.DrawTiles = true;
        }

        private void joinToolButton_Click(object sender, EventArgs e)
        {
            currentToolType = ToolType.Join;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            joinToolButton.Checked = true;
            this.DrawJoins = true;
        }

        private void SaveFormSettings(params Form[] forms)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Form form in forms)
            {
                sb.Append(form.Name)
                    .Append("|").Append(form.Location.X)
                    .Append("|").Append(form.Location.Y)
                    .Append("|").Append(form.Width)
                    .Append("|").Append(form.Height)
                    .Append("||");
            }

            Properties.Settings.Default.Windows = sb.ToString();
            Properties.Settings.Default.Save();
        }

        private void LoadFormSettings(params Form[] forms)
        {
            string settings = Properties.Settings.Default.Windows;
            if (string.IsNullOrEmpty(settings)) return;

            Dictionary<string, Form> formDict = new Dictionary<string, Form>();
            foreach (Form f in forms) formDict.Add(f.Name, f);

            string[] formSettingsList = settings.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string formSettings in formSettingsList)
            {
                string[] properties = formSettings.Split(new char[] { '|' });
                if (!formDict.ContainsKey(properties[0])) continue;
                Form tgtForm = formDict[properties[0]];

                tgtForm.Location = new Point(int.Parse(properties[1]), int.Parse(properties[2]));
                tgtForm.Width = int.Parse(properties[3]);
                tgtForm.Height = int.Parse(properties[4]);
            }
        }

        private void joinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawJoins = !DrawJoins;
        }
    }
}

