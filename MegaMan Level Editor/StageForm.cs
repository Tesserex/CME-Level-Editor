﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using MegaMan.Common;

namespace MegaMan.LevelEditor
{
    public partial class StageForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private StageDocument stage;

        private History history;
        
        private Dictionary<string, ScreenDrawingSurface> surfaces;

        private JoinOverlay joinOverlay;

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

        public StageForm(StageDocument stage)
        {
            InitializeComponent();
            joinOverlay = new JoinOverlay();
            joinOverlay.Owner = this;

            this.SetBackgroundGrid();

            history = new History();
            surfaces = new Dictionary<String, ScreenDrawingSurface>();

            SetStage(stage);

            MainForm.Instance.DrawOptionToggled += () => { joinOverlay.Visible = MainForm.Instance.DrawJoins; };
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            joinOverlay.Invalidate();
            base.OnScroll(se);
        }

        private void SetBackgroundGrid()
        {
            // just need a tiny tile image
            Image tile = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(tile))
            {
                g.DrawLine(Pens.Gray, 15, 0, 15, 15);
                g.DrawLine(Pens.Gray, 0, 15, 15, 15);
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
            if (this.stage.Dirty) this.Text += "*";
        }

        /* *
         * SetStage - Decide the stage object that will be edited
         * */
        private void SetStage(StageDocument stage)
        {
            this.stage = stage;

            SetText();
            this.stage.DirtyChanged += (b) => SetText();

            stage.JoinChanged += (join) =>
            {
                AlignScreenSurfaces();
                if (surfaces.ContainsKey(join.screenOne)) surfaces[join.screenOne].RedrawJoins();
                if (surfaces.ContainsKey(join.screenTwo)) surfaces[join.screenTwo].RedrawJoins();
            };

            foreach (var screen in stage.Screens)
            {
                var surface = CreateScreenSurface(screen);
                surface.Location = new Point(0, 0);
            }

            AlignScreenSurfaces();
            stage.ScreenAdded += (s) =>
            {
                CreateScreenSurface(s);
                AlignScreenSurfaces();
            };
        }

        private class SurfaceCollision
        {
            public ScreenDrawingSurface Surface;
            public Rectangle Collision;
            public SurfaceCollision(ScreenDrawingSurface surface, Rectangle collision)
            {
                Surface = surface;
                Collision = collision;
            }
        }

        private void AlignScreenSurfaces()
        {
            if (surfaces.Count == 0) return;

            var placeable = new HashSet<string>();
            var orphans = new List<string>();

            int oldscroll = this.VerticalScroll.Value;
            this.VerticalScroll.Value = 0;

            int minX = 0, minY = 0, maxX = 0, maxY = 0;

            foreach (var surface in surfaces.Values)
            {
                surface.Placed = false;
            }

            var startScreen = surfaces.Values.First();
            if (surfaces.ContainsKey(stage.StartScreen))
            {
                startScreen = surfaces[stage.StartScreen];
            }

            // lay the screens out like a deep graph traversal
            LayoutFromScreen(startScreen, new Point(0, 0));

            // any remaining disconnected screens
            foreach (var surface in surfaces.Values.Where(s => !s.Placed))
            {
                LayoutFromScreen(surface, new Point(0, 0));
            }
            
            foreach (var surface in surfaces.Values)
            {
                minX = Math.Min(minX, surface.Location.X);
                minY = Math.Min(minY, surface.Location.Y);
            }

            if (minX < -this.HorizontalScroll.Value || minY < -this.VerticalScroll.Value)
            {
                // now readjust to all positive locations
                foreach (var surface in surfaces.Values)
                {
                    surface.Location = new Point(surface.Location.X - minX - this.HorizontalScroll.Value, surface.Location.Y - minY - this.VerticalScroll.Value);
                }
            }

            foreach (var surface in surfaces.Values)
            {
                maxX = Math.Max(maxX, surface.Right);
                maxY = Math.Max(maxY, surface.Bottom);
            }

            joinOverlay.Refresh(maxX + 20, maxY + 20, stage.Joins, surfaces);
            joinOverlay.Visible = MainForm.Instance.DrawJoins;

            this.VerticalScroll.Value = oldscroll;
        }

