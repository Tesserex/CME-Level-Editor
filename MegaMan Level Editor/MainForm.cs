using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MegaMan;
using WeifenLuo.WinFormsUI.Docking;

namespace MegaMan_Level_Editor
{
    public partial class MainForm : Form
    {
        #region Private Members

        private TilesetStrip tilestrip;
        private ProjectEditor activeProject;
        private StageDocument activeStage;

        public BrushForm brushForm;
        public ProjectForm projectForm;
        public HistoryForm historyForm;
        public EntityForm entityForm;

        private bool drawGrid;
        private bool drawTiles;
        private bool drawBlock;
        private bool drawJoins;
        private bool drawEntities;

        private string recentPath = Path.Combine(Application.StartupPath, "recent.ini");
        private List<string> recentFiles = new List<string>(10);

        private string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

        public ITileBrush CurrentBrush { get; private set; }
        private ToolType currentToolType;
        private Entity currentEntity;

        #endregion Private Members

        #region Properties
        public ProjectEditor ActiveProject
        {
            get { return activeProject; }
            set
            {
                activeProject = value;
                saveToolStripMenuItem.Enabled =
                    closeToolStripMenuItem.Enabled = propertiesToolStripMenuItem.Enabled =
                    newScreenMenuItem.Enabled =
                    manageEnemiesToolStripMenuItem.Enabled = mergeScreenToolStripMenuItem.Enabled =
                    splitScreenToolStripMenuItem.Enabled = addEnemyToolStripMenuItem.Enabled =
                    stageSelectToolStripMenuItem.Enabled = (value != null);
            }
        }

