using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public class TileButton : ToolStripLabel
    {
        private Tile tile;
        private bool hover;

        public TileButton(Tile tile)
        {
            this.tile = tile;
            this.Margin = new Padding(0);
            this.Padding = new Padding(0);
            this.Text = "";
            this.AutoSize = false;
            this.Width = tile.Sprite.Width;
            this.Height = tile.Sprite.Height;

            Program.FrameTick += new Action(Program_FrameTick);
            this.MouseEnter += new EventHandler(EnableHover);
            this.MouseLeave += new EventHandler(DisableHover);
        }

        void DisableHover(object sender, EventArgs e)
        {
            this.hover = false;
        }

        private void EnableHover(object sender, EventArgs e)
        {
            this.hover = true;
        }

        private void Program_FrameTick()
        {
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            tile.Draw(e.Graphics, 0, 0);
            base.OnPaint(e);
        }
    }
}
