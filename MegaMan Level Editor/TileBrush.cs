using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MegaMan;
using System.Windows.Forms;

namespace MegaMan_Level_Editor
{
    public interface ITileBrush
    {
        ITileBrush DrawOn(MegaMan.Screen screen, int tile_x, int tile_y);
        void DrawOn(Graphics g, int x, int y);
        IEnumerable<TileBrushCell> Cells();
        int Height { get; }
        int Width { get; }
        int CellSize { get; }
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
        protected Tile tile;

        public int Height { get { return 1; } }
        public int Width { get { return 1; } }

        public int CellSize { get { return (int)tile.Width; } }

        public SingleTileBrush(Tile tile)
        {
            this.tile = tile;
        }

        public void DrawOn(Graphics g, int x, int y)
        {
            tile.Draw(g, x, y);
        }

        public virtual ITileBrush DrawOn(MegaMan.Screen screen, int tile_x, int tile_y) {
            var old = screen.TileAt(tile_x, tile_y);
            
            if (old == null)
                return null;

            if (old.Id == this.tile.Id) {
                return null;
            } else {
//                MessageBox.Show("Drawing tile " + this.tile + " where previously we had " + old);
                screen.ChangeTile(tile_x, tile_y, tile.Id);
                return new SingleTileBrush(old);
            }
        }

        public override String ToString() {
            return "Tile Id: (" + this.tile + ")";
        }

        public IEnumerable<TileBrushCell> Cells() { yield return new TileBrushCell(0, 0, this.tile); }
    }

    public class TileBrush : ITileBrush
    {
        private TileBrushCell[][] cells;
        
        public int Height { get; private set; }
        public int Width { get; private set; }

        public int CellSize { get; private set; }

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
            CellSize = (int)tile.Width;
        }

        #region ITileBrush Members

        /// <summary>
        /// Draws the brush onto the given screen at the given tile location.
        /// Returns an "undo brush" - a brush of all tiles that were overwritten.
        /// Returns null if no tiles were changed.
        /// </summary>
        public ITileBrush DrawOn(MegaMan.Screen screen, int tile_x, int tile_y) {
            TileBrush undo = new TileBrush(Width, Height);
            bool changed = false;
            foreach (TileBrushCell[] col in cells) {
                foreach (TileBrushCell cell in col) {
                    var old = screen.TileAt(cell.x + tile_x, cell.y + tile_y);

                    if (old == null)
                        continue;
                    undo.AddTile(old, cell.x, cell.y);
                    if (old.Id != cell.tile.Id) {
                        changed = true;
                        screen.ChangeTile(cell.x + tile_x, cell.y + tile_y, cell.tile.Id);
                    }
                }
            }

            if (changed) return undo;
            return null;
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

