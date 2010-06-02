using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace MegaMan_Level_Editor
{
    /* *
     * ScreenDrawingSurface - Draw a screen onto one of these. 
     * Multiple screen surfaces show an entire map in one window
     * */
    public class ScreenDrawingSurface
    {
        private static Brush blockBrush = new SolidBrush(Color.FromArgb(160, Color.OrangeRed));
        private static Brush ladderBrush = new SolidBrush(Color.FromArgb(160, Color.Yellow));
        private static Pen highlightPen = new Pen(Color.Green, 2);

        private Bitmap tileLayer = null;
        private Bitmap gridLayer = null;
        private Bitmap blockLayer = null;
        private Bitmap mouseLayer = null;
        private Bitmap masterImage = null;

        public bool Drawing { get; private set; }
        private bool drawGrid;
        private bool drawTiles;
        private bool drawBlock;

        private bool active = false;
        public bool Placed { get; set; }

        public PictureBox screenImage;

        public bool DrawGrid
        {
            get { return drawGrid; }
            set
            {
                drawGrid = value;
                ReDrawMaster();
            }
        }

        public bool DrawTiles
        {
            get { return drawTiles; }
            set
            {
                drawTiles = value;
                ReDrawMaster();
            }
        }

        public bool DrawBlock
        {
            get { return drawBlock; }
            set
            {
                drawBlock = value;
                ReDrawMaster();
            }
        }

        public MegaMan.Screen Screen { get; private set; }

        private StageForm parent;

        public ScreenDrawingSurface(MegaMan.Screen screen, StageForm parent)
        {
            this.Screen = screen;
            this.parent = parent;

            AddPictureBox();
            AddScreenImageHandlers();

            BuildLayers();

            DrawGrid = false;
            DrawTiles = true;
            DrawBlock = false;

            this.Screen.Resized += (w, h) => this.ResizeLayers();
        }

        //*****************
        // Event Handlers *
        //*****************

        private void AddScreenImageHandlers()
        {
            screenImage.MouseLeave += (s, ev) => { this.active = false; ReDrawTiles(); ReDrawMaster(); };
            screenImage.MouseEnter += (s, ev) => { this.active = true; ReDrawTiles(); ReDrawMaster(); };

            screenImage.MouseMove += new MouseEventHandler(screenImage_MouseMove);
            screenImage.MouseDown += new MouseEventHandler(screenImage_MouseDown);
            screenImage.MouseUp += new MouseEventHandler(screenImage_MouseUp);
            //                screenImage.MouseHover += (s, ev) => { this.active = true; };
            //                screenImage.MouseCaptureChanged += (s, ev) => { this.active = !this.active; };
        }

        private void screenImage_MouseDown(object sender, MouseEventArgs e)
        {
            Drawing = true;
            DrawTile(e.X / Screen.Tileset.TileSize, e.Y / Screen.Tileset.TileSize);
        }

        private void screenImage_MouseUp(object sender, MouseEventArgs e)
        {
            Drawing = false;
        }

        private void screenImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseLayer == null) return;

            int tx = (e.X / Screen.Tileset.TileSize) * Screen.Tileset.TileSize;
            int ty = (e.Y / Screen.Tileset.TileSize) * Screen.Tileset.TileSize;

            using (Graphics g = Graphics.FromImage(mouseLayer))
            {
                g.Clear(Color.Transparent);
                g.DrawRectangle(highlightPen, tx, ty, Screen.Tileset.TileSize - 1, Screen.Tileset.TileSize - 1);
            }

            DrawTile(e.Location.X / Screen.Tileset.TileSize, e.Y / Screen.Tileset.TileSize);
            ReDrawMaster();
        }


        private void AddPictureBox()
        {
            this.screenImage = new System.Windows.Forms.PictureBox();
            this.screenImage.BackColor = System.Drawing.SystemColors.Control;
            this.screenImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.screenImage.Location = new System.Drawing.Point(0, 0);

            parent.sizingPanel.Controls.Add(this.screenImage);
        }

        public void ReDrawAll()
        {
            ReDrawTiles();
            ReDrawBlocking();
            ReDrawMaster();
            ReDrawGrid();
        }

        private void ReDrawTiles()
        {
            using (Graphics g = Graphics.FromImage(tileLayer))
            {
                if (active)
                    Screen.Draw(g, 0, 0, Screen.PixelWidth, Screen.PixelHeight);
                else
                {
                    Image gray;
                    using (Image img = new Bitmap(Screen.PixelWidth, Screen.PixelHeight))
                    {
                        using (Graphics img_g = Graphics.FromImage(img))
                        {
                            Screen.Draw(img_g, 0, 0, Screen.PixelWidth, Screen.PixelHeight);
                        }
                        gray = ConvertToGrayscale(img);
                    }
                    g.DrawImage(gray, 0, 0);
                    gray.Dispose();
                }
            }
        }

        private Image ConvertToGrayscale(Image source)
        {
            var bitmapSource = new Bitmap(source);

            Bitmap bm = new Bitmap(source.Width, source.Height);
            for (int y = 0; y < bm.Height; y++)
            {
                for (int x = 0; x < bm.Width; x++)
                {
                    Color c = bitmapSource.GetPixel(x, y);
                    int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
                }
            }

            return (Image)bm;
        }

        private void ReDrawBlocking()
        {
            using (Graphics g = Graphics.FromImage(blockLayer))
            {
                for (int y = 0; y < Screen.Height; y++)
                {
                    for (int x = 0; x < Screen.Width; x++)
                    {
                        if (Screen.TileAt(x, y).Properties.Blocking)
                        {
                            g.FillRectangle(blockBrush, x * Screen.Tileset.TileSize, y * Screen.Tileset.TileSize, Screen.Tileset.TileSize, Screen.Tileset.TileSize);
                        }

                        if (Screen.TileAt(x, y).Properties.Climbable)
                        {
                            g.FillRectangle(ladderBrush, x * Screen.Tileset.TileSize, y * Screen.Tileset.TileSize, Screen.Tileset.TileSize, Screen.Tileset.TileSize);
                        }
                    }
                }
            }
        }

        private void ReDrawGrid()
        {
            using (Graphics g = Graphics.FromImage(gridLayer))
            {
                for (int x = 0; x < Screen.Width; x++)
                {
                    int tx = x * Screen.Tileset.TileSize;
                    g.DrawLine(Pens.GreenYellow, tx, 0, tx, Screen.PixelHeight);
                }

                for (int y = 0; y < Screen.Height; y++)
                {
                    int ty = y * Screen.Tileset.TileSize;
                    g.DrawLine(Pens.GreenYellow, 0, ty, Screen.PixelWidth, ty);
                }
            }
        }

        private void ReDrawMaster()
        {
            if (Screen == null)
                return;

            using (Graphics g = Graphics.FromImage(masterImage))
            {
                g.Clear(Color.Black);

                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                if (DrawTiles && tileLayer != null)
                    g.DrawImageUnscaled(tileLayer, 0, 0);

                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                if (DrawBlock && blockLayer != null)
                    g.DrawImageUnscaled(blockLayer, 0, 0);

                if (DrawGrid && gridLayer != null)
                    g.DrawImageUnscaled(gridLayer, 0, 0);

                g.DrawImageUnscaled(mouseLayer, 0, 0);
            }

            screenImage.Image = masterImage;
            screenImage.Width = masterImage.Width;
            screenImage.Height = masterImage.Height;
            screenImage.Refresh();
        }

        private void DrawTile(int x, int y)
        {
            parent.DrawTile(x, y, this);
        }


        private void InitLayer(ref Bitmap layer)
        {
            if (layer != null) layer.Dispose();
            ResizeLayer(ref layer);
        }

        private void ResizeLayer(ref Bitmap layer)
        {
            layer = new Bitmap(Screen.Width * Screen.Tileset.TileSize, Screen.Height * Screen.Tileset.TileSize);
        }

        private void BuildLayers()
        {
            InitLayer(ref tileLayer);
            InitLayer(ref gridLayer);
            InitLayer(ref blockLayer);
            InitLayer(ref mouseLayer);
            InitLayer(ref masterImage);
        }

        private void ResizeLayers()
        {
            ResizeLayer(ref tileLayer);
            ResizeLayer(ref gridLayer);
            ResizeLayer(ref blockLayer);
            ResizeLayer(ref mouseLayer);
            ResizeLayer(ref masterImage);
        }
    }
}
