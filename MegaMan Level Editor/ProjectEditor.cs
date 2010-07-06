﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan_Level_Editor
{
    public class StageInfo
    {
        public int Slot { get; set; }
        public string Name { get; set; }
        public FilePath PortraitPath { get; set; }
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

        private List<StageInfo> stages = new List<StageInfo>(8);
        private List<string> entityFiles = new List<string>();
        private Sprite bossFrameSprite;

        public string Name { get; private set; }
        public string Author { get; private set; }
        public string GameFile { get; private set; }
        public string BaseDir { get; private set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public FilePath StageSelectMusic { get; private set; }
        public FilePath StageSelectBackground { get; private set; }
        public FilePath StageSelectChangeSound { get; private set; }
        public int BossSpacingHorizontal { get; set; }
        public int BossSpacingVertical { get; set; }

        public event Action<bool> DirtyChanged;

        public static ProjectEditor CreateNew()
        {
            return new ProjectEditor();
        }

        public static ProjectEditor FromFile(string path)
        {
            var p = new ProjectEditor();
            p.Load(path);
            return p;
        }

        private ProjectEditor()
        {
            // sensible defaults where possible
            ScreenWidth = 256;
            ScreenHeight = 224;
            BossSpacingHorizontal = 24;
            BossSpacingVertical = 16;
        }

        private void Load(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("The project file does not exist: " + path);

            GameFile = path;
            BaseDir = System.IO.Path.GetDirectoryName(path);
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

            XElement stageNode = reader.Element("StageSelect");
            if (stageNode != null)
            {
                XElement musicNode = stageNode.Element("Music");
                if (musicNode != null) this.StageSelectMusic = FilePath.FromRelative(musicNode.Value, this.BaseDir);

                string changepath = GetNodeAttr(stageNode, "ChangeSound", "path");
                if (changepath != "") this.StageSelectChangeSound = FilePath.FromRelative(changepath, this.BaseDir);

                string bgpath = GetNodeVal(stageNode, "Background");
                if (bgpath != "") this.StageSelectBackground = FilePath.FromRelative(bgpath, this.BaseDir);

                XElement bossFrame = stageNode.Element("BossFrame");
                if (bossFrame != null)
                {
                    this.bossFrameSprite = Sprite.FromXml(bossFrame, this.BaseDir);
                }

                string spaceX = GetNodeAttr(stageNode, "Spacing", "x");
                if (spaceX != "")
                {
                    int x = BossSpacingHorizontal;
                    if (int.TryParse(spaceX, out x)) BossSpacingHorizontal = x;
                }

                string spaceY = GetNodeAttr(stageNode, "Spacing", "y");
                if (spaceY != "")
                {
                    int y = BossSpacingVertical;
                    if (int.TryParse(spaceY, out y)) BossSpacingVertical = y;
                }

                foreach (XElement bossNode in stageNode.Elements("Boss"))
                {
                    XAttribute slotAttr = bossNode.Attribute("slot");
                    int slot = -1;
                    if (slotAttr != null) int.TryParse(slotAttr.Value, out slot);

                    StageInfo info = new StageInfo();
                    info.Slot = slot;
                    info.Name = GetNodeVal(bossNode, "Name");
                    info.PortraitPath = FilePath.FromRelative(GetNodeVal(bossNode, "Portrait"), this.BaseDir);
                    info.StagePath = FilePath.FromRelative(GetNodeVal(bossNode, "Stage"), this.BaseDir);

                    this.stages.Add(info);
                }
            }

            XElement pauseNode = reader.Element("PauseScreen");

            foreach (XElement entityNode in reader.Elements("Entities"))
            {
                if (!string.IsNullOrEmpty(entityNode.Value.Trim())) entityFiles.Add(entityNode.Value);
            }
        }

        private string GetNodeVal(XElement parent, string name)
        {
            var node = parent.Element(name);
            if (node == null) return "";
            return node.Value;
        }

        private string GetNodeAttr(XElement parent, string nodename, string attrname)
        {
            var node = parent.Element(nodename);
            if (node == null) return "";
            var attr = node.Attribute(attrname);
            if (attr == null) return "";
            return attr.Value;
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(this.GameFile)) SaveAs();
        }

        public void SaveAs()
        {

        }

        private void SaveTo(string file)
        {
            this.GameFile = file;
            this.BaseDir = Path.GetDirectoryName(file);

            XmlTextWriter writer = new XmlTextWriter(file, Encoding.Default);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteStartElement("Game");
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("author", this.Author);

            writer.WriteStartElement("Size");
            writer.WriteAttributeString("x", this.ScreenWidth.ToString());
            writer.WriteAttributeString("y", this.ScreenHeight.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("StageSelect");

            writer.WriteElementString("Music", this.StageSelectMusic.Relative);
            writer.WriteStartElement("ChangeSound");
            writer.WriteAttributeString("path", this.StageSelectChangeSound.Relative);
            writer.WriteEndElement(); // ChangeSound
            writer.WriteElementString("Background", this.StageSelectBackground.Relative);

            if (this.bossFrameSprite != null)
            {
                writer.WriteStartElement("BossFrame");
                this.bossFrameSprite.WriteTo(writer);
                writer.WriteEndElement(); // BossFrame
            }

            foreach (StageInfo boss in this.stages)
            {
                writer.WriteStartElement("Boss");
                if (boss.Slot >= 0) writer.WriteAttributeString("slot", boss.Slot.ToString());
                if (!string.IsNullOrEmpty(boss.Name)) writer.WriteElementString("Name", boss.Name);
                if (!string.IsNullOrEmpty(boss.PortraitPath.Relative)) writer.WriteElementString("Portrait", boss.PortraitPath.Relative);
                if (!string.IsNullOrEmpty(boss.StagePath.Relative)) writer.WriteElementString("Stage", boss.StagePath.Relative);
                writer.WriteEndElement();
            }

            writer.WriteEndElement(); // StageSelect

            foreach (string entityFile in this.entityFiles)
            {
                writer.WriteElementString("Entities", entityFile);
            }

            writer.WriteEndElement(); // Game
        }
    }
}