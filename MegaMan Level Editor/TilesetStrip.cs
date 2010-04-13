using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan;
using System.Windows.Forms;
using System.Drawing;

namespace MegaMan_Level_Editor
{
    public class TilesetStrip : Panel
    {
        public TilesetStrip()
        {
            this.AutoSize = true;
            this.Padding = new Padding(4);

            Label selected = new Label();
            //selected.Margin = new Padding(6, 0, 10, 0);
            //selected.Padding = new Padding(0);
            //selected.BorderStyle = BorderStyle.None;
            //selected.Anchor = AnchorStyles.Left;
            selected.BackColor = Color.Red;
            selected.Text = "hahaha";
            this.Controls.Add(selected);
        }
    }
}
