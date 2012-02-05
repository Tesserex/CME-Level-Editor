using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace MegaMan.LevelEditor
{
    public class RectangleTool : ITool
    {
        private readonly ITileBrush brush;
        private int tx1, ty1, tx2, ty2;
        private bool held;
        private readonly List<TileChange> changes;

        public RectangleTool(ITileBrush brush)
        {
            this.brush = brush;
            held = false;
            changes = new List<TileChange>();
        }

        public Image Icon
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

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            tx1 = location.X / surface.Screen.Tileset.TileSize;
            ty1 = location.Y / surface.Screen.Tileset.TileSize;
            held = true;
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
            if (held)
            {
                tx2 = location.X / surface.Screen.Tileset.TileSize;
                ty2 = location.Y / surface.Screen.Tileset.TileSize;
            }
        }

        public void Release(ScreenDrawingSurface surface)
        {
            int x_start = Math.Min(tx1, tx2);
            int x_end = Math.Max(tx1, tx2) - 1;
            int y_start = Math.Min(ty1, ty2);
            int y_end = Math.Max(ty1, ty2) - 1;

            for (int y = y_start; y <= y_end; y++)
            {
                for (int x = x_start; x <= x_end; x++)
                {
                    Draw(surface, x, y);
                }
            }

            if (changes.Count > 0) surface.EditedWithAction(new DrawAction("Rectangle", changes, surface));
        }

        private void Draw(ScreenDrawingSurface surface, int tile_x, int tile_y)
        {
            ITileBrush reverse = brush.DrawOn(surface.Screen, tile_x, tile_y);
            if (reverse == null) return;

            int[,] tiles = new int[brush.Width, brush.Height];
            foreach (TileBrushCell cell in brush.Cells())
            {
                tiles[cell.x, cell.y] = cell.tile.Id;
            }
            foreach (TileBrushCell cell in reverse.Cells())
            {
                if (cell.tile == null) continue;
                // this will NOT work properly for multi-cell brushes - if you paint over a place you just painted,
                // the cell will change, but reversing the changes in an undo will be wrong.
                if (tiles[cell.x, cell.y] != cell.tile.Id)
                    changes.Add(new TileChange(tile_x + cell.x, tile_y + cell.y, cell.tile.Id, tiles[cell.x, cell.y], surface));
            }
            surface.ReDrawTiles();
        }

        public void RightClick(ScreenDrawingSurface surface, Point location)
        {
            
        }

        public Point IconOffset
        {
            get { return new Point(-7, -7); }
        }
    }
}
