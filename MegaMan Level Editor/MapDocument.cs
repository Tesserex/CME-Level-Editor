using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan;
using System.Windows.Forms;
using System.IO;

namespace MegaMan_Level_Editor
{
    // ========= What this IS, and IS NOT ===============
    // This class controls a single map / stage, whatever
    // you want to call it. NOT the whole damn project!
    // There should be no way to touch the stage, except through
    // one of these objects! All form updates should be event
    // driven, coming from this class!

    public class MapDocument
    {
        private MainForm parent;

        private Map map;

        private StageForm stageForm;

        private Dictionary<string, ScreenDocument> screens = new Dictionary<string,ScreenDocument>();

        public event Action<MapDocument> Closed;
        public event Action<ScreenDocument> ScreenAdded;
        public event Action<Join> JoinChanged;
        public event Action<bool> DirtyChanged;

        public MapDocument(MainForm parent)
        {
            this.parent = parent;
            this.map = new Map();
        }

        // TODO : Rename Map to Stages.. More consistent naming
        public MapDocument(string path, MainForm parent)
        {
            this.parent = parent;
            this.map = new Map(MainForm.Instance.rootPath, path);

            // wrap all map screens in screendocuments
            // this should be the only time MegaMan.Screen's are touched directly
            foreach (var pair in this.map.Screens)
            {
                this.screens.Add(pair.Key, new ScreenDocument(pair.Value, this));
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

        public string Path
        {
            get { return map.FileDir; }
        }

        public Tileset Tileset
        {
            get { return map.Tileset; }
        }

        public void ChangeTileset(string path)
        {
            map.ChangeTileset(path);
        }

        public string StartScreen { get { return map.StartScreen; } }

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
            if (map.Loaded && map.FileDir != null) map.Save();
        }

        public void Save(string directory)
        {
            if (map.Loaded) map.Save(directory);
        }

        #endregion

        public void AddScreen(string name, int tile_width, int tile_height)
        {
            var screen = new MegaMan.Screen(tile_width, tile_height, this.map);
            screen.Name = name;

            this.map.Screens.Add(name, screen);

            ScreenDocument doc = WrapScreen(screen);
            
            screen.Save(System.IO.Path.Combine(this.Path, name + ".scn"));

            // now I can do things like fire an event... how useful!
            if (ScreenAdded != null) ScreenAdded(doc);
        }

        public void AddJoin(Join join)
        {
            map.Joins.Add(join);
            map.Dirty = true;
            if (JoinChanged != null) JoinChanged(join);
        }

        private void RefreshInfo()
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
                this.stageForm = new StageForm(this);
                stageForm.MdiParent = parent;
                stageForm.GotFocus += new EventHandler(StageForm_GotFocus);
                stageForm.FormClosing += new FormClosingEventHandler(StageForm_FormClosing);
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
            if (map.Dirty)
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
            parent.FocusScreen(this);
        }

        private ScreenDocument WrapScreen(MegaMan.Screen screen)
        {
            ScreenDocument doc = new ScreenDocument(screen, this);
            this.screens.Add(screen.Name, doc);
            doc.Renamed += ScreenRenamed;
            return doc;
        }

        private void ScreenRenamed(string oldName, string newName)
        {
            if (!this.screens.ContainsKey(oldName)) return;
            ScreenDocument doc = this.screens[oldName];
            this.screens.Remove(oldName);
            this.screens.Add(newName, doc);
        }
    }
}
