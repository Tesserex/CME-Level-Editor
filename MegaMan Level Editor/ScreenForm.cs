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
    public partial class ScreenForm : Form
    {
        private static Brush blockBrush = new SolidBrush(Color.FromArgb(160, Color.OrangeRed));
        private static Brush ladderBrush = new SolidBrush(Color.FromArgb(160, Color.Yellow));

        private MegaMan.Screen myScreen;

        private ITileBrush currentBrush = null;
        
        private Bitmap tileLayer = null;
        private Bitmap gridLayer = null;
        private Bitmap blockLayer = null;
        private Bitmap mouseLayer = null;
        private Bitmap masterImage = null;

        private bool drawing = false;
        private bool drawGrid;
        private bool drawTiles;
        private bool drawBlock;

        private Pen highlightPen = new Pen(Color.Green, 2);

        #region Properties
        public Map Map { get; private set; }

        public string Path { get; private set; }

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
        #endregion

        public ScreenForm()
        {
            InitializeComponent();

            DrawGrid = false;
            DrawTiles = true;
            DrawBlock = false;

            screenImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.screenImage_MouseMove);
            screenImage.MouseDown += new System.Windows.Forms.MouseEventHandler(screenImage_MouseDown);
            screenImage.MouseUp += new MouseEventHandler(screenImage_MouseUp);

            this.FormClosing += new FormClosingEventHandler(MapForm_FormClosing);

            Program.FrameTick += new Action(Program_FrameTick);

            MainForm.Instance.BrushChanged += new BrushChangedHandler(parent_BrushChanged);
        }

        void parent_BrushChanged(BrushChangedEventArgs e)
        {
            SetBrush(e.Brush);
        }

        void Program_FrameTick()
        {
            ReDrawTiles();
            ReDrawMaster();
        }

        private void MapForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        public void SetScreen(Map map, MegaMan.Screen screen)
        {
            Map = map;
            myScreen = screen;

            InitLayer(ref masterImage);
            InitLayer(ref tileLayer);
            InitLayer(ref mouseLayer);
            InitLayer(ref gridLayer);
            InitLayer(ref blockLayer);

            ReDrawTiles();
            ReDrawBlocking();

            ReDrawMaster();
            
            // draw grid
            using (Graphics g = Graphics.FromImage(gridLayer))
            {
                for (int x = 0; x < screen.Width; x++)
                {
                    int tx = x * screen.Tileset.TileSize;
                    g.DrawLine(Pens.GreenYellow, tx, 0, tx, screen.PixelHeight);
                }

                for (int y = 0; y < screen.Height; y++)
                {
                    int ty = y * screen.Tileset.TileSize;
                    g.DrawLine(Pens.GreenYellow, 0, ty, screen.PixelWidth, ty);
                }
            }

            Center();
        }

        private void SetBrush(ITileBrush brush)
        {
            currentBrush = brush;
        }

        public void Undo()
        {
            
        }

        public void Redo()
        {
            
        }

        #region Private Methods
        private void ReDrawTiles()
        {
            using (Graphics g = Graphics.FromImage(tileLayer))
            {
                myScreen.Draw(g, 0, 0, myScreen.PixelWidth, myScreen.PixelHeight);
            }
        }

        private void ReDrawBlocking()
        {
            using (Graphics g = Graphics.FromImage(blockLayer))
            {
                for (int y = 0; y < myScreen.Height; y++)
                {
                    for (int x = 0; x < myScreen.Width; x++)
                    {
                        if (myScreen.TileAt(x, y).Properties.Blocking)
                        {
                            g.FillRectangle(blockBrush, x * myScreen.Tileset.TileSize, y * myScreen.Tileset.TileSize, myScreen.Tileset.TileSize, myScreen.Tileset.TileSize);
                        }
                        if (myScreen.TileAt(x, y).Properties.Climbable)
                        {
                            g.FillRectangle(ladderBrush, x * myScreen.Tileset.TileSize, y * myScreen.Tileset.TileSize, myScreen.Tileset.TileSize, myScreen.Tileset.TileSize);
                        }
                    }
                }
            }
        }

        private void InitLayer(ref Bitmap layer)
        {
            if (layer != null) layer.Dispose();
            layer = new Bitmap(myScreen.Width * myScreen.Tileset.TileSize, myScreen.Height * myScreen.Tileset.TileSize);
        }

        private void ReDrawMaster()
        {
            if (myScreen == null) return;

            using (Graphics g = Graphics.FromImage(masterImage))
            {
                g.Clear(Color.Black);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                if (DrawTiles && tileLayer != null) g.DrawImageUnscaled(tileLayer, 0, 0);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                if (DrawBlock && blockLayer != null) g.DrawImageUnscaled(blockLayer, 0, 0);
                if (DrawGrid && gridLayer != null) g.DrawImageUnscaled(gridLayer, 0, 0);
                g.DrawImageUnscaled(mouseLayer, 0, 0);
            }
            screenImage.Image = masterImage;
            screenImage.Width = masterImage.Width;
            screenImage.Height = masterImage.Height;
            screenImage.Refresh();
        }

        private void Center()
        {
            int newleft = (sizingPanel.Width - screenImage.Width) / 2, newtop = (sizingPanel.Height - screenImage.Height) / 2;
            if (newleft < 0) newleft = 0;
            if (newtop < 0) newtop = 0;
            screenImage.Top = newtop;
            screenImage.Left = newleft;
        }
        #endregion Private Methods

        #region Form Event Handlers
        private void mapForm_Resize(object sender, EventArgs e)
        {
            Center();
        }
        #endregion Form Event Handlers

        #region screenImage Event Handlers
        private void screenImage_MouseDown(object sender, MouseEventArgs e)
        {
            drawing = true;
            DrawTile(e.X / myScreen.Tileset.TileSize, e.Y / myScreen.Tileset.TileSize);
        }

        private void DrawTile(int x, int y)
        {
            if (!drawing || currentBrush == null) return;
            currentBrush.DrawOn(this.myScreen, x, y);
            ReDrawTiles();
            ReDrawBlocking();
            ReDrawMaster();
        }

        private void screenImage_MouseUp(object sender, MouseEventArgs e)
        {
            drawing = false;
        }

        private void screenImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseLayer == null) return;

            int tx = (e.X / myScreen.Tileset.TileSize) * myScreen.Tileset.TileSize;
            int ty = (e.Y / myScreen.Tileset.TileSize) * myScreen.Tileset.TileSize;

            using (Graphics g = Graphics.FromImage(mouseLayer))
            {
                g.Clear(Color.Transparent);
                g.DrawRectangle(highlightPen, tx, ty, myScreen.Tileset.TileSize - 1, myScreen.Tileset.TileSize - 1);
            }

            DrawTile(e.Location.X / myScreen.Tileset.TileSize, e.Y / myScreen.Tileset.TileSize);
            ReDrawMaster();
        }
        #endregion screenImage Event Handlers
    }
}
