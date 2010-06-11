using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MegaMan_Level_Editor
{
    public interface ITool
    {
        void Click(ScreenDrawingSurface surface, Point location);
        void Move(ScreenDrawingSurface surface, Point location);
        void Release(ScreenDrawingSurface surface, Point location);
    }

    public class BrushTool : ITool
    {
        private ITileBrush brush;
        private bool held;
        private Point currentTilePos;

        public BrushTool(ITileBrush brush)
        {
            this.brush = brush;
            held = false;
        }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            surface.DrawBrush(brush, location);
            held = true;
            currentTilePos = new Point(location.X / surface.Screen.Tileset.TileSize, location.Y / surface.Screen.Tileset.TileSize);
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
            if (!held) return;
            Point pos = new Point(location.X / surface.Screen.Tileset.TileSize, location.Y / surface.Screen.Tileset.TileSize);
            if (pos == currentTilePos) return; // don't keep drawing on the same spot

            surface.DrawBrush(brush, location);
        }

        public void Release(ScreenDrawingSurface surface, Point location)
        {
            held = false;
        }
    }

    public class Bucket : ITool
    {
        private ITileBrush brush;
        private MegaMan.Tile[,] cells;
        private int width, height;

        public Bucket(ITileBrush brush)
        {
            this.brush = brush;
            width = brush.Width;
            height = brush.Height;
            cells = new MegaMan.Tile[width, height];
            foreach (TileBrushCell cell in brush.Cells())
            {
                cells[cell.x, cell.y] = cell.tile;
            }
        }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            int tile_x = location.X / surface.Screen.Tileset.TileSize;
            int tile_y = location.Y / surface.Screen.Tileset.TileSize;

            var old = surface.Screen.TileAt(tile_x, tile_y);

            Flood(surface.Screen, tile_x, tile_y, old.Id, 0, 0);

            // need to manually inform the screen surface that I messed with it
            surface.RaiseDrawnOn(tile_x, tile_y, brush, new SingleTileBrush(old));
        }

        private void Flood(MegaMan.Screen screen, int tile_x, int tile_y, int tile_id, int brush_x, int brush_y)
        {
            var old = screen.TileAt(tile_x, tile_y);
            // checking whether this is already the new tile prevents infinite recursion, but
            // it can prevent filling a solid area with a brush that uses that same tile
            if (old == null || old.Id != tile_id || old.Id == cells[brush_x, brush_y].Id) return;

            screen.ChangeTile(tile_x, tile_y, cells[brush_x, brush_y].Id);

            Flood(screen, tile_x - 1, tile_y, tile_id, (brush_x == 0)? width-1 : brush_x - 1, brush_y);
            Flood(screen, tile_x + 1, tile_y, tile_id, (brush_x == width - 1)? 0 : brush_x + 1, brush_y);
            Flood(screen, tile_x, tile_y - 1, tile_id, brush_x, (brush_y == 0)? height-1 : brush_y - 1);
            Flood(screen, tile_x, tile_y + 1, tile_id, brush_x, (brush_y == height - 1)? 0 : brush_y + 1);
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
        }

        public void Release(ScreenDrawingSurface surface, Point location)
        {
        }
    }

    public class JoinTool : ITool
    {
        public void Click(ScreenDrawingSurface surface, Point location)
        {
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
        }

        public void Release(ScreenDrawingSurface surface, Point location)
        {
        }
    }

    public class ToolChangedEventArgs : EventArgs
    {
        public ITool Tool { get; private set; }
        public ToolChangedEventArgs(ITool tool)
        {
            this.Tool = tool;
        }
    }

    public enum ToolType
    {
        Brush,
        Bucket,
        Join
    }
}
