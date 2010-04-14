using System;
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
        private TileButton selected;
        private ToolStripSeparator sep;

        public TilesetStrip()
        {
            this.AutoSize = true;
            this.Padding = new Padding(4);

            selected = new TileButton(null);
            selected.Margin = new Padding(10, 0, 5, 0);
            selected.Padding = new Padding(0);
            this.Items.Add(selected);

            sep = new ToolStripSeparator();
            sep.Margin = new Padding(5, 2, 10, 2);
            sep.Padding = new Padding(0);
            this.Items.Add(sep);
        }

        public void ChangeTileset(Tileset tileset)
        {
            this.Items.Clear();

            selected.Size = new Size(tileset.TileSize, tileset.TileSize);
            this.Items.Add(selected);
            this.Items.Add(sep);

            foreach (Tile tile in tileset)
            {
                this.Items.Add(new TileButton(tile));
            }
        }
    }
}
