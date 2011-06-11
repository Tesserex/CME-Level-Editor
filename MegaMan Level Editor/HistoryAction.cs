using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

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
        private string name;

        public DrawAction(string name, IEnumerable<TileChange> changes, ScreenDrawingSurface surface)
        {
            this.name = name;
            this.changes = new List<TileChange>(changes);
            this.surface = surface;
        }

        public override string ToString()
        {
            return name;
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
            return new DrawAction(name, ch, surface);
        }
    }

    public class AddEntityAction : HistoryAction
    {
        private Entity entity;
        private ScreenDrawingSurface surface;
        private Point location;

        public AddEntityAction(Entity entity, ScreenDrawingSurface surface, Point location)
        {
            this.entity = entity;
            this.surface = surface;
            this.location = location;
        }

        public void Run()
        {
            surface.Screen.AddEntity(entity, location);
            surface.ReDrawAll();
        }

        public HistoryAction Reverse()
        {
            return new RemoveEntityAction(entity, surface, location);
        }
    }

    public class RemoveEntityAction : HistoryAction
    {
        private Entity entity;
        private ScreenDrawingSurface surface;
        private Point location;

        public RemoveEntityAction(Entity entity, ScreenDrawingSurface surface, Point location)
        {
            this.entity = entity;
            this.surface = surface;
            this.location = location;
        }

        public void Run()
        {
            surface.Screen.RemoveEntity(entity, location);
            surface.ReDrawAll();
        }

        public HistoryAction Reverse()
        {
            return new AddEntityAction(entity, surface, location);
        }
    }
}
