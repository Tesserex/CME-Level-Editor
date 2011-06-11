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
        bool IconSnap { get; }
        void Click(ScreenDrawingSurface surface, Point location);
        void Move(ScreenDrawingSurface surface, Point location);
        void Release(ScreenDrawingSurface surface, Point location);
        Point IconOffset { get; }
    }

    public class BrushTool : ITool
    {
        private ITileBrush brush;
        private bool held;
        private Point currentTilePos;
        private List<TileChange> changes;

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

        public void Release(ScreenDrawingSurface surface, Point location)
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
    }

    public class Bucket : ITool
    {
        private ITileBrush brush;
        private MegaMan.Tile[,] cells;
        private int width, height;
        private List<TileChange> changes;

        public Image Icon { get; private set; }
        public Point IconOffset { get { return Point.Empty; } }
        public bool IconSnap { get { return true; } }

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
            if (changes.Count > 0)
            {
                surface.EditedWithAction(new DrawAction("Fill", changes, surface));
                surface.ReDrawTiles();
            }
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
        public Point IconOffset { get { return Point.Empty; } }
        public bool IconSnap { get { return false; } }

        private MegaMan.Join NearestJoin(ScreenDrawingSurface surface, Point location)
        {
            MegaMan.Join nearest = null;

            foreach (var join in surface.Screen.Stage.Joins)
            {
                if (join.screenOne == surface.Screen.Name)
                {
                    int begin = join.offsetOne * surface.Screen.Tileset.TileSize;
                    int end = (join.offsetOne + join.Size) * surface.Screen.Tileset.TileSize;
                    if (join.type == MegaMan.JoinType.Vertical)
                    {
                        if (location.X > surface.Width - surface.Screen.Tileset.TileSize && location.Y >= begin && location.Y <= end)
                        {
                            nearest = join;
                        }
                    }
                    else
                    {
                        if (location.Y > surface.Height - surface.Screen.Tileset.TileSize && location.X >= begin && location.X <= end)
                        {
                            nearest = join;
                        }
                    }
                }
                else if (join.screenTwo == surface.Screen.Name)
                {
                    int begin = join.offsetTwo * surface.Screen.Tileset.TileSize;
                    int end = (join.offsetTwo + join.Size) * surface.Screen.Tileset.TileSize;
                    if (join.type == MegaMan.JoinType.Vertical)
                    {
                        if (location.X < surface.Screen.Tileset.TileSize && location.Y >= begin && location.Y <= end)
                        {
                            nearest = join;
                        }
                    }
                    else
                    {
                        if (location.Y < surface.Screen.Tileset.TileSize && location.X >= begin && location.X <= end)
                        {
                            nearest = join;
                        }
                    }
                }
            }
            return nearest;
        }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            ContextMenu menu = new ContextMenu();

            // find a join to modify
            var nearest = NearestJoin(surface, location);

            if (nearest != null)
            {
                string typeText = (nearest.type == MegaMan.JoinType.Vertical)? "Left-Right" : "Up-Down";

                menu.MenuItems.Add("Modify " + typeText + " Join from " + nearest.screenOne + " to " + nearest.screenTwo, (s, e) => { EditJoin(surface, nearest); });

                menu.MenuItems.Add("Delete " + typeText + " Join from " + nearest.screenOne + " to " + nearest.screenTwo, (s, e) => { DeleteJoin(surface, nearest); });
            }
            else
            {
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
            JoinForm form = new JoinForm(newjoin, surface.Screen.Stage.Screens);
            form.OK += () => { surface.Screen.Stage.AddJoin(newjoin); };
            form.Show();
        }

        private void EditJoin(ScreenDrawingSurface surface, MegaMan.Join join)
        {
            JoinForm form = new JoinForm(join, surface.Screen.Stage.Screens);
            form.OK += () => surface.Screen.Stage.RaiseJoinChange(join);
            form.Show();
        }

        private void DeleteJoin(ScreenDrawingSurface surface, MegaMan.Join join)
        {
            surface.Screen.Stage.RemoveJoin(join);
        }
    }

    public class StartPositionTool : ITool
    {
        private static Bitmap icon;

        static StartPositionTool()
        {
            icon = new Bitmap(21,24);
            icon.SetResolution(96, 96);
            using (Graphics g = Graphics.FromImage(icon))
            {
                g.DrawImage(Properties.Resources.megaman, 0, 0, 21, 24);
            }
        }

        public static Image MegaMan
        {
            get { return icon; }
        }

        #region ITool Members

        public Image Icon
        {
            get
            {
                return icon;
            }
        }
        public Point IconOffset { get { return new Point(-4, -8); } }
        public bool IconSnap { get { return false; } }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            int px = (location.X / surface.Screen.Tileset.TileSize) * surface.Screen.Tileset.TileSize;
            int py = (location.Y / surface.Screen.Tileset.TileSize) * surface.Screen.Tileset.TileSize + 4;

            surface.Screen.Stage.StartScreen = surface.Screen.Name;
            surface.Screen.Stage.StartPoint = new Point(px, py);
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
            
        }

        public void Release(ScreenDrawingSurface surface, Point location)
        {
            
        }

        #endregion
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
        Join,
        Start,
        Entity
    }
}