        private void LayoutFromScreen(ScreenDrawingSurface surface, Point location)
        {
            surface.Location = location;
            surface.Placed = true;

            var myJoins = stage.Joins.Where(j => j.screenOne == surface.Screen.Name || j.screenTwo == surface.Screen.Name);
            var joinedScreens = surfaces.Values.Where(s => myJoins.Any(j => j.screenOne == s.Screen.Name || j.screenTwo == s.Screen.Name));

            var placed = surfaces.Values.Where(s => s.Placed && s != surface && !joinedScreens.Contains(s));
            var collision = SurfaceCollides(placed, surface);
            while (collision != Rectangle.Empty)
            {
                TryToFixCollision(surface, collision);
                collision = SurfaceCollides(placed, surface);
            }

            foreach (var join in stage.Joins.Where(j => j.screenOne == surface.Screen.Name))
            {
                var nextScreen = surfaces[join.screenTwo];
                if (nextScreen.Placed) continue;

                LayoutNextScreen(nextScreen, surface.Location, join, true);
            }

            foreach (var join in stage.Joins.Where(j => j.screenTwo == surface.Screen.Name))
            {
                var nextScreen = surfaces[join.screenOne];
                if (nextScreen.Placed) continue;

                LayoutNextScreen(nextScreen, surface.Location, join, false);
            }
        }

        private void LayoutNextScreen(ScreenDrawingSurface surface, Point location, Join join, bool one)
        {
            int offsetX = location.X;
            int offsetY = location.Y;
            int mag = one ? 1 : -1;

            if (join.type == JoinType.Horizontal)
            {
                offsetX += (join.offsetOne - join.offsetTwo) * stage.Tileset.TileSize * mag;
                offsetY += surfaces[join.screenOne].Screen.PixelHeight * mag;
            }
            else
            {
                offsetX += surfaces[join.screenOne].Screen.PixelWidth * mag;
                offsetY += (join.offsetOne - join.offsetTwo) * stage.Tileset.TileSize * mag;
            }

            LayoutFromScreen(surface, new Point(offsetX, offsetY));
        }

        private Rectangle SurfaceCollides(IEnumerable<ScreenDrawingSurface> placedAlready, ScreenDrawingSurface next)
        {
            Rectangle collisions = Rectangle.Empty;
            Rectangle nextRect = new Rectangle(next.Location, next.Size);
            foreach (ScreenDrawingSurface surface in placedAlready)
            {
                Rectangle inter = Rectangle.Intersect(new Rectangle(surface.Location, surface.Size), nextRect);
                if (inter.Width > 0 || inter.Height > 0)
                {
                    if (collisions.Height > 0 && collisions.Width > 0) collisions = Rectangle.Union(collisions, inter);
                    else collisions = inter;
                }
            }
            return collisions;
        }

        private void TryToFixCollision(ScreenDrawingSurface surface, Rectangle collision)
        {
            Point collCenter = collision.Location;
            collCenter.Offset(collision.Width / 2, collision.Height / 2);

            Point surfCenter = surface.Location;
            surfCenter.Offset(surface.Width / 2, surface.Height / 2);

            int off_y = surfCenter.Y - collCenter.Y;
            int off_x = surfCenter.X - collCenter.X;
            if (Math.Abs(off_y) > Math.Abs(off_x))
            {
                surface.Location = new Point(surface.Location.X, surface.Location.Y + surface.Screen.Tileset.TileSize);
            }
            else
            {
                surface.Location = new Point(surface.Location.X + surface.Screen.Tileset.TileSize, surface.Location.Y);
            }
        }

        private ScreenDrawingSurface CreateScreenSurface(ScreenDocument screen)
        {
            var surface = new ScreenDrawingSurface(screen);
            surfaces.Add(screen.Name, surface);
            screen.Renamed += this.RenameSurface;
            screen.Resized += (w, h) => this.AlignScreenSurfaces();
            surface.Edited += new EventHandler<ScreenEditEventArgs>(surface_Edited);
            
            this.Controls.Add(surface);
            joinOverlay.Add(surface);
            return surface;
        }

        void surface_Edited(object sender, ScreenEditEventArgs e)
        {
            history.Push(e.Action);
        }
    }
}
