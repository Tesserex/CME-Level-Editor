using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public class JoinOverlay : CodeProject.GraphicalOverlay
    {
        private static Pen passPen = new Pen(Color.Blue, 4);
        private static Pen blockPen = new Pen(Color.Red, 4);
        private static Pen joinPen = new Pen(Color.Green, 4);
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

                    if (surfaces.ContainsKey(join.screenOne) && surfaces.ContainsKey(join.screenTwo))
                    {
                        int mid = join.Size * 8;  // the 8 is from tilesize (16) / 2 for midpoint
                        int midOffsetOne = join.offsetOne * 16 + mid;
                        int midOffsetTwo = join.offsetTwo * 16 + mid;

                        if (join.type == JoinType.Horizontal)
                        {
                            if (surfaces[join.screenOne].Bottom != surfaces[join.screenTwo].Top)
                            {
                                int y1 = surfaces[join.screenOne].Left + midOffsetOne;
                                int y2 = surfaces[join.screenTwo].Left + midOffsetTwo;

                                int x1 = surfaces[join.screenOne].Bottom;
                                int x2 = surfaces[join.screenTwo].Top;

                                DrawJoinPath(g, x1, x2, y1, y2, true);
                            }
                        }
                        else
                        {
                            if (surfaces[join.screenOne].Right != surfaces[join.screenTwo].Left)
                            {
                                int x1 = surfaces[join.screenOne].Right;
                                int x2 = surfaces[join.screenTwo].Left;

                                int y1 = surfaces[join.screenOne].Top + midOffsetOne;
                                int y2 = surfaces[join.screenTwo].Top + midOffsetTwo;

                                DrawJoinPath(g, x1, x2, y1, y2, false);
                            }
                        }

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

        private void DrawJoinPath(Graphics g, int x1, int x2, int y1, int y2, bool transpose)
        {
            Point start, end, midpoint, c1, c2;

            start = new Point(x1, y1);
            end = new Point(x2, y2);

            int skew = y2 - y1;
            if (skew > 32)
            {
                int halfskew = skew / 2;
                int qtrskew = halfskew / 2;
                midpoint = new Point(x1 + (x2 - x1) / 2, y1 + halfskew);
                c1 = new Point(x1 + 8, y1 + qtrskew);
                c2 = new Point(x2 - 8, y2 - qtrskew);
            }
            else
            {
                midpoint = new Point(x1 + (x2 - x1) / 2, Math.Max(y1, y2) + 32);
                c1 = new Point(x1 + 8, y1 + 16);
                c2 = new Point(x2 - 8, y2 + 16);
            }

            if (transpose)
            {
                start = new Point(start.Y, start.X);
                c1 = new Point(c1.Y, c1.X);
                c2 = new Point(c2.Y, c2.X);
                midpoint = new Point(midpoint.Y, midpoint.X);
                end = new Point(end.Y, end.X);
            }

            GraphicsPath path = new GraphicsPath();
            path.AddCurve(new Point[] { start, c1, midpoint, c2, end }, 0.5f);
            g.DrawPath(joinPen, path);
        }
    }
}
