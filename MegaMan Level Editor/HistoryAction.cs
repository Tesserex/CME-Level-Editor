﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan_Level_Editor
{
    public interface HistoryAction
    {
        void Run();
        HistoryAction Reverse();
    }

    public class TileChange
    {
        public int Tile_X { get; private set; }
        public int Tile_Y { get; private set; }
        public int OldTileId { get; private set; }
        public int NewTileId { get; private set; }
        public ScreenDrawingSurface Surface { get; private set; }

        public TileChange(int tx, int ty, int oldId, int newId, ScreenDrawingSurface surface)
        {
            Tile_X = tx;
            Tile_Y = ty;
            OldTileId = oldId;
            NewTileId = newId;
            Surface = surface;
        }

        public TileChange Reverse()
        {
            return new TileChange(Tile_X, Tile_Y, NewTileId, OldTileId, Surface);
        }
    }

    public class DrawAction : HistoryAction
    {
        private List<TileChange> changes;
        private ScreenDrawingSurface surface;

        public DrawAction(IEnumerable<TileChange> changes, ScreenDrawingSurface surface)
        {
            this.changes = new List<TileChange>(changes);
            this.surface = surface;
        }

        public override String ToString()
        {
            return "Draw something";
        }

        public void Run()
        {
            foreach (TileChange change in changes)
            {
                surface.Screen.ChangeTile(change.Tile_X, change.Tile_Y, change.NewTileId);
            }
            surface.ReDrawAll();
        }

        public HistoryAction Reverse()
        {
            List<TileChange> ch = new List<TileChange>(changes.Count);
            foreach (TileChange change in changes) ch.Add(change.Reverse());
            return new DrawAction(ch, surface);
        }
    }
}
