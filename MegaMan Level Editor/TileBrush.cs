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
        void DrawOn(Graphics g, int x, int y);
        IEnumerable<TileBrushCell> Cells();
    }

    public struct TileBrushCell
    {
        public int x;
        public int y;
        public Tile tile;

        public TileBrushCell(int x, int y, Tile tile)
        {
            this.x = x;
            this.y = y;
            this.tile = tile;
        }
    }

    public class SingleTileBrush : ITileBrush
    {
        private Tile tile;

        public SingleTileBrush(Tile tile)
        {
            this.tile = tile;
        }

        public void DrawOn(Graphics g, int x, int y)
        {
            tile.Draw(g, x, y);
        }

        public ITileBrush DrawOn(Screen screen, int tile_x, int tile_y)
        {
            screen.ChangeTile(tile_x, tile_y, tile.Id);
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

        public void AddTile(Tile tile, int x, int y)
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
                Tile old = screen.TileAt(cell.x + tile_x, cell.y + tile_y);
                if (old == null) continue;
                undo.AddTile(old, cell.x, cell.y);
                if (old.Id != cell.tile.Id) changed = true;
                screen.ChangeTile(cell.x + tile_x, cell.y + tile_y, cell.tile.Id);
            }
            if (!changed) return null;
            return undo;
        }

        public void DrawOn(Graphics g, int x, int y)
        {
            foreach (TileBrushCell cell in Cells())
            {
                cell.tile.Draw(g, x + cell.x * cell.tile.Width, y + cell.y * cell.tile.Height);
            }
        }

        public IEnumerable<TileBrushCell> Cells()
        {
            foreach (TileBrushCell[] col in cells) foreach (TileBrushCell cell in col) yield return cell;
        }

        #endregion
    }
}
