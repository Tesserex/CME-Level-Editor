using System.Collections.Generic;
using System.Drawing;

namespace MegaMan_Level_Editor
{
    public class BrushTool : ITool
    {
        private readonly ITileBrush brush;
        private bool held;
        private Point currentTilePos;
        private readonly List<TileChange> changes;

        public Image Icon { get; private set; }
        public Point IconOffset { get { return Point.Empty; } }
        public bool IconSnap { get { return true; } }

        public BrushTool(ITileBrush brush)
        {
            this.brush = brush;
            held = false;
            Icon = new Bitmap(brush.Width * brush.CellSize, brush.Height * brush.CellSize);
            using (Graphics g = Graphics.FromImage(Icon))
            {
                brush.DrawOn(g, 0, 0);
            }
            changes = new List<TileChange>();
        }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            Draw(surface, location);
            held = true;
            currentTilePos = new Point(location.X / surface.Screen.Tileset.TileSize, location.Y / surface.Screen.Tileset.TileSize);
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
            if (!held) return;
            Point pos = new Point(location.X / surface.Screen.Tileset.TileSize, location.Y / surface.Screen.Tileset.TileSize);
            if (pos == currentTilePos) return; // don't keep drawing on the same spot

            Draw(surface, location);
        }

        public void Release(ScreenDrawingSurface surface)
        {
            held = false;
            if (changes.Count > 0) surface.EditedWithAction(new DrawAction("Brush", changes, surface));
            changes.Clear();
        }

        private void Draw(ScreenDrawingSurface surface, Point location)
        {
            int tile_x = location.X / surface.Screen.Tileset.TileSize;
            int tile_y = location.Y / surface.Screen.Tileset.TileSize;

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

        // behaves as eyedropper
        public void RightClick(ScreenDrawingSurface surface, Point location)
        {
            int tile_x = location.X / surface.Screen.Tileset.TileSize;
            int tile_y = location.Y / surface.Screen.Tileset.TileSize;

            var tile = surface.Screen.TileAt(tile_x, tile_y);
            MainForm.Instance.TileStrip.SelectTile(tile);
        }
    }
}