        public StageDocument ActiveStage
        {
            get { return activeStage; }
            set
            {
                activeStage = value;
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

        public bool DrawEntities
        {
            get { return drawEntities; }
            set
            {
                drawEntities = value;
                showEnemiesToolStripMenuItem.Checked = value;
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
            dockPanel1.BringToFront();
            tilestrip.TileChanged += TileChanged;

            Instance = this;

            CreateBrushForm();
            CreateProjectForm();
            CreateHistoryForm();
            CreateEntityForm();

            if (File.Exists(configFile))
            {
                dockPanel1.LoadFromXml(configFile, new DeserializeDockContent(GetContentFromPersistString));
            }

            if (!projectForm.Visible) projectForm.Show(this.dockPanel1, DockState.DockRight);
            if (!historyForm.Visible) historyForm.Show(this.dockPanel1, DockState.DockRight);
            if (!brushForm.Visible) brushForm.Show(this.dockPanel1, DockState.DockLeft);
            if (!entityForm.Visible) entityForm.Show(this.dockPanel1, DockState.DockLeft);

            DrawGrid = false;
            DrawTiles = true;
            DrawBlock = false;
            DrawEntities = true;

            LoadRecentFiles();

            ActiveStage = null;
        }

        void CreateBrushForm()
        {
            brushForm = new BrushForm();
            brushForm.BrushChanged += new BrushChangedHandler(brushForm_BrushChanged);
            brushForm.Shown += (s, e) => brushesToolStripMenuItem.Checked = true;
            brushForm.FormClosing += (s, e) =>
            {
                e.Cancel = true;
                brushForm.Hide();
                brushesToolStripMenuItem.Checked = false;
            };
        }

        void CreateProjectForm()
        {
            projectForm = new ProjectForm();
            projectForm.Shown += (s, e) => projectToolStripMenuItem.Checked = true;
            projectForm.FormClosing += (s, e) =>
            {
                e.Cancel = true;
                projectForm.Hide();
                projectToolStripMenuItem.Checked = false;
            };   
        }

        void CreateHistoryForm()
        {
            historyForm = new HistoryForm();
            historyForm.Shown += (s, e) => historyToolStripMenuItem.Checked = true;
            historyForm.FormClosing += (s, e) =>
            {
                e.Cancel = true;
                historyForm.Hide();
                historyToolStripMenuItem.Checked = false;
            };
        }

        private void CreateEntityForm()
        {
            entityForm = new EntityForm();
            entityForm.Shown += (s, e) => entitiesToolStripMenuItem.Checked = true;
            entityForm.FormClosing += (s, e) =>
            {
                e.Cancel = true;
                entityForm.Hide();
                entitiesToolStripMenuItem.Checked = false;
            };
            entityForm.EntityChanged += new Action<Entity>(entityForm_EntityChanged);
        }

        private void entityForm_EntityChanged(Entity entity)
        {
            currentEntity = entity;
            this.currentToolType = ToolType.Entity;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            this.DrawJoins = false;
        }

        public void ShowStageForm(StageForm form)
        {
            form.DockAreas = DockAreas.Document;
            form.Show(this.dockPanel1);
        }

        public void FocusScreen(StageDocument stage)
        {
            ActiveStage = stage;
            ChangeTileset(stage.Tileset);
        }

        private void ChangeTileset(Tileset tileset)
        {
            if (brushForm != null) brushForm.ChangeTileset(activeStage.Tileset);

            tilestrip.ChangeTileset(tileset);
        }

        private void brushForm_BrushChanged(BrushChangedEventArgs e)
        {
            CurrentBrush = e.Brush;
            AssembleTool();
        }

        private void TileChanged(Tile tile)
        {
            if (tile == null) this.CurrentBrush = null;
            else this.CurrentBrush = new SingleTileBrush(tile);
            AssembleTool();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            File.WriteAllLines(recentPath, recentFiles.ToArray());

            dockPanel1.SaveAsXml(configFile);

            e.Cancel = false;
            base.OnClosing(e);
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(ProjectForm).ToString())
                return projectForm;
            else if (persistString == typeof(HistoryForm).ToString())
                return historyForm;
            else if (persistString == typeof(BrushForm).ToString())
                return brushForm;
            else if (persistString == typeof(EntityForm).ToString())
                return entityForm;
            else return null;
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
                int i = 1;
                foreach (string path in recent.Take(9).Reverse())
                {
                    recentFiles.Add(path);
                    ToolStripMenuItem r = new ToolStripMenuItem(path);
                    r.Click += new EventHandler(RecentMenu_Click);
                    Keys key = (Keys)Enum.Parse(typeof(Keys), ("D" + i.ToString()));
                    r.ShortcutKeys = Keys.Control | key;
                    recentMenuItem.DropDownItems.Add(r);
                    i++;
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

        private void stage_Closed(StageDocument stagedoc)
        {
            // if the tile form is showing this map's tileset, remove it from the form
            if (ActiveStage == stagedoc)
            {
                tilestrip.ChangeTileset(null);
            }
        }
        #endregion Private Methods

        private void OpenProject(string gamefile)
        {
            ProjectEditor project;
            try
            {
                project = ProjectEditor.FromFile(gamefile);
            }
            catch
            {
                MessageBox.Show("The selected file could not be loaded. Perhaps it was created with a different version of this editor.",
                    "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            projectForm.AddProject(project);
            entityForm.LoadEntities(project);
            AddRecentFile(gamefile);
            ActiveProject = project;
        }

        private void AssembleTool()
        {
            switch (currentToolType)
            {
                case ToolType.Brush:
                    this.CurrentTool = null;
                    if (CurrentBrush != null) this.CurrentTool = new BrushTool(CurrentBrush);
                    break;

                case ToolType.Bucket:
                    this.CurrentTool = null;
                    if (CurrentBrush != null) this.CurrentTool = new Bucket(CurrentBrush);
                    break;

                case ToolType.Join:
                    this.CurrentTool = new JoinTool();
                    break;

                case ToolType.Start:
                    this.CurrentTool = new StartPositionTool();
                    break;

                case ToolType.Entity:
                    this.CurrentTool = new EntityTool(currentEntity);
                    break;
            }

            if (currentToolType != ToolType.Entity) entityForm.Deselect();
            
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

        private void joinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawJoins = !DrawJoins;
        }

        private void showEnemiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawEntities = !DrawEntities;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveProject != null)
            {
                ActiveProject.Save();
                if (ActiveStage != null) ActiveStage.Save();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveProject == null) return;

            if (ActiveProject.Close())
            {
                projectForm.CloseProject();
                tilestrip.ChangeTileset(null);
                entityForm.Unload();
                ActiveProject = null;
                ActiveStage = null;
            }
        }

        //*****************
        // Undo/Redo Menu *
        //*****************

        public void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.activeStage.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.activeStage.Redo();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var projectForm = new ProjectProperties();
            projectForm.Owner = this;
            projectForm.Location = new Point((this.Width - projectForm.Width) / 2, (this.Height - projectForm.Height) / 2);
            projectForm.Show();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveStage == null) return;

            StageProp.EditStage(ActiveStage);
        }

        //***************
        // Tool Windows *
        //***************

        private void newScreenStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveStage != null) ScreenProp.CreateScreen(this.ActiveStage);
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

        private void entitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (entityForm.Visible)
                entityForm.Hide();
            else
                entityForm.Show();

            entitiesToolStripMenuItem.Checked = entityForm.Visible;
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

        private void stageSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveProject != null) new StageSelectEdit(this.ActiveProject).Show();
        }

        private void startPosToolButton_Click(object sender, EventArgs e)
        {
            currentToolType = ToolType.Start;
            AssembleTool();
            foreach (ToolStripButton item in toolBar.Items) { item.Checked = false; }
            startPosToolButton.Checked = true;
            this.DrawJoins = false;
        }
    }
}

