using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace MegaMan.LevelEditor
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
        public bool IsIconCursor { get { return false; } }

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
            Point tilePos = new Point(location.X / surface.Screen.Tileset.TileSize, location.Y / surface.Screen.Tileset.TileSize);

            // check for line drawing
            if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
            {
                var xdist = Math.Abs(tilePos.X - currentTilePos.X);
                var ydist = Math.Abs(tilePos.Y - currentTilePos.Y);

                if (xdist >= ydist)
                {
                    var min = Math.Min(currentTilePos.X, tilePos.X);
                    var max = Math.Max(currentTilePos.X, tilePos.X);
                    for (int i = min; i <= max; i++)
                    {
                        Draw(surface, i, currentTilePos.Y);
                    }
                }
                else
                {
                    var min = Math.Min(currentTilePos.Y, tilePos.Y);
                    var max = Math.Max(currentTilePos.Y, tilePos.Y);
                    for (int i = min; i <= max; i++)
                    {
                        Draw(surface, currentTilePos.X, i);
                    }
                }
            }
            else
            {
                Draw(surface, tilePos.X, tilePos.Y);
                held = true;
            }

            currentTilePos = tilePos;
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
            if (!held) return;
            Point pos = new Point(location.X / surface.Screen.Tileset.TileSize, location.Y / surface.Screen.Tileset.TileSize);
            if (pos == currentTilePos) return; // don't keep drawing on the same spot

            Draw(surface, pos.X, pos.Y);
        }

        public void Release(ScreenDrawingSurface surface)
        {
            held = false;
            if (changes.Count > 0) surface.EditedWithAction(new DrawAction("Brush", changes, surface));
            changes.Clear();
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