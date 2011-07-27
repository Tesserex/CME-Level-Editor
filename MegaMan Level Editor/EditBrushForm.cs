using System;
using System.Drawing;
using System.Windows.Forms;
using MegaMan;

namespace MegaMan_Level_Editor {
    public partial class EditBrushForm : Form
    {
        private TileBrush Brush;
        private readonly Tileset Tileset;

        public EditBrushForm(TileBrush brush, Tileset tileset) {
            InitializeComponent();
            Tileset = tileset;
            Brush = brush;

            brushPict.MouseDown += brushPict_MouseDown;

            Reset(2, 2);
        }

        void brushPict_MouseDown(object sender, MouseEventArgs e) {
            if (MainForm.Instance.CurrentBrush == null) return;

            int tx = e.X / Tileset.TileSize;
            int ty = e.Y / Tileset.TileSize;

            foreach (TileBrushCell cell in MainForm.Instance.CurrentBrush.Cells())
            {
                Brush.AddTile(cell.tile, cell.x + tx, cell.y + ty);
            }

            ReDraw();
        }

        private void ReDraw() {
            using (Graphics g = Graphics.FromImage(brushPict.Image)) {
                g.Clear(Color.Black);
                Brush.DrawOn(g, 0, 0);
            }
            brushPict.Refresh();
        }

        private void resetButton_Click(object sender, EventArgs e) {
            int width;
            int height;
            if (!int.TryParse(widthBox.Text, out width)) return;
            if (!int.TryParse(heightBox.Text, out height)) return;

            Reset(width, height);
        }

        private void Reset(int width, int height)
        {
            Brush.Reset(width, height);

            if (brushPict.Image != null) brushPict.Image.Dispose();

            brushPict.Image = new Bitmap(width * Tileset.TileSize, height * Tileset.TileSize);
            brushPict.Size = brushPict.Image.Size;
            ReDraw();
        }
    }
}
