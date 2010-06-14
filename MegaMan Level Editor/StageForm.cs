using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public partial class StageForm : Form
    {
        private MegaMan.Map stage;

        private History history;
        
        private Dictionary<string, ScreenDrawingSurface> surfaces;

        private JoinOverlay joinOverlay;

        public bool DrawGrid
        {
            set
            {
                foreach (var pair in surfaces)
                {
                    pair.Value.DrawGrid = value;
                }
            }
        }

        public bool DrawTiles
        {
            set
            {
                foreach (var pair in surfaces)
                {
                    pair.Value.DrawTiles = value;
                }
            }
        }

        public bool DrawBlock
        {
            set
            {
                foreach (var pair in surfaces)
                {
                    pair.Value.DrawBlock = value;
                }
            }
        }

        public void Undo()
        {
            var action = history.Undo();

            if (action != null)
            {
                action.Run();
            }
        }

        public void Redo()
        {
            var action = history.Redo();

            if (action != null)
            {
                action.Run();
            }
        }

        public StageForm(MegaMan.Map stage)
        {
            InitializeComponent();
            joinOverlay = new JoinOverlay();
            joinOverlay.Owner = this;

            this.SetBackgroundGrid();

            history = new History();
            surfaces = new Dictionary<String, ScreenDrawingSurface>();

            SetStage(stage);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            joinOverlay.Invalidate();
            base.OnScroll(se);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        private void SetBackgroundGrid()
        {
            // just need a tiny tile image
            Image tile = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(tile))
            {
                g.DrawLine(Pens.Black, 15, 0, 15, 15);
                g.DrawLine(Pens.Black, 0, 15, 15, 15);
            }
            this.BackgroundImage = tile;
            this.BackgroundImageLayout = ImageLayout.Tile;
        }

        private void RenameSurface(string oldScreenName, string newScreenName)
        {
            if (oldScreenName == newScreenName) return;
            var surface = surfaces[oldScreenName];
            surfaces.Add(newScreenName, surface);
            surfaces.Remove(oldScreenName);
        }

        /* *
         * SetText - Name the window
         * */
        public void SetText()
        {
            this.Text = this.stage.Name;
            if (this.stage.Dirty) this.Text += " *";
        }

        /* *
         * SetStage - Decide the stage object that will be edited
         * */
        private void SetStage(MegaMan.Map stage)
        {
            this.stage = stage;

            SetText();
            this.stage.DirtyChanged += (b) => SetText();

            foreach (var pair in stage.Screens)
            {
                var surface = CreateScreenSurface(pair.Value);
                surface.Location = new Point(0, 0);
            }

            AlignScreenSurfaces();
        }

        private void AlignScreenSurfaces()
        {
            foreach (var pair in surfaces)
            {
                if (stage.StartScreen == pair.Key)
                    pair.Value.Placed = true;
                else
                    pair.Value.Placed = false;
            }

            int placeCount = 0;
            int minX = 0, minY = 0, maxX = 0, maxY = 0;

            // account for screens that aren't placed - need to find them
            var placeable = new HashSet<string>(); // enforces uniqueness
            foreach (var join in stage.Joins)
            {
                placeable.Add(join.screenOne);
                placeable.Add(join.screenTwo);
            }
            placeable.Remove(stage.StartScreen); // this one is already placed

            while (placeCount < placeable.Count)
            {
                foreach (var join in stage.Joins)
                {
                    bool placed = AlignScreenSurfaceUsingJoin(surfaces[join.screenOne], surfaces[join.screenTwo], join);
                    if (placed)
                    {
                        placeCount++;
                        minX = Math.Min(minX, Math.Min(surfaces[join.screenOne].Location.X, surfaces[join.screenTwo].Location.X));
                        minY = Math.Min(minY, Math.Min(surfaces[join.screenOne].Location.Y, surfaces[join.screenTwo].Location.Y));
                    }
                }
            }

            if (minX < 0 || minY < 0)
            {
                // now readjust to all positive locations
                foreach (var surface in surfaces.Values)
                {
                    surface.Location = new Point(surface.Location.X - minX, surface.Location.Y - minY);
                }
            }

            foreach (var surface in surfaces.Values)
            {
                maxX = Math.Max(maxX, surface.Right);
                maxY = Math.Max(maxY, surface.Bottom);
            }

            joinOverlay.Refresh(maxX, maxY, stage.Joins, surfaces);
        }

        private bool AlignScreenSurfaceUsingJoin(ScreenDrawingSurface surface, ScreenDrawingSurface secondSurface, Join join)
        {
            var offset = (join.offsetTwo - join.offsetOne) * surface.Screen.Tileset.TileSize;

            if (surface.Placed && !secondSurface.Placed)
            {
                // TODO: WTF? Why does horizontal mean vertical and vertical mean horizontal?
                if (join.type == JoinType.Horizontal)
                {
                    // Place image below
                    var p = new Point(surface.Location.X - offset, surface.Location.Y + surface.Screen.PixelHeight);
                    secondSurface.Location = p;
                }
                else
                {
                    // Place image to the right
                    var p = new Point(surface.Location.X + surface.Screen.PixelWidth, surface.Location.Y - offset);
                    secondSurface.Location = p;
                }
                secondSurface.Placed = true;
                return true;
            }
            else if (secondSurface.Placed && !surface.Placed)
            {
                if (join.type == JoinType.Horizontal)
                {
                    // Place image above
                    var p = new Point(secondSurface.Location.X + offset, secondSurface.Location.Y - surface.Screen.PixelHeight);
                    surface.Location = p;
                }
                else
                {
                    // Place image to the left
                    var p = new Point(secondSurface.Location.X - surface.Screen.PixelWidth, secondSurface.Location.Y + offset);
                    surface.Location = p;
                }
                surface.Placed = true;
                return true;
            }
            return false;
        }

        private ScreenDrawingSurface CreateScreenSurface(MegaMan.Screen screen)
        {
            var surface = new ScreenDrawingSurface(screen);
            surfaces.Add(screen.Name, surface);
            screen.Renamed += this.RenameSurface;
            screen.Resized += (w, h) => this.AlignScreenSurfaces();
            surface.DrawnOn += new EventHandler<ScreenDrawEventArgs>(surface_DrawnOn);
            this.Controls.Add(surface);
            joinOverlay.Add(surface);
            return surface;
        }

        void surface_DrawnOn(object sender, ScreenDrawEventArgs e)
        {
            if (e.Changes.Count > 0)
            {
                history.Push(new DrawAction(e.Changes, e.Surface));
            }
        }
    }
}
