﻿using System;
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

        private bool dirty;
        public bool Dirty
        {
            get
            {
                return dirty;
            }
            set
            {
                dirty = value;
                Stage.Dirty = value;
            }
        }

        public string Name
        {
            get { return screen.Name; }
            set
            {
                if (value == screen.Name) return;

                string old = screen.Name;
                screen.Name = value;
                Dirty = true;
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
            Dirty = true;
            if (Resized != null) Resized(width, height);
        }

        public Tile TileAt(int x, int y)
        {
            return screen.TileAt(x, y);
        }

        public void ChangeTile(int tile_x, int tile_y, int tile_id)
        {
            screen.ChangeTile(tile_x, tile_y, tile_id);
            Dirty = true;
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
            Dirty = true;
        }

        public void RemoveEntity(Entity entity, Point location)
        {
            screen.EnemyInfo.RemoveAll(i =>
                i.enemy == entity.Name && i.screenX == location.X && i.screenY == location.Y
            );
        }

        public EnemyCopyInfo FindEntityAt(Point location)
        {
            return screen.EnemyInfo.FirstOrDefault(e => EntityBounded(e, location));
        }

        private bool EntityBounded(EnemyCopyInfo entityInfo, Point location)
        {
            Entity entity = Stage.Project.EntityByName(entityInfo.enemy);
            RectangleF bounds = entity.MainSprite.BoundBox;
            bounds.Offset(-entity.MainSprite.HotSpot.X, -entity.MainSprite.HotSpot.Y);
            bounds.Offset(entityInfo.screenX, entityInfo.screenY);
            return bounds.Contains(location);
        }

        public void DrawOn(Graphics graphics)
        {
            screen.Draw(graphics, 0, 0, screen.PixelWidth, screen.PixelHeight);
        }

        public void DrawEntities(Graphics graphics)
        {
            foreach (var info in screen.EnemyInfo)
            {
                Stage.Project.EntityByName(info.enemy).MainSprite.Draw(graphics, info.screenX, info.screenY);
            }
        }
    }
}
