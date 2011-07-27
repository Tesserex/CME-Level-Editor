using System;
using System.Drawing;
using System.Windows.Forms;
using MegaMan;
using System.Drawing.Imaging;

namespace MegaMan_Level_Editor
{
    public class ScreenEditEventArgs : EventArgs
    {
        public HistoryAction Action { get; private set; }

        public ScreenEditEventArgs(HistoryAction action)
        {
            Action = action;
        }
    }

    /* *
     * ScreenDrawingSurface - Draw a screen onto one of these. 
     * Multiple screen surfaces show an entire map in one window
     * */
    public class ScreenDrawingSurface : PictureBox
    {
        private static readonly Brush blockBrush = new SolidBrush(Color.FromArgb(160, Color.OrangeRed));
        private static readonly Brush ladderBrush = new SolidBrush(Color.FromArgb(160, Color.Yellow));
        private static readonly Pen passPen = new Pen(Color.Blue, 4);
        private static readonly Pen blockPen = new Pen(Color.Red, 4);
        private static readonly Bitmap cursor;

        private Bitmap tileLayer;
        private Bitmap gridLayer;
        private Bitmap blockLayer;
        private Bitmap entityLayer;
        private Bitmap mouseLayer;
        private Bitmap joinLayer;
        private Bitmap masterImage;
        private Bitmap grayTiles;

        private bool grayDirty;

        private static readonly ColorMatrix grayMatrix = new ColorMatrix(
           new float[][] 
          {
             new float[] {.3f, .3f, .3f, 0, 0},
             new float[] {.59f, .59f, .59f, 0, 0},
             new float[] {.11f, .11f, .11f, 0, 0},
             new float[] {0, 0, 0, 1, 0},
             new float[] {0, 0, 0, 0, 1}
          });

        private bool active;
        public bool Placed { get; set; }

        public ScreenDocument Screen { get; private set; }

        public event EventHandler<ScreenEditEventArgs> Edited;

        #region Constructors

        static ScreenDrawingSurface()
        {
            cursor = new Bitmap(Cursor.Current.Size.Width, Cursor.Current.Size.Height);
            using (Graphics g = Graphics.FromImage(cursor)) Cursor.Current.Draw(g, new Rectangle(0,0,cursor.Width,cursor.Height));
        }

        public ScreenDrawingSurface(ScreenDocument screen)
        {
            Screen = screen;

            BackColor = SystemColors.Control;
            BackgroundImageLayout = ImageLayout.None;

            BuildLayers();

            Screen.Resized += (w, h) => ResizeLayers();

            Program.FrameTick += Program_FrameTick;

            RedrawJoins();
            ReDrawAll();

            MainForm.Instance.DrawOptionToggled += ReDrawMaster;
        }

        #endregion

        public void EditedWithAction(HistoryAction action)
        {
            if (Edited != null) Edited(this, new ScreenEditEventArgs(action));
        }

        void Program_FrameTick()
        {
            if (active)
            {
                ReDrawTiles();
                ReDrawMaster();
            }
        }

        #region Mouse Handlers

