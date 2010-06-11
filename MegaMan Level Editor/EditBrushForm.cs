using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MegaMan;

namespace MegaMan_Level_Editor {
    public partial class EditBrushForm : Form {
        private TileBrush brush;
        private Tileset Tileset;

        private ITileBrush cursorBrush;

        public EditBrushForm(TileBrush brush, Tileset tileset) {
            InitializeComponent();
            this.Tileset = tileset;
            this.brush = brush;

            MainForm.Instance.brushForm.BrushChanged += new BrushChangedHandler(Instance_BrushChanged);

            this.brushPict.MouseDown += new MouseEventHandler(brushPict_MouseDown);

            Reset(2, 2);
        }

        void brushPict_MouseDown(object sender, MouseEventArgs e) {
            if (cursorBrush == null) return;

            int tx = e.X / Tileset.TileSize;
            int ty = e.Y / Tileset.TileSize;

            foreach (TileBrushCell cell in cursorBrush.Cells())
            {
                brush.AddTile(cell.tile, cell.x + tx, cell.y + ty);
            }

            ReDraw();
        }

        private void ReDraw() {
            using (Graphics g = Graphics.FromImage(brushPict.Image)) {
                g.Clear(Color.Black);
                brush.DrawOn(g, 0, 0);
            }
            brushPict.Refresh();
        }

        void Instance_BrushChanged(BrushChangedEventArgs e) {
            this.cursorBrush = e.Brush;
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
            this.brush.Reset(width, height);

            if (this.brushPict.Image != null) this.brushPict.Image.Dispose();

            brushPict.Image = new Bitmap(width * Tileset.TileSize, height * Tileset.TileSize);
            brushPict.Size = brushPict.Image.Size;
            ReDraw();
        }
    }
}
