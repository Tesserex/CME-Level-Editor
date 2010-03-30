using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public interface ITileBrush
    {
        ITileBrush DrawOn(Screen screen, int tile_x, int tile_y);
        IEnumerable<TileBrushCell> Cells();
    }

    public struct TileBrushCell
    {
        public int x;
        public int y;
        public int tile;

        public TileBrushCell(int x, int y, int tile)
        {
            this.x = x;
            this.y = y;
            this.tile = tile;
        }
    }

    public class SingleTileBrush : ITileBrush
    {
        private int tile;

        public SingleTileBrush(int tile)
        {
            this.tile = tile;
        }

        public ITileBrush DrawOn(Screen screen, int tile_x, int tile_y)
        {
            screen.ChangeTile(tile_x, tile_y, tile);
            return this;
        }

        public IEnumerable<TileBrushCell> Cells() { yield return new TileBrushCell(0, 0, this.tile); }
    }

    public class TileBrush : ITileBrush
    {
        private TileBrushCell[][] cells;
        
        public int Height { get; private set; }
        public int Width { get; private set; }

        public TileBrush(int width, int height)
        {
            Reset(width, height);
        }

        public void Reset(int width, int height)
        {
            cells = new TileBrushCell[width][];
            for (int i = 0; i < width; i++) cells[i] = new TileBrushCell[height];
            Height = height;
            Width = width;
        }

        public void AddTile(int tile, int x, int y)
        {
            TileBrushCell cell = new TileBrushCell();
            cell.x = x;
            cell.y = y;
            cell.tile = tile;
            cells[x][y] = cell;
        }

        #region ITileBrush Members

        /// <summary>
        /// Draws the brush onto the given screen at the given tile location.
        /// Returns an "undo brush" - a brush of all tiles that were overwritten.
        /// Returns null if no tiles were changed.
        /// </summary>
        public ITileBrush DrawOn(Screen screen, int tile_x, int tile_y)
        {
            TileBrush undo = new TileBrush(Width, Height);
            bool changed = false;
            foreach (TileBrushCell[] col in cells) foreach (TileBrushCell cell in col)
            {
                int old = screen.TileIndexAt(cell.x + tile_x, cell.y + tile_y) ?? -1;
                if (old < 0) continue;
                undo.AddTile(old, cell.x, cell.y);
                if (old != cell.tile) changed = true;
                screen.ChangeTile(cell.x + tile_x, cell.y + tile_y, cell.tile);
            }
            if (!changed) return null;
            return undo;
        }

        public void DrawOn(Graphics g, Tileset tileset)
        {
            foreach (TileBrushCell cell in Cells())
            {
                tileset[cell.tile].Draw(g, cell.x * tileset.TileSize, cell.y * tileset.TileSize);
            }
        }

        public IEnumerable<TileBrushCell> Cells()
        {
            foreach (TileBrushCell[] col in cells) foreach (TileBrushCell cell in col) yield return cell;
        }

        #endregion
    }
}
