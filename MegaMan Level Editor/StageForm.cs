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
    public partial class StageForm : Form {

        public static Brush blockBrush  = new SolidBrush(Color.FromArgb(160, Color.OrangeRed));
        public static Brush ladderBrush = new SolidBrush(Color.FromArgb(160, Color.Yellow));
        public Pen highlightPen = new Pen(Color.Green, 2);

        public MegaMan.Map stage;
        public string stageName;

        public History history;
        public ITileBrush currentBrush = null;
        public Dictionary<string, ScreenDrawingSurface> surfaces;
        
        public bool DrawGrid {
            set {
                foreach (var pair in surfaces) {
                    pair.Value.DrawGrid = value;
                }
            }
        }

        public bool DrawTiles {
            set {
                foreach (var pair in surfaces) {
                    pair.Value.DrawTiles = value;
                }
            }
        }

        public bool DrawBlock {
            set {
                foreach (var pair in surfaces) {
                    pair.Value.DrawBlock = value;
                }
            }
        }



        /* *
         * DrawAction - An action that is saved into history
         * */
        public class DrawAction {
            public int x, y;
            public ITileBrush current, previous;
            public MegaMan.Screen screen;

            public DrawAction(int x, int y, ITileBrush current, ITileBrush previous, MegaMan.Screen screen) {
                this.x = x;
                this.y = y;
                this.current = current;
                this.previous = previous;
                this.screen = screen;
            }

            override public String ToString() {
                return "(x, y) : (" + x + "," + y + ")  current : " + current.ToString() + " previous : " + previous.ToString();
            }

            public DrawAction Reverse() {
                return new DrawAction(x, y, previous, current, screen);
            }
        }

        /* *
         * History - Saves previous actions for "undo"/"redo" functionality
         * */
        public class History {
            // TODO: Make the stack a type "Action[]" where an Action is anything
            // we may want to undo.
            public List<DrawAction> stack;
            public int currentAction;

            public History() {
                this.currentAction = -1;
                this.stack = new List<DrawAction>();
            }

            /*
             * push - Adds a new action to the stack. If the stack already has actions after the current,
             * we delete them.
             * 
             * Ex: 
             * 
             * Suppose the current history looks like this:
             * 
             *   (1,4,Wall) <- currentAction
             *   (1,3,Wall)
             *   (1,2,Wall)
             *   (1,1,Wall)
             *   
             * Then we execute two undos to get
             * 
             *   (1,4,Wall)
             *   (1,3,Wall)
             *   (1,2,Wall) <- currentAction
             *   (1,1,Wall)
             *   
             * So what happens if we push another onto the stack? We have to delete the ones *after* current
             * and push it to the stack.
             *
             *   (1,3,Enemy) <- currentAction
             *   (1,2,Wall)  
             *   (1,1,Wall)
             *   
             * This behavior would be consistent with popular graphics programs like Photoshop and GIMP
             * 
             * */
            public void Push(int x, int y, ITileBrush current, ITileBrush previous, MegaMan.Screen screen) {
                currentAction += 1;
                stack.Add(new DrawAction(x, y, current, previous, screen));
                UpdateHistoryForm();
           }
            
            public DrawAction Undo() {
                if (currentAction >= 0) {
                    var action = stack[currentAction];
                    currentAction -= 1;
//                    stack.RemoveAt(stack.Count - 1);                    
                    UpdateHistoryForm();
                    return action.Reverse();
                } else {
                    return null;
                }                
            }

            public DrawAction Redo() {
                if (currentAction < stack.Count) {
                    currentAction += 1;
                    var action = stack[currentAction];
                    UpdateHistoryForm();
                    return action;
                } else {
                    return null;
                }
            }

            public void UpdateHistoryForm() {
                MainForm.Instance.historyForm.UpdateHistory(this);
            }
        }

        //TODO: Move this back into StageForm
        public void DrawTile(int x, int y, ScreenDrawingSurface surface) {
            if (!surface.drawing || currentBrush == null)
                return;

            var previous = currentBrush.DrawOn(surface.Screen, x, y);
            if (previous != null) {
                history.Push(x, y, currentBrush, previous, surface.Screen);
            }

            surface.ReDrawAll();
        }

        public void Undo() {
            var action = history.Undo();

            if (action != null) {
                var previous = action.current.DrawOn(action.screen, action.x, action.y);

                // TODO: Use list combinators to select the first element that matches
                foreach (var surface in surfaces.Values) {
                    if (surface.Screen == action.screen)
                        surface.ReDrawAll();
                }
            }
        }

        public void Redo() {
            var action = history.Redo();

            if (action != null) {
                var future = action.current.DrawOn(action.screen, action.x, action.y);

                foreach (var surface in surfaces.Values) {
                    if (surface.Screen == action.screen)
                        surface.ReDrawAll();
                }
            }
        }


        public string Path { get; private set; }

        public StageForm(MegaMan.Map stage) {
            InitializeComponent();

            history   = new History();
            surfaces  = new Dictionary<String, ScreenDrawingSurface>();

            Program.FrameTick += new Action(Program_FrameTick);

            MainForm.Instance.BrushChanged += new BrushChangedHandler(parent_BrushChanged);
            MainForm.Instance.stageForms.Add(stage.Name, this);
            SetStage(stage);
        }

        public void RenameSurface(string oldScreenName, string newScreenName) {
            var surface = surfaces[oldScreenName];
            surface.screenName = newScreenName;
            surfaces.Add(newScreenName, surface);
            surfaces.Remove(oldScreenName);
        }

        void parent_BrushChanged(BrushChangedEventArgs e) {
            SetBrush(e.Brush);
        }

        void Program_FrameTick() {
            foreach (var pair in surfaces) {
                pair.Value.ReDrawAll();
            }
        }

        /* *
         * SetText - Name the window
         * */
        public void SetText() {
            this.Text = this.stage.Name;
            if (this.stage.Dirty) this.Text += " *";
        }

        /* *
         * SetStage - Decide the stage object that will be edited
         * */
        public void SetStage(MegaMan.Map stage) {
            this.stage = stage;
            this.stageName = stage.Name;

            SetText();
            this.stage.DirtyChanged += (b) => SetText();

            foreach (var pair in stage.Screens) {              
                var surface = CreateScreenSurface(stage.Name, pair.Key);
                surface.screenImage.Location = new Point(0, 0);
            }

            AlignScreenSurfaces();
        }

        public void AlignScreenSurfaces() {
            foreach (var pair in surfaces) {
                if (stage.StartScreen == pair.Key)
                    pair.Value.placed = true;
                else
                    pair.Value.placed = false;
            }

            foreach (var join in stage.Joins) {
                AlignScreenSurfaceUsingJoin(surfaces[join.screenOne], surfaces[join.screenTwo], join);
            }
        }

        public void AlignScreenSurfaceUsingJoin(ScreenDrawingSurface surface, ScreenDrawingSurface secondSurface, Join join) {
            var offset = (join.offsetTwo - join.offsetOne) * join.Size;

            if (surface.placed) {
                // TODO: WTF? Why does horizontal mean vertical and vertical mean horizontal?
                if (join.type == JoinType.Horizontal) {
                    // Place image below
                    var p = new Point(surface.screenImage.Location.X - offset, surface.screenImage.Location.Y + surface.screenImage.Size.Height);
                    secondSurface.screenImage.Location = p;
                } else {
                    // Place image to the right
                    var p = new Point(surface.screenImage.Location.X + surface.screenImage.Size.Width, surface.screenImage.Location.Y - offset);
                    secondSurface.screenImage.Location = p;
                }
                secondSurface.placed = true;
            } else if (secondSurface.placed) {
                if (join.type == JoinType.Horizontal) {
                    // Place image above
                    var p = new Point(secondSurface.screenImage.Location.X - offset, secondSurface.screenImage.Location.Y - surface.screenImage.Size.Height);
                    surface.screenImage.Location = p;
                } else {
                    // Place image to the left
                    var p = new Point(secondSurface.screenImage.Location.X - surface.screenImage.Size.Width, secondSurface.screenImage.Location.Y - offset);
                    surface.screenImage.Location = p;
                }
                surface.placed = true;
            }
        }

        public ScreenDrawingSurface CreateScreenSurface(string stageName, string screenName) {
            var surface = new ScreenDrawingSurface(stageName, screenName, this);
            surface.ReDrawAll();
            surfaces.Add(screenName, surface);
            return surface;
        }

        private void SetBrush(ITileBrush brush) {
            currentBrush = brush;
        }

        /* *
         * Redo - Reiterate the history stack
         * */


        public void StageForm_Load(object sender, EventArgs e) {

        }
        
        public void StageForm_GotFocus(object sender, EventArgs e) {
            MainForm.Instance.currentStageForm = this;
            // MessageBox.Show("I just got focus! " + this.stageName);
        }
    }
}
