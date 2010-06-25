using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace MegaMan_Level_Editor
{
    public interface ITool
    {
        Image Icon { get; }
        void Click(ScreenDrawingSurface surface, Point location);
        void Move(ScreenDrawingSurface surface, Point location);
        void Release(ScreenDrawingSurface surface, Point location);
    }

    public class BrushTool : ITool
    {
        private ITileBrush brush;
        private bool held;
        private Point currentTilePos;
        private List<TileChange> changes;

        public Image Icon { get; private set; }

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

        public void Release(ScreenDrawingSurface surface, Point location)
        {
            held = false;
            if (changes.Count > 0) surface.RaiseDrawnOn(new DrawAction("Brush", changes, surface));
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
                // this will NOT work properly for multi-cell brushes - if you paint over a place you just painted,
                // the cell will change, but reversing the changes in an undo will be wrong.
                if (tiles[cell.x, cell.y] != cell.tile.Id)
                    changes.Add(new TileChange(tile_x + cell.x, tile_y + cell.y, cell.tile.Id, tiles[cell.x, cell.y], surface));
            }
            surface.ReDrawAll();
        }
    }

    public class Bucket : ITool
    {
        private ITileBrush brush;
        private MegaMan.Tile[,] cells;
        private int width, height;
        private List<TileChange> changes;

        public Image Icon { get; private set; }

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
            Icon = new Bitmap(brush.Width * brush.CellSize, brush.Height * brush.CellSize);
            using (Graphics g = Graphics.FromImage(Icon))
            {
                brush.DrawOn(g, 0, 0);
            }
            changes = new List<TileChange>();
        }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            int tile_x = location.X / surface.Screen.Tileset.TileSize;
            int tile_y = location.Y / surface.Screen.Tileset.TileSize;

            var old = surface.Screen.TileAt(tile_x, tile_y);

            Flood(surface, tile_x, tile_y, old.Id, 0, 0);

            // need to manually inform the screen surface that I messed with it
            if (changes.Count > 0) surface.RaiseDrawnOn(new DrawAction("Fill", changes, surface));
            changes.Clear();
        }

        private void Flood(ScreenDrawingSurface surface, int tile_x, int tile_y, int tile_id, int brush_x, int brush_y)
        {
            var old = surface.Screen.TileAt(tile_x, tile_y);
            // checking whether this is already the new tile prevents infinite recursion, but
            // it can prevent filling a solid area with a brush that uses that same tile
            if (old == null || old.Id != tile_id || old.Id == cells[brush_x, brush_y].Id) return;

            surface.Screen.ChangeTile(tile_x, tile_y, cells[brush_x, brush_y].Id);
            changes.Add(new TileChange(tile_x, tile_y, tile_id, cells[brush_x, brush_y].Id, surface));

            Flood(surface, tile_x - 1, tile_y, tile_id, (brush_x == 0)? width-1 : brush_x - 1, brush_y);
            Flood(surface, tile_x + 1, tile_y, tile_id, (brush_x == width - 1) ? 0 : brush_x + 1, brush_y);
            Flood(surface, tile_x, tile_y - 1, tile_id, brush_x, (brush_y == 0) ? height - 1 : brush_y - 1);
            Flood(surface, tile_x, tile_y + 1, tile_id, brush_x, (brush_y == height - 1) ? 0 : brush_y + 1);
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
        public Image Icon { get { return null; } }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            ContextMenu menu = new ContextMenu();
            // find a join to modify
            foreach (var j in surface.Screen.Map.Joins)
            {
                MegaMan.Join join = j; // for lambda closure
                if (join.screenOne == surface.Screen.Name)
                {
                    if (join.type == MegaMan.JoinType.Vertical)
                    {
                        if (location.X > surface.Width - surface.Screen.Tileset.TileSize &&
                            location.Y >= (join.offsetOne * surface.Screen.Tileset.TileSize) &&
                            location.Y <= ((join.offsetOne + join.Size) * surface.Screen.Tileset.TileSize))
                        {
                            menu.MenuItems.Add(new MenuItem("Modify Left-Right Join " + join.screenOne + " to " + join.screenTwo,
                                (s, e) =>
                                {
                                    EditJoin(surface, join);
                                }));
                        }
                    }
                    else
                    {
                        if (location.Y > surface.Height - surface.Screen.Tileset.TileSize &&
                            location.X >= (join.offsetOne * surface.Screen.Tileset.TileSize) &&
                            location.X <= ((join.offsetOne + join.Size) * surface.Screen.Tileset.TileSize))
                        {
                            menu.MenuItems.Add(new MenuItem("Modify Up-Down Join " + join.screenOne + " to " + join.screenTwo,
                                (s, e) =>
                                {
                                    EditJoin(surface, join);
                                }));
                        }
                    }
                }
                else if (join.screenTwo == surface.Screen.Name)
                {
                    if (join.type == MegaMan.JoinType.Vertical)
                    {
                        if (location.X < surface.Screen.Tileset.TileSize &&
                            location.Y >= (join.offsetTwo * surface.Screen.Tileset.TileSize) &&
                            location.Y <= ((join.offsetTwo + join.Size) * surface.Screen.Tileset.TileSize))
                        {
                            menu.MenuItems.Add(new MenuItem("Modify Left-Right Join " + join.screenOne + " to " + join.screenTwo,
                                (s, e) =>
                                {
                                    EditJoin(surface, join);
                                }));
                        }
                    }
                    else
                    {
                        if (location.Y < surface.Screen.Tileset.TileSize &&
                            location.X >= (join.offsetTwo * surface.Screen.Tileset.TileSize) &&
                            location.X <= ((join.offsetTwo + join.Size) * surface.Screen.Tileset.TileSize))
                        {
                            menu.MenuItems.Add(new MenuItem("Modify Up-Down Join " + join.screenOne + " to " + join.screenTwo,
                                (s, e) =>
                                {
                                    EditJoin(surface, join);
                                }));
                        }
                    }
                }
            }

            if (location.X > surface.Width - surface.Screen.Tileset.TileSize)
            {
                menu.MenuItems.Add(new MenuItem("New Rightward Join from " + surface.Screen.Name,
                    (s, e) =>
                    {
                        NewJoin(surface, surface.Screen.Name, "", MegaMan.JoinType.Vertical, location.Y / surface.Screen.Tileset.TileSize);
                    }));
            }
            if (location.X < surface.Screen.Tileset.TileSize)
            {
                menu.MenuItems.Add(new MenuItem("New Leftward Join from " + surface.Screen.Name,
                    (s, e) =>
                    {
                        NewJoin(surface, "", surface.Screen.Name, MegaMan.JoinType.Vertical, location.Y / surface.Screen.Tileset.TileSize);
                    }));
            }
            if (location.Y > surface.Height - surface.Screen.Tileset.TileSize)
            {
                menu.MenuItems.Add(new MenuItem("New Downward Join from " + surface.Screen.Name,
                    (s, e) =>
                    {
                        NewJoin(surface, surface.Screen.Name, "", MegaMan.JoinType.Horizontal, location.X / surface.Screen.Tileset.TileSize);
                    }));
            }
            if (location.Y < surface.Screen.Tileset.TileSize)
            {
                menu.MenuItems.Add(new MenuItem("New Upward Join from " + surface.Screen.Name,
                    (s, e) =>
                    {
                        NewJoin(surface, "", surface.Screen.Name, MegaMan.JoinType.Horizontal, location.X / surface.Screen.Tileset.TileSize);
                    }));
            }

            menu.Show(surface, location);
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
        }

        public void Release(ScreenDrawingSurface surface, Point location)
        {
        }

        private void NewJoin(ScreenDrawingSurface surface, string s1, string s2, MegaMan.JoinType type, int offset)
        {
            MegaMan.Join newjoin = new MegaMan.Join();
            newjoin.screenTwo = s2;
            newjoin.screenOne = s1;
            newjoin.type = type;
            newjoin.Size = 1;
            newjoin.offsetOne = newjoin.offsetTwo = offset;
            JoinForm form = new JoinForm(newjoin, surface.Screen.Map.Screens);
            form.OK += () => { surface.Screen.Map.AddJoin(newjoin); };
            form.Show();
        }

        private void EditJoin(ScreenDrawingSurface surface, MegaMan.Join join)
        {
            JoinForm form = new JoinForm(join, surface.Screen.Map.Screens);
            form.OK += () => surface.Screen.Map.RaiseJoinChange(join);
            form.Show();
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
