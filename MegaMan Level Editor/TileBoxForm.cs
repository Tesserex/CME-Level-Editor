using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public partial class TileBoxForm : Form
    {
        private Tileset tileset;
        private Bitmap image;
        private Pen highlightPen = new Pen(Color.Green, 2);
        private Pen hotPen = new Pen(Color.Orange, 2);
        private int hot;

        private bool drawBlock;

        #region Properties
        public bool DrawBlock
        {
            get { return drawBlock; }
            set
            {
                drawBlock = value;
            }
        }

        public int Selected { get; private set; }

        public event Action SelectedChanged;

        public Tileset Tileset
        {
            get { return tileset; }
            set
            {
                tileset = value;
                if (image != null) image.Dispose();
                image = new Bitmap(tileset.TileSize * tileset.Count, tileset.TileSize);
                tilesetImage.Image = image;
                tilesetImage.Width = image.Width;
                tilesetImage.Height = image.Height;
                ReDraw();
            }
        }
        #endregion

        public TileBoxForm()
        {
            InitializeComponent();

            Program.FrameTick += new Action(ReDraw);
            tilesetImage.MouseClick += new MouseEventHandler(tilesetImage_MouseClick);
        }

        #region Private Methods
        private void ReDraw()
        {
            if (image == null || tileset == null) return;

            foreach (Tile tile in tileset)
            {
                tile.Sprite.Update();
            }

            using (Graphics g = Graphics.FromImage(image))
            {
                for (int i = 0, x = 0; i < tileset.Count; i++, x+= tileset.TileSize)
                {
                    tileset[i].Draw(g, x, 0);
                }
                g.DrawRectangle(hotPen, hot * tileset.TileSize, 0, tileset.TileSize, tileset.TileSize);
                g.DrawRectangle(highlightPen, Selected * tileset.TileSize, 0, tileset.TileSize, tileset.TileSize);
            }

            tilesetImage.Refresh();
        }

        private void tilesetImage_MouseClick(object sender, MouseEventArgs e)
        {
            Selected = hot;
            if (SelectedChanged != null) SelectedChanged();
        }

        private void tilesetImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (tileset != null)
            {
                hot = e.X / tileset.TileSize;
                ReDraw();
            }
        }
        #endregion
    }
}
