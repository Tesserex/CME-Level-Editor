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
        public MegaMan.Map stage;
        public string stageName;

        private History history;
        private ITileBrush currentBrush = null;
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

        public ScreenDrawingSurface GetSurface(string name)
        {
            if (!surfaces.ContainsKey(name)) return null;
            return surfaces[name];
        }

        //TODO: Move this back into StageForm
        public void DrawTile(int x, int y, ScreenDrawingSurface surface)
        {
            if (!surface.Drawing || currentBrush == null)
                return;

            var previous = currentBrush.DrawOn(surface.Screen, x, y);
            if (previous != null)
            {
                history.Push(x, y, currentBrush, previous, surface.Screen);
            }

            surface.ReDrawAll();
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


        public string Path { get; private set; }

        public StageForm(MegaMan.Map stage)
        {
            InitializeComponent();

            this.SetBackgroundGrid();

            history = new History();
            surfaces = new Dictionary<String, ScreenDrawingSurface>();

            Program.FrameTick += new Action(Program_FrameTick);

            MainForm.Instance.BrushChanged += new BrushChangedHandler(parent_BrushChanged);
            MainForm.Instance.stageForms.Add(stage.Name, this);
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

        public void RenameSurface(string oldScreenName, string newScreenName)
        {
            var surface = surfaces[oldScreenName];
            surface.screenName = newScreenName;
            surfaces.Add(newScreenName, surface);
            surfaces.Remove(oldScreenName);
        }

        void parent_BrushChanged(BrushChangedEventArgs e)
        {
            SetBrush(e.Brush);
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
        public void SetStage(MegaMan.Map stage)
        {
            this.stage = stage;
            this.stageName = stage.Name;

            SetText();
            this.stage.DirtyChanged += (b) => SetText();

            foreach (var pair in stage.Screens)
            {
                var surface = CreateScreenSurface(stage.Name, pair.Key);
                surface.screenImage.Location = new Point(0, 0);
            }

            AlignScreenSurfaces();
        }

        public void AlignScreenSurfaces()
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

        public void AlignScreenSurfaceUsingJoin(ScreenDrawingSurface surface, ScreenDrawingSurface secondSurface, Join join)
        {
            var offset = (join.offsetTwo - join.offsetOne) * join.Size;

            if (surface.Placed)
            {
                // TODO: WTF? Why does horizontal mean vertical and vertical mean horizontal?
                if (join.type == JoinType.Horizontal)
                {
                    // Place image below
                    var p = new Point(surface.screenImage.Location.X - offset, surface.screenImage.Location.Y + surface.screenImage.Size.Height);
                    secondSurface.screenImage.Location = p;
                }
                else
                {
                    // Place image to the right
                    var p = new Point(surface.screenImage.Location.X + surface.screenImage.Size.Width, surface.screenImage.Location.Y - offset);
                    secondSurface.screenImage.Location = p;
                }
                secondSurface.Placed = true;
            }
            else if (secondSurface.Placed)
            {
                if (join.type == JoinType.Horizontal)
                {
                    // Place image above
                    var p = new Point(secondSurface.screenImage.Location.X - offset, secondSurface.screenImage.Location.Y - surface.screenImage.Size.Height);
                    surface.screenImage.Location = p;
                }
                else
                {
                    // Place image to the left
                    var p = new Point(secondSurface.screenImage.Location.X - surface.screenImage.Size.Width, secondSurface.screenImage.Location.Y - offset);
                    surface.screenImage.Location = p;
                }
                surface.Placed = true;
            }
        }

        public ScreenDrawingSurface CreateScreenSurface(string stageName, string screenName)
        {
            var surface = new ScreenDrawingSurface(stageName, screenName, this);
            surface.ReDrawAll();
            surfaces.Add(screenName, surface);
            return surface;
        }

        private void SetBrush(ITileBrush brush)
        {
            currentBrush = brush;
        }

        public void StageForm_GotFocus(object sender, EventArgs e)
        {
            MainForm.Instance.currentStageForm = this;
        }
    }
}
