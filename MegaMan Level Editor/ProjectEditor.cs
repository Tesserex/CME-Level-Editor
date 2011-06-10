using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;

namespace MegaMan_Level_Editor
{
    public class ProjectEditor
    {
        public Project Project { get; private set; }

        private bool dirty;
        private bool Dirty
        {
            get { return dirty; }
            set
            {
                bool old = dirty;
                dirty = value;
                if (old != value && DirtyChanged != null) DirtyChanged(value);
            }
        }

        #region Game XML File Stuff

        private Dictionary<string, Entity> entities = new Dictionary<string,Entity>();

        public IEnumerable<Entity> Entities
        {
            get { return entities.Values; }
        }

        public string BaseDir
        {
            get { return Project.BaseDir; }
        }

        public string Name
        {
            get { return Project.Name; }
            set
            {
                if (Project.Name == value) return;
                Project.Name = value;
                Dirty = true;
            }
        }

        public string Author
        {
            get { return Project.Author; }
            set
            {
                if (Project.Author == value) return;
                Project.Author = value;
                Dirty = true;
            }
        }

        public int ScreenWidth
        {
            get { return Project.ScreenWidth; }
            set
            {
                if (Project.ScreenWidth == value) return;
                Project.ScreenWidth = value;
                Dirty = true;
            }
        }
        public int ScreenHeight
        {
            get { return Project.ScreenHeight; }
            set
            {
                if (Project.ScreenHeight == value) return;
                Project.ScreenHeight = value;
                Dirty = true;
            }
        }

        #endregion

        #region GUI Editor Stuff

        private Dictionary<string, StageDocument> openStages = new Dictionary<string,StageDocument>();

        public IEnumerable<string> StageNames
        {
            get
            {
                foreach (var info in Project.Stages) yield return info.Name;
            }
        }

        #endregion

        public event Action<bool> DirtyChanged;
        public event Action<StageDocument> StageAdded;

        public static ProjectEditor CreateNew(string baseDirectory)
        {
            var p = new ProjectEditor();
            return p;
        }

        public static ProjectEditor FromFile(string path)
        {
            var p = new ProjectEditor();
            p.Project.Load(path);
            p.LoadIncludes();
            return p;
        }

        public StageDocument StageByName(string name)
        {
            if (openStages.ContainsKey(name)) return openStages[name];
            foreach (var info in Project.Stages)
            {
                if (info.Name == name)
                {
                    try
                    {
                        StageDocument stage = new StageDocument(this, this.BaseDir, info.StagePath.Absolute);
                        openStages.Add(name, stage);
                        return stage;
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("A required file or directory for the stage was not found:\n\n" + ex.Message, "CME Level Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
            }
            return null;
        }

        private ProjectEditor()
        {
            Project = new Project();
        }

        private void LoadIncludes()
        {
            foreach (string path in Project.Includes)
            {
                string fullpath = Path.Combine(this.BaseDir, path);
                XDocument document = XDocument.Load(fullpath, LoadOptions.SetLineInfo);
                foreach (XElement element in document.Elements())
                {
                    switch (element.Name.LocalName)
                    {
                        case "Entities":
                            LoadEntities(element);
                            break;
                    }
                }
            }
        }

        private void LoadEntities(XElement entitiesNode)
        {
            foreach (XElement entityNode in entitiesNode.Elements("Entity"))
            {
                var entity = new Entity(entityNode, this.BaseDir);
                entities.Add(entity.Name, entity);
            }
        }

        public StageDocument AddStage(string name, string tilesetPath)
        {
            string stageDir = Path.Combine(this.BaseDir, "stages");
            if (!Directory.Exists(stageDir))
            {
                Directory.CreateDirectory(stageDir);
            }
            string stagePath = Path.Combine(stageDir, name);
            if (!Directory.Exists(stagePath))
            {
                Directory.CreateDirectory(stagePath);
            }

            var stage = new StageDocument(this);
            stage.Path = FilePath.FromAbsolute(stagePath, this.BaseDir); // must be set before tileset
            stage.Name = name;
            stage.ChangeTileset(tilesetPath);

            stage.Save();
            
            openStages.Add(name, stage);

            var info = new StageInfo();
            info.Name = name;
            info.StagePath = FilePath.FromAbsolute(stagePath, this.BaseDir);
            Project.AddStage(info);

            this.Save(); // need to save the reference to the new stage

            if (StageAdded != null) StageAdded(stage);

            return stage;
        }

        public void Save()
        {
            Project.Save();
        }

        public bool Close()
        {
            foreach (StageDocument stage in this.openStages.Values)
            {
                if (!stage.Close()) return false;
            }

            if (!ConfirmSave()) return false;

            return true;
        }

        public bool ConfirmSave()
        {
            if (Dirty)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes to " + this.Name + " before closing?", "Save Changes", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes) this.Save();
                else if (result == DialogResult.Cancel) return false;
            }
            return true;
        }
    }
}
