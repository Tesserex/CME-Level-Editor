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
    public class BossInfo
    {
        public int Slot { get; set; }
        public string Name { get; set; }
        public FilePath PortraitPath { get; set; }
        public string Stage { get; set; }
    }

    public class StageInfo
    {
        public string Name { get; set; }
        public FilePath StagePath { get; set; }
    }

    public class ProjectEditor
    {
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

        private List<StageInfo> stages = new List<StageInfo>();
        private List<BossInfo> bosses = new List<BossInfo>(8);
        private List<string> entityFiles = new List<string>();
        private Sprite bossFrameSprite;

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name == value) return;
                name = value;
                Dirty = true;
            }
        }

        private string author;
        public string Author
        {
            get { return author; }
            set
            {
                if (author == value) return;
                author = value;
                Dirty = true;
            }
        }
        public string GameFile { get; private set; }
        public string BaseDir { get; private set; }

        private int screen_width, screen_height;
        public int ScreenWidth
        {
            get { return screen_width; }
            set
            {
                if (screen_width == value) return;
                screen_width = value;
                Dirty = true;
            }
        }
        public int ScreenHeight
        {
            get { return screen_height; }
            set
            {
                if (screen_height == value) return;
                screen_height = value;
                Dirty = true;
            }
        }

        public Sprite BossFrameSprite
        {
            get { return this.bossFrameSprite; }
            set
            {
                this.bossFrameSprite = value;
                Dirty = true;
            }
        }

        private FilePath stageSelectBackground, stageSelectMusic, stageSelectChange, pauseBackground, pauseChange, pauseSound;

        public FilePath StageSelectMusic
        {
            get { return stageSelectMusic; }
            set
            {
                if (stageSelectMusic != null && stageSelectMusic.Absolute == value.Absolute) return;
                stageSelectMusic = value;
                Dirty = true;
            }
        }

        public FilePath StageSelectBackground
        {
            get { return stageSelectBackground; }
            set
            {
                if (stageSelectBackground != null && stageSelectBackground.Absolute == value.Absolute) return;
                stageSelectBackground = value;
                Dirty = true;
            }
        }

        public FilePath StageSelectChangeSound
        {
            get { return stageSelectChange; }
            set
            {
                if (stageSelectChange != null && stageSelectChange.Absolute == value.Absolute) return;
                stageSelectChange = value;
                Dirty = true;
            }
        }

        private int bossHoriz, bossVert;

        public int BossSpacingHorizontal
        {
            get { return bossHoriz; }
            set
            {
                if (bossHoriz == value) return;
                bossHoriz = value;
                Dirty = true;
            }
        }

        public int BossSpacingVertical
        {
            get { return bossVert; }
            set
            {
                if (bossVert == value) return;
                bossVert = value;
                Dirty = true;
            }
        }

        public FilePath PauseScreenBackground
        {
            get { return pauseBackground; }
            set
            {
                if (pauseBackground != null && pauseBackground.Absolute == value.Absolute) return;
                pauseBackground = value;
                Dirty = true;
            }
        }

        public FilePath PauseChangeSound
        {
            get { return pauseChange; }
            set
            {
                if (pauseChange != null && pauseChange.Absolute == value.Absolute) return;
                pauseChange = value;
                Dirty = true;
            }
        }

        public FilePath PauseSound
        {
            get { return pauseSound; }
            set
            {
                if (pauseSound != null && pauseSound.Absolute == value.Absolute) return;
                pauseSound = value;
                Dirty = true;
            }
        }

        public Point PauseLivesPosition { get; private set; }

        #endregion

        #region GUI Editor Stuff

        private Dictionary<string, StageDocument> openStages = new Dictionary<string,StageDocument>();

        public IEnumerable<string> StageNames
        {
            get
            {
                foreach (var info in stages) yield return info.Name;
            }
        }

        #endregion

        public event Action<bool> DirtyChanged;
        public event Action<StageDocument> StageAdded;

        public static ProjectEditor CreateNew(string baseDirectory)
        {
            var p = new ProjectEditor();
            p.BaseDir = baseDirectory;
            p.GameFile = Path.Combine(baseDirectory, "Game.xml");
            return p;
        }

        public static ProjectEditor FromFile(string path)
        {
            var p = new ProjectEditor();
            p.Load(path);
            return p;
        }

        public StageDocument StageByName(string name)
        {
            if (openStages.ContainsKey(name)) return openStages[name];
            foreach (var info in stages)
            {
                if (info.Name == name)
                {
                    StageDocument stage = new StageDocument(this, this.BaseDir, info.StagePath.Absolute);
                    openStages.Add(name, stage);
                    return stage;
                }
            }
            return null;
        }

        public BossInfo BossAtSlot(int slot)
        {
            foreach (var info in bosses)
            {
                if (info.Slot == slot) return info;
            }
            // find the next available one
            foreach (var info in bosses)
            {
                if (info.Slot == -1)
                {
                    info.Slot = slot;
                    return info;
                }
            }
            return null;
        }

        private ProjectEditor()
        {
            // sensible defaults where possible
            ScreenWidth = 256;
            ScreenHeight = 224;
            BossSpacingHorizontal = 24;
            BossSpacingVertical = 16;

            for (int i = 0; i < 8; i++)
            {
                var boss = new BossInfo();
                boss.Slot = -1;
                bosses.Add(boss);
            }
        }

        private void Load(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("The project file does not exist: " + path);

            try
            {
                GameFile = path;
                BaseDir = Path.GetDirectoryName(path);
                XElement reader = XElement.Load(path);

                XAttribute nameAttr = reader.Attribute("name");
                if (nameAttr != null) this.Name = nameAttr.Value;

                XAttribute authAttr = reader.Attribute("author");
                if (authAttr != null) this.Author = authAttr.Value;

                XElement sizeNode = reader.Element("Size");
                if (sizeNode != null)
                {
                    int across, down;
                    if (int.TryParse(sizeNode.Attribute("x").Value, out across))
                    {
                        ScreenWidth = across;
                    }
                    else ScreenWidth = 0;

                    if (int.TryParse(sizeNode.Attribute("y").Value, out down))
                    {
                        ScreenHeight = down;
                    }
                    else ScreenHeight = 0;
                }

                XElement stagesNode = reader.Element("Stages");
                if (stagesNode != null)
                {
                    foreach (XElement stageNode in stagesNode.Elements("Stage"))
                    {
                        var info = new StageInfo();
                        info.Name = GetNodeAttr(stageNode, "name");
                        info.StagePath = FilePath.FromRelative(GetNodeAttr(stageNode, "path"), this.BaseDir);
                        stages.Add(info);
                    }
                }

                XElement stageSelectNode = reader.Element("StageSelect");
                if (stageSelectNode != null)
                {
                    XElement musicNode = stageSelectNode.Element("Music");
                    if (musicNode != null) this.StageSelectMusic = FilePath.FromRelative(musicNode.Value, this.BaseDir);

                    string changepath = GetNodeAttr(stageSelectNode, "ChangeSound", "path");
                    if (changepath != "") this.StageSelectChangeSound = FilePath.FromRelative(changepath, this.BaseDir);

                    string bgpath = GetNodeVal(stageSelectNode, "Background");
                    if (bgpath != "") this.StageSelectBackground = FilePath.FromRelative(bgpath, this.BaseDir);

                    XElement bossFrame = stageSelectNode.Element("BossFrame");
                    if (bossFrame != null)
                    {
                        XElement bossSprite = bossFrame.Element("Sprite");
                        if (bossSprite != null) this.bossFrameSprite = Sprite.FromXml(bossSprite, this.BaseDir);
                    }

                    string spaceX = GetNodeAttr(stageSelectNode, "Spacing", "x");
                    if (spaceX != "")
                    {
                        int x = BossSpacingHorizontal;
                        if (int.TryParse(spaceX, out x)) BossSpacingHorizontal = x;
                    }

                    string spaceY = GetNodeAttr(stageSelectNode, "Spacing", "y");
                    if (spaceY != "")
                    {
                        int y = BossSpacingVertical;
                        if (int.TryParse(spaceY, out y)) BossSpacingVertical = y;
                    }

                    int bossIndex = 0;
                    foreach (XElement bossNode in stageSelectNode.Elements("Boss"))
                    {
                        XAttribute slotAttr = bossNode.Attribute("slot");
                        int slot = -1;
                        if (slotAttr != null) int.TryParse(slotAttr.Value, out slot);

                        BossInfo info = this.bosses[bossIndex];
                        info.Slot = slot;
                        info.Name = GetNodeAttr(bossNode, "name");
                        var portrait = GetNodeAttr(bossNode, "portrait");
                        if (portrait != null) info.PortraitPath = FilePath.FromRelative(portrait, this.BaseDir);
                        info.Stage = GetNodeAttr(bossNode, "stage");
                        bossIndex++;
                    }
                }

                XElement pauseNode = reader.Element("PauseScreen");

                foreach (XElement entityNode in reader.Elements("Entities"))
                {
                    if (!string.IsNullOrEmpty(entityNode.Value.Trim())) entityFiles.Add(entityNode.Value);
                }
            }
            catch
            {
                Name = Author = GameFile = BaseDir = null;
                StageSelectBackground = StageSelectChangeSound = StageSelectMusic =
                     PauseChangeSound = PauseScreenBackground = null;
                PauseLivesPosition = Point.Empty;
                PauseSound = null;
                bosses.Clear();
                bossFrameSprite = null;
                entityFiles.Clear();

                throw;
            }

            this.Dirty = false;
        }

        private string GetNodeVal(XElement parent, string name)
        {
            var node = parent.Element(name);
            if (node == null) return "";
            return node.Value;
        }

        private string GetNodeAttr(XElement parent, string nodename, string attrname)
        {
            return GetNodeAttr(parent.Element(nodename), attrname);
        }

        private string GetNodeAttr(XElement node, string attrname)
        {
            if (node == null) return "";
            var attr = node.Attribute(attrname);
            if (attr == null) return "";
            return attr.Value;
        }

        public void AddStage(string name, string tilesetPath)
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
            this.stages.Add(info);

            this.Save(); // need to save the reference to the new stage

            if (StageAdded != null) StageAdded(stage);
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(this.GameFile)) return;

            XmlTextWriter writer = new XmlTextWriter(this.GameFile, Encoding.Default);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteStartElement("Game");
            if (!string.IsNullOrEmpty(this.Name)) writer.WriteAttributeString("name", this.Name);
            if (!string.IsNullOrEmpty(this.Author)) writer.WriteAttributeString("author", this.Author);
            writer.WriteAttributeString("editor_version", System.Windows.Forms.Application.ProductVersion);

            writer.WriteStartElement("Size");
            writer.WriteAttributeString("x", this.ScreenWidth.ToString());
            writer.WriteAttributeString("y", this.ScreenHeight.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Stages");
            foreach (var info in stages)
            {
                writer.WriteStartElement("Stage");
                writer.WriteAttributeString("name", info.Name);
                writer.WriteAttributeString("path", info.StagePath.Relative);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // Stages

            writer.WriteStartElement("StageSelect");

            if (this.StageSelectMusic != null) writer.WriteElementString("Music", this.StageSelectMusic.Relative);
            if (this.StageSelectChangeSound != null)
            {
                writer.WriteStartElement("ChangeSound");
                writer.WriteAttributeString("path", this.StageSelectChangeSound.Relative);
                writer.WriteEndElement(); // ChangeSound
            }
            if (this.StageSelectBackground != null) writer.WriteElementString("Background", this.StageSelectBackground.Relative);

            if (this.bossFrameSprite != null)
            {
                writer.WriteStartElement("BossFrame");
                this.bossFrameSprite.WriteTo(writer);
                writer.WriteEndElement(); // BossFrame
            }

            foreach (BossInfo boss in this.bosses)
            {
                writer.WriteStartElement("Boss");
                if (boss.Slot >= 0) writer.WriteAttributeString("slot", boss.Slot.ToString());
                if (!string.IsNullOrEmpty(boss.Name)) writer.WriteAttributeString("name", boss.Name);
                if (boss.PortraitPath != null && !string.IsNullOrEmpty(boss.PortraitPath.Relative)) writer.WriteElementString("Portrait", boss.PortraitPath.Relative);
                if (!string.IsNullOrEmpty(boss.Stage)) writer.WriteAttributeString("stage", boss.Stage);
                writer.WriteEndElement();
            }

            writer.WriteEndElement(); // StageSelect

            foreach (string entityFile in this.entityFiles)
            {
                writer.WriteElementString("Entities", entityFile);
            }

            writer.WriteEndElement(); // Game
            
            writer.Close();
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
