using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace MegaMan_Level_Editor
{
    // ========= What this IS, and IS NOT ===============
    // This class controls a single stage. NOT the whole damn project!
    // There should be no way to touch the stage, except through
    // one of these objects! All form updates should be event
    // driven, coming from this class!

    public class StageDocument
    {
        private Map map;

        private StageForm stageForm;

        private Dictionary<string, ScreenDocument> screens = new Dictionary<string,ScreenDocument>();

        public ProjectEditor Project { get; private set; }

        public Point StartPoint
        {
            get { return new Point(map.PlayerStartX, map.PlayerStartY); }
            set
            {
                map.PlayerStartX = value.X;
                map.PlayerStartY = value.Y;
            }
        }

        public event Action<StageDocument> Closed;
        public event Action<ScreenDocument> ScreenAdded;
        public event Action<Join> JoinChanged;
        public event Action<bool> DirtyChanged;

        public StageDocument(ProjectEditor project)
        {
            this.Project = project;
            this.map = new Map();
        }

        public StageDocument(ProjectEditor project, string basepath, string filepath)
        {
            this.Project = project;
            this.map = new Map(FilePath.FromAbsolute(filepath, basepath));

            // wrap all map screens in screendocuments
            // this should be the only time MegaMan.Screen's are touched directly
            foreach (var pair in this.map.Screens)
            {
                WrapScreen(pair.Value);
            }
        }

        // this is going to get encapsulated further, so that even Screens are inaccessible
        public ScreenDocument GetScreen(string name)
        {
            if (screens.ContainsKey(name)) return screens[name];
            return null;
        }

        #region Exposed Map Items

        // this should be removed from the common lib, and implemented directly by me
        public bool Dirty
        {
            get { return map.Dirty; }
            set {
                map.Dirty = value;
                if (DirtyChanged != null) DirtyChanged(value);
            }
        }

        public string Name
        {
            get { return map.Name; }
            set
            {
                map.Name = value;
                RefreshInfo();
            }
        }

        public FilePath Path
        {
            get { return map.StagePath; }
            set { map.StagePath = value; }
        }

        public Tileset Tileset
        {
            get { return map.Tileset; }
        }

        public void ChangeTileset(string path)
        {
            map.ChangeTileset(path);
            Dirty = true;
        }

        public string StartScreen
        {
            get { return map.StartScreen; }
            set { map.StartScreen = value; }
        }

        public int ScreenCount { get { return map.Screens.Count; } }

        public IEnumerable<ScreenDocument> Screens
        {
            get { foreach (var screen in screens.Values) yield return screen; }
        }

        public int JoinCount { get { return map.Joins.Count; } }

        public IEnumerable<MegaMan.Join> Joins
        {
            get { foreach (var join in map.Joins) yield return join; }
        }

        public void Save()
        {
            map.Save();
            Dirty = false;
        }

        public void Save(string directory)
        {
            map.Save(directory);
            Dirty = false;
        }

        #endregion

        public void AddScreen(string name, int tile_width, int tile_height)
        {
            var screen = new MegaMan.Screen(tile_width, tile_height, this.map);
            screen.Name = name;

            this.map.Screens.Add(name, screen);

            if (this.StartScreen == null) this.StartScreen = this.map.Screens.Keys.First();

            ScreenDocument doc = WrapScreen(screen);
            
            screen.Save(System.IO.Path.Combine(this.Path.Absolute, name + ".scn"));

            // now I can do things like fire an event... how useful!
            if (ScreenAdded != null) ScreenAdded(doc);

            this.Save();
        }

        public void AddJoin(Join join)
        {
            map.Joins.Add(join);
            this.Dirty = true;
            if (JoinChanged != null) JoinChanged(join);
        }

        // this should probably be replaced by a join wrapper that has events
        public void RaiseJoinChange(Join join)
        {
            if (JoinChanged != null) JoinChanged(join);
        }

        private void RefreshInfo()
        {
            if (this.stageForm != null) stageForm.SetText();
        }

        public void ReFocus()
        {
            ShowStage();
        }

        public void ShowStage()
        {
            if (this.stageForm == null)
            {
                this.stageForm = new StageForm(this);
                stageForm.GotFocus += new EventHandler(StageForm_GotFocus);
                stageForm.FormClosing += new FormClosingEventHandler(StageForm_FormClosing);
            }

            MainForm.Instance.ShowStageForm(this.stageForm);
            stageForm.Focus();
        }

        public bool Close()
        {
            if (!ConfirmSave()) return false;

            stageForm.FormClosing -= StageForm_FormClosing;
            stageForm.Close();
            if (Closed != null) Closed(this);
            return true;
        }

        public bool ConfirmSave()
        {
            if (Dirty)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes to " + map.Name + " before closing?", "Save Changes", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes) map.Save();
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
            MainForm.Instance.FocusScreen(this);
        }

        private ScreenDocument WrapScreen(MegaMan.Screen screen)
        {
            ScreenDocument doc = new ScreenDocument(screen, this);
            this.screens.Add(screen.Name, doc);
            doc.Renamed += ScreenRenamed;
            doc.TileChanged += () => this.Dirty = true;
            return doc;
        }

        private void ScreenRenamed(string oldName, string newName)
        {
            if (!this.screens.ContainsKey(oldName)) return;
            ScreenDocument doc = this.screens[oldName];
            this.screens.Remove(oldName);
            this.screens.Add(newName, doc);
            if (this.map.StartScreen == oldName) this.map.StartScreen = newName;
            foreach (var join in this.Joins)
            {
                if (join.screenOne == oldName) join.screenOne = newName;
                if (join.screenTwo == oldName) join.screenTwo = newName;
            }
            this.Dirty = true;
        }
    }
}
