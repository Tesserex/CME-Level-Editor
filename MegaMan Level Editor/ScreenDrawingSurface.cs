using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public class ScreenDrawEventArgs : EventArgs
    {
        public HistoryAction Action { get; private set; }
        public ScreenDrawingSurface Surface { get; private set; }

        public ScreenDrawEventArgs(HistoryAction action, ScreenDrawingSurface surface)
        {
            Action = action;
            Surface = surface;
        }
    }

    /* *
     * ScreenDrawingSurface - Draw a screen onto one of these. 
     * Multiple screen surfaces show an entire map in one window
     * */
    public class ScreenDrawingSurface : PictureBox
    {
        private static Brush blockBrush = new SolidBrush(Color.FromArgb(160, Color.OrangeRed));
        private static Brush ladderBrush = new SolidBrush(Color.FromArgb(160, Color.Yellow));
        private static Pen highlightPen = new Pen(Color.Green, 2);
        private static Pen passPen = new Pen(Color.Blue, 4);
        private static Pen blockPen = new Pen(Color.Red, 4);

        private Bitmap tileLayer = null;
        private Bitmap gridLayer = null;
        private Bitmap blockLayer = null;
        private Bitmap mouseLayer = null;
        private Bitmap joinLayer = null;
        private Bitmap masterImage = null;
        private Bitmap grayTiles = null;

        private bool grayDirty = false;

        public bool Drawing { get; private set; }

        private bool active = false;
        public bool Placed { get; set; }

        public MegaMan.Screen Screen { get; private set; }

        public event EventHandler<ScreenDrawEventArgs> DrawnOn;
        public event Action JoinChanged;

        public ScreenDrawingSurface(MegaMan.Screen screen)
        {
            this.Screen = screen;

            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;

            BuildLayers();

            this.Screen.Resized += (w, h) => this.ResizeLayers();

            Program.FrameTick += new Action(Program_FrameTick);

            RedrawJoins();
            ReDrawAll();
            DrawGray();

            MainForm.Instance.DrawOptionToggled += ReDrawMaster;
        }

        public void RaiseJoinChange()
        {
            if (JoinChanged != null) JoinChanged();
        }

        public void RaiseDrawnOn(HistoryAction action)
        {
            if (DrawnOn != null) DrawnOn(this, new ScreenDrawEventArgs(action, this));

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

        protected override void OnMouseEnter(EventArgs e)
        {
            this.active = true;
            ReDrawMaster();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.active = false;
            if (mouseLayer != null)
            {
                using (Graphics g = Graphics.FromImage(mouseLayer))
                {
                    g.Clear(Color.Transparent);
                }
            }
            ReDrawMaster();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (MainForm.Instance.CurrentTool == null) return;
            MainForm.Instance.CurrentTool.Click(this, e.Location);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (MainForm.Instance.CurrentTool == null) return;
            MainForm.Instance.CurrentTool.Release(this, e.Location);
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mouseLayer == null || MainForm.Instance.CurrentTool == null) return;

            int tx = (e.X / Screen.Tileset.TileSize) * Screen.Tileset.TileSize;
            int ty = (e.Y / Screen.Tileset.TileSize) * Screen.Tileset.TileSize;

            if (MainForm.Instance.CurrentTool.Icon != null)
            {
                using (Graphics g = Graphics.FromImage(mouseLayer))
                {
                    g.Clear(Color.Transparent);
                    g.DrawImage(MainForm.Instance.CurrentTool.Icon, tx, ty);
                }
            }

            MainForm.Instance.CurrentTool.Move(this, e.Location);
            ReDrawMaster();
            base.OnMouseMove(e);
        }

        public void RedrawJoins()
        {
            if (joinLayer == null) return;
            using (Graphics g = Graphics.FromImage(joinLayer))
            {
                g.Clear(Color.Transparent);
            }

            foreach (Join join in Screen.Map.Joins)
            {
                if (join.screenOne == this.Screen.Name) DrawJoinEnd(join, true);
                else if (join.screenTwo == this.Screen.Name) DrawJoinEnd(join, false);
            }
            ReDrawMaster();
        }

        private void DrawJoinEnd(Join join, bool one)
        {
            if (joinLayer == null) return;
            using (Graphics g = Graphics.FromImage(joinLayer))
            {
                int offset = one ? join.offsetOne : join.offsetTwo;
                int start = (join.type == JoinType.Horizontal) ? this.Left : this.Top;
                start += offset * 16;
                int end = start + (join.Size * 16);
                int edge;
                Pen pen;
                if (one ? join.direction == JoinDirection.BackwardOnly : join.direction == JoinDirection.ForwardOnly) pen = blockPen;
                else pen = passPen;
                if (join.type == JoinType.Horizontal)
                {
                    edge = one ? Screen.PixelHeight - 2 : 2;
                    int curl = one ? edge - 6 : edge + 6;
                    g.DrawLine(pen, start, edge, end, edge);
                    g.DrawLine(pen, start + 1, edge, start + 1, curl);
                    g.DrawLine(pen, end - 1, edge, end - 1, curl);
                }
                else
                {
                    edge = one ? Screen.PixelWidth - 2 : 2;
                    int curl = one ? edge - 6 : edge + 6;
                    g.DrawLine(pen, edge, start, edge, end);
                    g.DrawLine(pen, edge, start, curl, start);
                    g.DrawLine(pen, edge, end, curl, end);
                }
            }
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

                if (MainForm.Instance.DrawTiles)
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

                if (MainForm.Instance.DrawBlock && blockLayer != null)
                    g.DrawImageUnscaled(blockLayer, 0, 0);

                if (MainForm.Instance.DrawGrid && gridLayer != null)
                    g.DrawImageUnscaled(gridLayer, 0, 0);

                if (MainForm.Instance.DrawJoins && joinLayer != null)
                    g.DrawImageUnscaled(joinLayer, 0, 0);

                if (active) g.DrawImageUnscaled(mouseLayer, 0, 0);
            }

            this.Image = masterImage;
            this.Width = masterImage.Width;
            this.Height = masterImage.Height;
            this.Refresh();
        }

        private void InitLayer(ref Bitmap layer)
        {
            if (layer != null) layer.Dispose();
            ResizeLayer(ref layer);
        }

        private void ResizeLayer(ref Bitmap layer)
        {
            if (layer != null) layer.Dispose();
            layer = new Bitmap(Screen.Width * Screen.Tileset.TileSize, Screen.Height * Screen.Tileset.TileSize);
        }

        private void BuildLayers()
        {
            InitLayer(ref tileLayer);
            InitLayer(ref grayTiles);
            InitLayer(ref gridLayer);
            InitLayer(ref blockLayer);
            InitLayer(ref mouseLayer);
            InitLayer(ref joinLayer);
            InitLayer(ref masterImage);
        }

        private void ResizeLayers()
        {
            ResizeLayer(ref tileLayer);
            ResizeLayer(ref grayTiles);
            ResizeLayer(ref gridLayer);
            ResizeLayer(ref blockLayer);
            ResizeLayer(ref mouseLayer);
            ResizeLayer(ref joinLayer);
            ResizeLayer(ref masterImage);
            ReDrawAll();
        }
    }
}
