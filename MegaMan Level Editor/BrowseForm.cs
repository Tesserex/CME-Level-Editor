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
    public partial class BrowseForm : Form
    {
        private Map map;
        private Bitmap screenImage;
        private Bitmap mainImage;
        private float zoom = 0.5f;
        private const int spacing = 10;
        private float hotX;
        private string hotId;

        private Pen hotPen = new Pen(Brushes.Green, 3);

        public event Action<Map, string> ScreenSelected;

        public BrowseForm()
        {
            InitializeComponent();
        }

        public void SetMap(Map map)
        {
            this.map = map;
            float width = 0, height = 0;
            foreach (MegaMan.Screen screen in map.Screens.Values)
            {
                width += screen.Width * map.Tileset.TileSize * zoom;
                width += spacing;

                height = Math.Max(height, screen.Height * map.Tileset.TileSize * zoom);
            }
            width -= spacing;

            if (screenImage != null) screenImage.Dispose();
            screenImage = new Bitmap((int)width, (int)height);

            using (Graphics g = Graphics.FromImage(screenImage))
            {
                g.Clear(Color.Transparent);
                float x = 0;
                foreach (MegaMan.Screen screen in map.Screens.Values)
                {
                    Bitmap temp = new Bitmap(screen.Width * map.Tileset.TileSize, screen.Height * map.Tileset.TileSize);
                    using (Graphics t = Graphics.FromImage(temp)) screen.Draw(t, 0, 0, screen.PixelWidth, screen.PixelHeight);
                    g.DrawImage(temp, x, 0, temp.Width * zoom, temp.Height * zoom);
                    x += temp.Width*zoom + spacing;
                    temp.Dispose();
                }
            }

            ReDraw();
        }

        private void ReDraw()
        {
            if (mainImage != null) mainImage.Dispose();
            mainImage = new Bitmap(screenImage.Width, screenImage.Height);
            screensBox.Image = mainImage;
            screensBox.Size = mainImage.Size;

            using (Graphics g = Graphics.FromImage(mainImage))
            {
                g.DrawImage(screenImage, 0, 0);

                if (hotId != null) g.DrawRectangle(hotPen, hotX, 0, map.Screens[hotId].Width * map.Tileset.TileSize * zoom, map.Screens[hotId].Height * map.Tileset.TileSize * zoom);
            }

            screensBox.Refresh();
        }

        private void screensBox_MouseMove(object sender, MouseEventArgs e)
        {
            float x = 0;
            foreach (string mapkey in map.Screens.Keys)
            {
                float max_x = x + map.Screens[mapkey].Width * map.Tileset.TileSize * zoom;
                if (e.X < max_x)
                {
                    hotX = x;
                    hotId = mapkey;
                    break;
                }
                x = max_x + spacing;
            }
            ReDraw();
        }

        private void screensBox_Click(object sender, EventArgs e)
        {
            if (ScreenSelected != null) ScreenSelected(map, hotId);
            this.Hide();
        }
    }
}
