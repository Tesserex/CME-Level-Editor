﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan;
using System.Windows.Forms;
using System.Drawing;

namespace MegaMan_Level_Editor
{
    public class TilesetStrip : ToolStrip
    {
        private Tileset tileset;
        private TileButton selected;
        private ToolStripSeparator sep;

        public event Action<Tile> TileChanged;

        public TilesetStrip()
        {
            this.AutoSize = true;
            this.Padding = new Padding(0);
            this.Margin = new Padding(0);

            selected = new TileButton(null);
            selected.Margin = new Padding(10, 0, 5, 0);
            selected.Padding = new Padding(0);
            this.Items.Add(selected);

            sep = new ToolStripSeparator();
            sep.Margin = new Padding(5, 2, 10, 2);
            sep.Padding = new Padding(0);
            this.Items.Add(sep);

            Program.FrameTick += new Action(TickSprites);
        }

        public void ChangeTileset(Tileset tileset)
        {
            this.Items.Clear();

            this.tileset = tileset;
            if (tileset == null) return;

            selected.Size = new Size(tileset.TileSize, tileset.TileSize);
            this.Items.Add(selected);
            this.Items.Add(sep);

            foreach (Tile tile in tileset)
            {
                TileButton button = new TileButton(tile);
                button.Click += new EventHandler(button_Click);
                this.Items.Add(button);
            }
        }

        private void TickSprites()
        {
            if (tileset != null)
            {
                foreach (Tile tile in tileset)
                {
                    tile.Sprite.Update();
                }

                
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            TileButton button = sender as TileButton;
            selected.Tile = button.Tile;
            selected.Invalidate();

            if (TileChanged != null) TileChanged(selected.Tile);
        }
    }
}