        protected override void OnMouseEnter(EventArgs e)
        {
            active = true;
            ReDrawMaster();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            active = false;
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

            if (e.Button == MouseButtons.Left)
            {
                MainForm.Instance.CurrentTool.Click(this, e.Location);
            }
            else if (e.Button == MouseButtons.Right)
            {
                MainForm.Instance.CurrentTool.RightClick(this, e.Location);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (MainForm.Instance.CurrentTool == null) return;

            if (e.Button == MouseButtons.Left)
            {
                MainForm.Instance.CurrentTool.Release(this);
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mouseLayer == null) return;

            if (MainForm.Instance.CurrentTool != null)
            {
                if (MainForm.Instance.CurrentTool.Icon != null)
                {
                    int tx = e.X;
                    int ty = e.Y;

                    if (MainForm.Instance.CurrentTool.IconSnap)
                    {
                        tx = (e.X / Screen.Tileset.TileSize) * Screen.Tileset.TileSize;
                        ty = (e.Y / Screen.Tileset.TileSize) * Screen.Tileset.TileSize;
                    }

                    Bitmap icon = (Bitmap)MainForm.Instance.CurrentTool.Icon;

                    Point offset = MainForm.Instance.CurrentTool.IconOffset;

                    icon.SetResolution(mouseLayer.HorizontalResolution, mouseLayer.VerticalResolution);

                    using (Graphics g = Graphics.FromImage(mouseLayer))
                    {
                        g.Clear(Color.Transparent);
                        g.DrawImageUnscaled(icon, tx + offset.X, ty + offset.Y, icon.Width, icon.Height);
                    }

                    ReDrawMaster();
                }
                if (e.Button == MouseButtons.Left)
                {
                    MainForm.Instance.CurrentTool.Move(this, e.Location);
                }
            }

            base.OnMouseMove(e);
        }

        #endregion

        #region Layer Drawing

        public void RedrawJoins()
        {
            if (joinLayer == null) return;
            using (Graphics g = Graphics.FromImage(joinLayer))
            {
                g.Clear(Color.Transparent);
            }

            foreach (Join join in Screen.Stage.Joins)
            {
                if (join.screenOne == Screen.Name) DrawJoinEnd(join, true);
                else if (join.screenTwo == Screen.Name) DrawJoinEnd(join, false);
            }
            ReDrawMaster();
        }

        private void DrawJoinEnd(Join join, bool one)
        {
            if (joinLayer == null) return;
            using (Graphics g = Graphics.FromImage(joinLayer))
            {
                int offset = one ? join.offsetOne : join.offsetTwo;
                int start = offset * Screen.Tileset.TileSize;
                int end = start + (join.Size * Screen.Tileset.TileSize);
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

        private void ReDrawAll()
        {
            ReDrawTiles();
            ReDrawEntities();
            ReDrawBlocking();
            ReDrawMaster();
            ReDrawGrid();
        }

        public void ReDrawTiles()
        {
            using (Graphics g = Graphics.FromImage(tileLayer))
            {
                Screen.DrawOn(g);
            }
            grayDirty = true;

            ReDrawMaster();
        }

        private void DrawGray()
        {
            if (!grayDirty) return;

            if (grayTiles != null) grayTiles.Dispose();
            grayTiles = ConvertToGrayscale(tileLayer);
            grayDirty = false;
        }

        private static Bitmap ConvertToGrayscale(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(grayMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }

        public void ReDrawEntities()
        {
            using (Graphics g = Graphics.FromImage(entityLayer))
            {
                g.Clear(Color.Transparent);

                Screen.DrawEntities(g);
                if (Screen.Name == Screen.Stage.StartScreen)
                {
                    g.DrawImage(StartPositionTool.MegaMan, Screen.Stage.StartPoint.X - 4, Screen.Stage.StartPoint.Y - 12);
                }
            }

            ReDrawMaster();
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
                        if (grayTiles != null) g.DrawImageUnscaled(grayTiles, 0, 0);
                    }
                }

                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                if (MainForm.Instance.DrawEntities && entityLayer != null)
                    g.DrawImageUnscaled(entityLayer, 0, 0);

                if (MainForm.Instance.DrawBlock && blockLayer != null)
                    g.DrawImageUnscaled(blockLayer, 0, 0);

                if (MainForm.Instance.DrawGrid && gridLayer != null)
                    g.DrawImageUnscaled(gridLayer, 0, 0);

                if (MainForm.Instance.DrawJoins && joinLayer != null)
                    g.DrawImageUnscaled(joinLayer, 0, 0);

                if (active) g.DrawImageUnscaled(mouseLayer, 0, 0);
            }

            Image = masterImage;
            Width = masterImage.Width;
            Height = masterImage.Height;
            Refresh();
        }

#endregion

        #region Layer Helpers

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
            InitLayer(ref entityLayer);
            InitLayer(ref mouseLayer);
            InitLayer(ref joinLayer);
            InitLayer(ref masterImage);
        }

        private void ResizeLayers()
        {
            ResizeLayer(ref tileLayer);
            ResizeLayer(ref grayTiles);
            ResizeLayer(ref gridLayer);
            ResizeLayer(ref entityLayer);
            ResizeLayer(ref blockLayer);
            ResizeLayer(ref mouseLayer);
            ResizeLayer(ref joinLayer);
            ResizeLayer(ref masterImage);
            ReDrawAll();
        }

        #endregion
    }
}
