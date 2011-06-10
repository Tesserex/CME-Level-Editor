using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan;
using System.Xml.Linq;
using System.Drawing;

namespace MegaMan_Level_Editor
{
    public class Entity
    {
        // for now, this class is only used for placement
        public string Name { get; private set; }
        public Sprite MainSprite { get; private set; }

        public Entity(XElement xmlNode, string basePath)
        {
            Name = xmlNode.RequireAttribute("name").Value;

            // find the primary sprite
            var spriteNode = xmlNode.Element("Sprite");

            if (spriteNode != null)
            {
                // if it doesn't have a tilesheet, use the first for the entity
                var sheetAttr = spriteNode.Attribute("tilesheet");
                if (sheetAttr == null)
                {
                    var sheetNode = xmlNode.Element("Tilesheet");
                    string sheetPath = System.IO.Path.Combine(basePath, sheetNode.Value);
                    var sheet = Bitmap.FromFile(sheetPath);
                    MainSprite = Sprite.FromXml(spriteNode, sheet);
                }
                else
                {
                    MainSprite = Sprite.FromXml(spriteNode, basePath);
                }

                MainSprite.Play();
                Program.FrameTick += new Action(Program_FrameTick);
            }
        }

        void Program_FrameTick()
        {
            MainSprite.Update();
        }
    }
}
