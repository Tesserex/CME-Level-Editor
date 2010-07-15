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
    public partial class BrushForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private List<ITileBrush> brushes = new List<ITileBrush>();
        private Tileset Tileset;

        public BrushForm()
        {
            InitializeComponent();
            brushes = new List<ITileBrush>();
        }

        public void ChangeTileset(Tileset tileset)
        {
            Tileset = tileset;
        }

        public event BrushChangedHandler BrushChanged;

        private void buttonNewBrush_Click(object sender, EventArgs e)
        {
            TileBrush brush = new TileBrush(2, 2);
            EditBrushForm brushForm = new EditBrushForm(brush, Tileset);
            brushForm.Show();

            brushForm.FormClosed += (s, ev) =>
            {
                brushes.Add(brush);

                PictureBox brushPict = new PictureBox();

                if (Tileset != null)
                {
                    brushPict.Image = new Bitmap(brush.Width * Tileset.TileSize, brush.Height * Tileset.TileSize);
                    brushPict.Size = brushPict.Image.Size;
                    using (Graphics g = Graphics.FromImage(brushPict.Image))
                    {
                        brush.DrawOn(g, 0, 0);
                    }
                }

                brushPict.Click += (snd, args) => ChangeBrush(brush);

                brushPanel.Controls.Add(brushPict);
            };
        }

        private void ChangeBrush(ITileBrush brush)
        {
            BrushChangedEventArgs args = new BrushChangedEventArgs(brush);
            if (BrushChanged != null) BrushChanged(args);
        }
    }

    public class BrushChangedEventArgs : EventArgs
    {
        public ITileBrush Brush { get; private set; }

        public BrushChangedEventArgs(ITileBrush brush)
        {
            Brush = brush;
        }
    }

    public delegate void BrushChangedHandler(BrushChangedEventArgs e);
}
