using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public class JoinOverlay : CodeProject.GraphicalOverlay
    {
        private static Pen passPen = new Pen(Color.Blue, 4);
        private static Pen blockPen = new Pen(Color.Red, 4);
        private Bitmap image;

        public JoinOverlay()
        {
            this.Paint += new EventHandler<PaintEventArgs>(JoinOverlay_Paint);
        }

        void JoinOverlay_Paint(object sender, PaintEventArgs e)
        {
            Control c = sender as Control;
            Point loc;
            if (c is ScreenDrawingSurface) loc = c.Location;
            else loc = new Point(0, 0);
            // when scrolling, the actual Location property of the control changes. So to compensate,
            // we have to also subtract the scroll amount so that the image is always drawn on right area of the control.
            e.Graphics.DrawImageUnscaled(image, -loc.X - Owner.HorizontalScroll.Value, -loc.Y - Owner.VerticalScroll.Value, c.Width, c.Height);
        }

        public void Refresh(int width, int height, IEnumerable<Join> joins, IDictionary<string, ScreenDrawingSurface> surfaces)
        {
            if (image == null || image.Height != height || image.Width != width)
            {
                if (image != null) image.Dispose();
                image = new Bitmap(width, height);
            }

            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(Color.Transparent);
                foreach (Join join in joins)
                {
                    if (surfaces.ContainsKey(join.screenOne))
                    {
                        DrawJoinEnd(g, surfaces[join.screenOne], join, true);
                    }
                    if (surfaces.ContainsKey(join.screenTwo))
                    {
                        DrawJoinEnd(g, surfaces[join.screenTwo], join, false);
                    }
                }
            }
            Invalidate();
        }

        private void DrawJoinEnd(Graphics g, ScreenDrawingSurface surface, Join join, bool one)
        {
            int offset = one ? join.offsetOne : join.offsetTwo;
            int start = (join.type == JoinType.Horizontal)? surface.Left: surface.Top;
            start += offset * 16;
            int end = start + (join.Size * 16);
            int edge;
            Pen pen;
            if (one ? join.direction == JoinDirection.BackwardOnly : join.direction == JoinDirection.ForwardOnly) pen = blockPen;
            else pen = passPen;
            if (join.type == JoinType.Horizontal)
            {
                edge = one ? surface.Bottom - 2 : surface.Top + 2;
                int curl = one ? edge - 6 : edge + 6;
                g.DrawLine(pen, start, edge, end, edge);
                g.DrawLine(pen, start+1, edge, start+1, curl);
                g.DrawLine(pen, end-1, edge, end-1, curl);
            }
            else
            {
                edge = one ? surface.Right - 2 : surface.Left + 2;
                int curl = one ? edge - 6 : edge + 6;
                g.DrawLine(pen, edge, start, edge, end);
                g.DrawLine(pen, edge, start, curl, start);
                g.DrawLine(pen, edge, end, curl, end);
            }
        }
    }
}
