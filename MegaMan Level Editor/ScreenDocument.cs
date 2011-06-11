using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan;
using System.Drawing;

namespace MegaMan_Level_Editor
{
    public class ScreenDocument
    {
        private Screen screen;

        public StageDocument Stage { get; private set; }

        public event Action<int, int> Resized;
        public event Action TileChanged;

        public string Name
        {
            get { return screen.Name; }
            set
            {
                string old = screen.Name;
                screen.Name = value;
                if (Renamed != null) Renamed(old, value);
            }
        }

        public int Width
        {
            get { return screen.Width; }
        }

        public int Height
        {
            get { return screen.Height; }
        }

        public Tileset Tileset { get { return screen.Tileset; } }
        public int PixelWidth { get { return screen.PixelWidth; } }
        public int PixelHeight { get { return screen.PixelHeight; } }

        public event Action<string, string> Renamed;

        public ScreenDocument(MegaMan.Screen screen, StageDocument stage)
        {
            this.Stage = stage;
            this.screen = screen;
        }

        public void Resize(int width, int height)
        {
            screen.Resize(width, height);
            if (Resized != null) Resized(width, height);
        }

        public Tile TileAt(int x, int y)
        {
            return screen.TileAt(x, y);
        }

        public void ChangeTile(int tile_x, int tile_y, int tile_id)
        {
            screen.ChangeTile(tile_x, tile_y, tile_id);
            if (TileChanged != null) TileChanged();
        }

        public void ChangeTile(int tile_x, int tile_y, Tile tile)
        {
            ChangeTile(tile_x, tile_y, tile.Id);
        }

        public void AddEntity(Entity entity, Point location)
        {
            screen.AddEnemy(new EnemyCopyInfo
                {
                    enemy = entity.Name,
                    screenX = location.X,
                    screenY = location.Y,
                }
            );
        }

        public void DrawOn(System.Drawing.Graphics graphics)
        {
            screen.Draw(graphics, 0, 0, screen.PixelWidth, screen.PixelHeight);
            foreach (var info in screen.EnemyInfo)
            {
                Stage.Project.EntityByName(info.enemy).MainSprite.Draw(graphics, info.screenX, info.screenY);
            }
        }
    }
}
