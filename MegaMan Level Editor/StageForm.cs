using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public partial class StageForm : Form
    {
        private MegaMan.Map stage;

        private History history;
        
        private Dictionary<string, ScreenDrawingSurface> surfaces;

        public bool DrawGrid
        {
            set
            {
                foreach (var pair in surfaces)
                {
                    pair.Value.DrawGrid = value;
                }
            }
        }

        public bool DrawTiles
        {
            set
            {
                foreach (var pair in surfaces)
                {
                    pair.Value.DrawTiles = value;
                }
            }
        }

        public bool DrawBlock
        {
            set
            {
                foreach (var pair in surfaces)
                {
                    pair.Value.DrawBlock = value;
                }
            }
        }

        public void Undo()
        {
            var action = history.Undo();

            if (action != null)
            {
                var previous = action.current.DrawOn(action.screen, action.x, action.y);

                // TODO: Use list combinators to select the first element that matches
                foreach (var surface in surfaces.Values)
                {
                    if (surface.Screen == action.screen)
                        surface.ReDrawAll();
                }
            }
        }

        public void Redo()
        {
            var action = history.Redo();

            if (action != null)
            {
                var future = action.current.DrawOn(action.screen, action.x, action.y);

                foreach (var surface in surfaces.Values)
                {
                    if (surface.Screen == action.screen)
                        surface.ReDrawAll();
                }
            }
        }

        public StageForm(MegaMan.Map stage)
        {
            InitializeComponent();

            this.SetBackgroundGrid();

            history = new History();
            surfaces = new Dictionary<String, ScreenDrawingSurface>();

            Program.FrameTick += new Action(Program_FrameTick);

            SetStage(stage);
        }

        private void SetBackgroundGrid()
        {
            // just need a tiny tile image
            Image tile = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(tile))
            {
                g.DrawLine(Pens.Black, 15, 0, 15, 15);
                g.DrawLine(Pens.Black, 0, 15, 15, 15);
            }
            sizingPanel.BackgroundImage = tile;
            sizingPanel.BackgroundImageLayout = ImageLayout.Tile;
        }

        private void RenameSurface(string oldScreenName, string newScreenName)
        {
            var surface = surfaces[oldScreenName];
            surfaces.Add(newScreenName, surface);
            surfaces.Remove(oldScreenName);
        }

        void Program_FrameTick()
        {
            foreach (var pair in surfaces)
            {
                pair.Value.ReDrawAll();
            }
        }

        /* *
         * SetText - Name the window
         * */
        public void SetText()
        {
            this.Text = this.stage.Name;
            if (this.stage.Dirty) this.Text += " *";
        }

        /* *
         * SetStage - Decide the stage object that will be edited
         * */
        private void SetStage(MegaMan.Map stage)
        {
            this.stage = stage;

            SetText();
            this.stage.DirtyChanged += (b) => SetText();

            foreach (var pair in stage.Screens)
            {
                var surface = CreateScreenSurface(pair.Value);
                surface.Location = new Point(0, 0);
            }

            AlignScreenSurfaces();
        }

        private void AlignScreenSurfaces()
        {
            foreach (var pair in surfaces)
            {
                if (stage.StartScreen == pair.Key)
                    pair.Value.Placed = true;
                else
                    pair.Value.Placed = false;
            }

            foreach (var join in stage.Joins)
            {
                AlignScreenSurfaceUsingJoin(surfaces[join.screenOne], surfaces[join.screenTwo], join);
            }
        }

        private void AlignScreenSurfaceUsingJoin(ScreenDrawingSurface surface, ScreenDrawingSurface secondSurface, Join join)
        {
            var offset = (join.offsetTwo - join.offsetOne) * join.Size;

            if (surface.Placed)
            {
                // TODO: WTF? Why does horizontal mean vertical and vertical mean horizontal?
                if (join.type == JoinType.Horizontal)
                {
                    // Place image below
                    var p = new Point(surface.Location.X - offset, surface.Location.Y + surface.Size.Height);
                    secondSurface.Location = p;
                }
                else
                {
                    // Place image to the right
                    var p = new Point(surface.Location.X + surface.Size.Width, surface.Location.Y - offset);
                    secondSurface.Location = p;
                }
                secondSurface.Placed = true;
            }
            else if (secondSurface.Placed)
            {
                if (join.type == JoinType.Horizontal)
                {
                    // Place image above
                    var p = new Point(secondSurface.Location.X - offset, secondSurface.Location.Y - surface.Size.Height);
                    surface.Location = p;
                }
                else
                {
                    // Place image to the left
                    var p = new Point(secondSurface.Location.X - surface.Size.Width, secondSurface.Location.Y - offset);
                    surface.Location = p;
                }
                surface.Placed = true;
            }
        }

        private ScreenDrawingSurface CreateScreenSurface(MegaMan.Screen screen)
        {
            var surface = new ScreenDrawingSurface(screen);
            surface.ReDrawAll();
            surfaces.Add(screen.Name, surface);
            screen.Renamed += this.RenameSurface;
            screen.Resized += (w, h) => this.AlignScreenSurfaces();
            surface.DrawnOn += new EventHandler<ScreenDrawEventArgs>(surface_DrawnOn);
            this.sizingPanel.Controls.Add(surface);
            return surface;
        }

        void surface_DrawnOn(object sender, ScreenDrawEventArgs e)
        {
            if (e.HistoryBrush != null)
            {
                history.Push(e.X, e.Y, e.Brush, e.HistoryBrush, e.Screen);
            }
        }
    }
}
