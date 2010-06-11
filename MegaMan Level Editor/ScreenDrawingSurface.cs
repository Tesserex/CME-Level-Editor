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
    public class ScreenDrawingSurface : PictureBox
    {
        private static Brush blockBrush = new SolidBrush(Color.FromArgb(160, Color.OrangeRed));
        private static Brush ladderBrush = new SolidBrush(Color.FromArgb(160, Color.Yellow));
        private static Pen highlightPen = new Pen(Color.Green, 2);

        private ITileBrush currentBrush = null;

        private Bitmap tileLayer = null;
        private Bitmap gridLayer = null;
        private Bitmap blockLayer = null;
        private Bitmap mouseLayer = null;
        private Bitmap masterImage = null;
        private Bitmap grayTiles = null;

        private bool grayDirty = false;

        public bool Drawing { get; private set; }
        private bool drawGrid;
        private bool drawTiles;
        private bool drawBlock;

        private bool active = false;
        public bool Placed { get; set; }

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

        public event EventHandler<ScreenDrawEventArgs> DrawnOn;

        public ScreenDrawingSurface(MegaMan.Screen screen)
        {
            this.Screen = screen;

            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;

            AddScreenImageHandlers();

            BuildLayers();

            DrawGrid = false;
            DrawTiles = true;
            DrawBlock = false;

            this.Screen.Resized += (w, h) => this.ResizeLayers();

            MainForm.Instance.BrushChanged += new BrushChangedHandler(Instance_BrushChanged);

            Program.FrameTick += new Action(Program_FrameTick);

            ReDrawAll();
            DrawGray();
        }

        public void DrawBrush(ITileBrush brush, Point location)
        {
            int tile_x = location.X / this.Screen.Tileset.TileSize;
            int tile_y = location.Y / this.Screen.Tileset.TileSize;

            var previous = brush.DrawOn(Screen, tile_x, tile_y);
            RaiseDrawnOn(tile_x, tile_y, brush, previous);
        }

        public void RaiseDrawnOn(int tile_x, int tile_y, ITileBrush brush, ITileBrush history)
        {
            if (DrawnOn != null) DrawnOn(this, new ScreenDrawEventArgs(tile_x, tile_y, brush, history, this.Screen));

            ReDrawAll();
        }

        void Program_FrameTick()
        {
            if (active)
            {
                ReDrawTiles();
                ReDrawMaster();
            }
        }

        //*****************
        // Event Handlers *
        //*****************

        void Instance_BrushChanged(BrushChangedEventArgs e)
        {
            currentBrush = e.Brush;
        }

        private void AddScreenImageHandlers()
        {
            this.MouseLeave += (s, ev) => { this.active = false; ReDrawMaster(); };
            this.MouseEnter += (s, ev) => { this.active = true; ReDrawMaster(); };

            this.MouseMove += new MouseEventHandler(screenImage_MouseMove);
            this.MouseDown += new MouseEventHandler(screenImage_MouseDown);
            this.MouseUp += new MouseEventHandler(screenImage_MouseUp);
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
            if (mouseLayer == null || currentBrush == null) return;

            int tx = (e.X / Screen.Tileset.TileSize) * Screen.Tileset.TileSize;
            int ty = (e.Y / Screen.Tileset.TileSize) * Screen.Tileset.TileSize;

            using (Graphics g = Graphics.FromImage(mouseLayer))
            {
                g.Clear(Color.Transparent);
                currentBrush.DrawOn(g, tx, ty);
            }

            DrawTile(e.Location.X / Screen.Tileset.TileSize, e.Y / Screen.Tileset.TileSize);
            ReDrawMaster();
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
                Screen.Draw(g, 0, 0, Screen.PixelWidth, Screen.PixelHeight);
            }
            grayDirty = true;
        }

        private void DrawGray()
        {
            if (grayTiles != null) grayTiles.Dispose();
            grayTiles = ConvertToGrayscale(tileLayer);
        }

        private Bitmap ConvertToGrayscale(Bitmap source)
        {
            Bitmap bm = new Bitmap(source.Width, source.Height);
            for (int y = 0; y < bm.Height; y++)
            {
                for (int x = 0; x < bm.Width; x++)
                {
                    Color c = source.GetPixel(x, y);
                    int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
                }
            }

            return bm;
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

                if (DrawTiles)
                {
                    if (active)
                    {
                        if (tileLayer != null) g.DrawImageUnscaled(tileLayer, 0, 0);
                    }
                    else
                    {
                        if (grayDirty) DrawGray();
                        grayDirty = false;
                        if (grayTiles != null) g.DrawImageUnscaled(grayTiles, 0, 0);
                    }
                }

                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                if (DrawBlock && blockLayer != null)
                    g.DrawImageUnscaled(blockLayer, 0, 0);

                if (DrawGrid && gridLayer != null)
                    g.DrawImageUnscaled(gridLayer, 0, 0);

                if (active) g.DrawImageUnscaled(mouseLayer, 0, 0);
            }

            this.Image = masterImage;
            this.Width = masterImage.Width;
            this.Height = masterImage.Height;
            this.Refresh();
        }

        private void DrawTile(int x, int y)
        {
            if (!Drawing || currentBrush == null)
                return;

            var previous = currentBrush.DrawOn(Screen, x, y);
            if (DrawnOn != null) DrawnOn(this, new ScreenDrawEventArgs(x, y, currentBrush, previous, this.Screen));

            ReDrawAll();
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
            InitLayer(ref grayTiles);
            InitLayer(ref gridLayer);
            InitLayer(ref blockLayer);
            InitLayer(ref mouseLayer);
            InitLayer(ref masterImage);
        }

        private void ResizeLayers()
        {
            ResizeLayer(ref tileLayer);
            ResizeLayer(ref grayTiles);
            ResizeLayer(ref gridLayer);
            ResizeLayer(ref blockLayer);
            ResizeLayer(ref mouseLayer);
            ResizeLayer(ref masterImage);
            ReDrawAll();
        }
    }
}
