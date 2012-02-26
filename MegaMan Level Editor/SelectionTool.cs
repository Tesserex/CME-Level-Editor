using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MegaMan.LevelEditor
{
    public class SelectionTool : ITool, IDisposable
    {
        private int tx1, ty1, tx2, ty2;
        private bool held;
        private Pen pen;
        private int tickFrame = 0;

        private ScreenDrawingSurface currentSurface;

        public SelectionTool()
        {
            pen = new Pen(Color.LimeGreen, 2);
            pen.DashPattern = new float[] { 3, 2 };
            Program.FrameTick += Program_FrameTick;
        }

        void Program_FrameTick()
        {
            if (held)
            {
                tickFrame++;
                if (tickFrame >= 5)
                {
                    pen.DashOffset++;
                    if (pen.DashOffset > 4) pen.DashOffset = 0;
                    tickFrame = 0;
                    DrawAnts();
                }
            }
        }

        public System.Drawing.Image Icon
        {
            get { return Properties.Resources.cross; }
        }

        public bool IconSnap
        {
            get { return true; }
        }

        public bool IsIconCursor
        {
            get { return false; }
        }

        public void Click(ScreenDrawingSurface surface, System.Drawing.Point location)
        {
            if (currentSurface != null)
            {
                var g = surface.GetToolLayerGraphics();
                if (g != null)
                {
                    g.Clear(Color.Transparent);
                }
            }

            currentSurface = surface;

            tx1 = location.X / surface.Screen.Tileset.TileSize;
            ty1 = location.Y / surface.Screen.Tileset.TileSize;
            held = true;
        }

        public void Move(ScreenDrawingSurface surface, System.Drawing.Point location)
        {
            if (held)
            {
                tx2 = location.X / surface.Screen.Tileset.TileSize;
                ty2 = location.Y / surface.Screen.Tileset.TileSize;

                DrawAnts();
            }
        }

        private void DrawAnts()
        {
            if (currentSurface == null) return;

            var size = currentSurface.Screen.Tileset.TileSize;

            var g = currentSurface.GetToolLayerGraphics();
            if (g != null)
            {
                g.Clear(Color.Transparent);

                // draw selection preview
                g.DrawRectangle(pen, tx1 * size, ty1 * size, size * (tx2 - tx1), size * (ty2 - ty1));

                currentSurface.ReturnToolLayerGraphics(g);
            }
        }

        public void Release(ScreenDrawingSurface surface)
        {
            
        }

        public void RightClick(ScreenDrawingSurface surface, System.Drawing.Point location)
        {

        }

        public System.Drawing.Point IconOffset
        {
            get { return new Point(-7, -7); }
        }

        public void Dispose()
        {
            pen.Dispose();
            Program.FrameTick -= Program_FrameTick;
        }
    }
}
