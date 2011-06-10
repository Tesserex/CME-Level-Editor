using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public partial class EntityButton : Panel
    {
        private int lastFrame = -1;
        private Sprite sprite;
        private PictureBox spritePict;

        public EntityButton(Entity entity)
        {
            InitializeComponent();

            sprite = entity.MainSprite;

            spritePict = new PictureBox();
            spritePict.Height = sprite.Height;
            spritePict.Width = sprite.Width;

            BackColor = Color.Transparent;
            Width = spritePict.Width + 8;
            Height = spritePict.Height + 8;
            Controls.Add(spritePict);
            spritePict.Top = 4;
            spritePict.Left = 4;

            spritePict.Image = sprite[0].CutTile;
            spritePict.Refresh();

            Program.FrameTick += new Action(Program_FrameTick);
            spritePict.Click += new EventHandler(spritePict_Click);
        }

        void spritePict_Click(object sender, EventArgs e)
        {
            this.InvokeOnClick(this, e);
        }

        private void Program_FrameTick()
        {
            if (sprite.CurrentFrame == lastFrame) return;
            lastFrame = sprite.CurrentFrame;

            spritePict.Image = sprite[sprite.CurrentFrame].CutTile;
            spritePict.Refresh();
        }
    }
}
