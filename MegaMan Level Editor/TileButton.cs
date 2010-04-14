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
        private static Pen highlightPen = new Pen(Color.Orange, 2);

        private Tile tile;
        private bool hover;

        public TileButton(Tile tile)
        {
            this.tile = tile;
            this.Margin = new Padding(0);
            this.Padding = new Padding(2);
            this.Text = "";
            this.AutoSize = false;

            if (tile != null)
            {
                this.Width = tile.Sprite.Width;
                this.Height = tile.Sprite.Height;
            }

            Program.FrameTick += new Action(Program_FrameTick);
            this.MouseEnter += new EventHandler(EnableHover);
            this.MouseLeave += new EventHandler(DisableHover);
        }

        void DisableHover(object sender, EventArgs e)
        {
            this.hover = false;
            Invalidate();
        }

        private void EnableHover(object sender, EventArgs e)
        {
            this.hover = true;
            Invalidate();
        }

        private void Program_FrameTick()
        {
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (tile == null) e.Graphics.FillRectangle(Brushes.Black, e.ClipRectangle);
            else tile.Draw(e.Graphics, 0, 0);
            if (hover) e.Graphics.DrawRectangle(highlightPen, e.ClipRectangle);
            base.OnPaint(e);
        }
    }
}
