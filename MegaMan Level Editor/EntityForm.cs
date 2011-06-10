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

namespace MegaMan_Level_Editor
{
    public partial class EntityForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public EntityForm()
        {
            InitializeComponent();
            Program.FrameTick += new Action(Program_FrameTick);
        }

        private void Program_FrameTick()
        {
            container.Refresh();
        }

        public void LoadEntities(ProjectEditor project)
        {
            foreach (Entity entity in project.Entities)
            {
                if (entity.MainSprite == null) continue;

                var sprite = entity.MainSprite;

                PictureBox spritePict = new PictureBox();
                spritePict.Image = new Bitmap(sprite.Width, sprite.Height);
                spritePict.Paint += (s, e) => spritePict_Paint(spritePict, sprite);
                spritePict.Size = spritePict.Image.Size;

                Panel border = new Panel();
                border.BackColor = container.BackColor;
                border.Width = spritePict.Width + 8;
                border.Height = spritePict.Height + 8;
                border.Controls.Add(spritePict);
                spritePict.Top = 4;
                spritePict.Left = 4;

                spritePict.Click += (snd, args) =>
                {
                    //ChangeBrush(brush);
                    foreach (Control c in container.Controls) c.BackColor = container.BackColor;
                    border.BackColor = Color.Orange;
                };

                container.Controls.Add(border);
            }
        }

        private void spritePict_Paint(PictureBox pict, Sprite sprite)
        {
            using (var g = Graphics.FromImage(pict.Image))
            {
                g.Clear(Color.Transparent);
                sprite.Draw(g, sprite.HotSpot.X, sprite.HotSpot.Y);
            }
        }

        public void Unload()
        {
            container.Controls.Clear();
        }
    }
}
