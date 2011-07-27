using System.Collections.Generic;
using System.Linq;
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
        private readonly int tileX;
        private readonly int tileY;
        private readonly int oldTileId;
        private readonly int newTileId;
        private readonly ScreenDrawingSurface Surface;

        public TileChange(int tx, int ty, int oldId, int newId, ScreenDrawingSurface surface)
        {
            tileX = tx;
            tileY = ty;
            oldTileId = oldId;
            newTileId = newId;
            Surface = surface;
        }

        public TileChange Reverse()
        {
            return new TileChange(tileX, tileY, newTileId, oldTileId, Surface);
        }

        public void ApplyToSurface(ScreenDrawingSurface surface)
        {
            surface.Screen.ChangeTile(tileX, tileY, newTileId);
        }
    }

    public class DrawAction : HistoryAction
    {
        private readonly List<TileChange> changes;
        private readonly ScreenDrawingSurface surface;
        private readonly string name;

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
                change.ApplyToSurface(surface);
            }
            surface.ReDrawTiles();
        }

        public HistoryAction Reverse()
        {
            List<TileChange> ch = new List<TileChange>(changes.Count);
            ch.AddRange(changes.Select(change => change.Reverse()));
            return new DrawAction(name, ch, surface);
        }
    }

    public class AddEntityAction : HistoryAction
    {
        private readonly Entity entity;
        private readonly ScreenDrawingSurface surface;
        private readonly Point location;

        public AddEntityAction(Entity entity, ScreenDrawingSurface surface, Point location)
        {
            this.entity = entity;
            this.surface = surface;
            this.location = location;
        }

        public void Run()
        {
            surface.Screen.AddEntity(entity, location);
            surface.ReDrawEntities();
        }

        public HistoryAction Reverse()
        {
            return new RemoveEntityAction(entity, surface, location);
        }
    }

    public class RemoveEntityAction : HistoryAction
    {
        private readonly Entity entity;
        private readonly ScreenDrawingSurface surface;
        private readonly Point location;

        public RemoveEntityAction(Entity entity, ScreenDrawingSurface surface, Point location)
        {
            this.entity = entity;
            this.surface = surface;
            this.location = location;
        }

        public void Run()
        {
            surface.Screen.RemoveEntity(entity, location);
            surface.ReDrawEntities();
        }

        public HistoryAction Reverse()
        {
            return new AddEntityAction(entity, surface, location);
        }
    }
}
