using System;
using System.Collections.Generic;

namespace MegaMan_Level_Editor
{
    /* *
    * DrawAction - An action that is saved into history
    * */
    public class DrawAction
    {
        public int x, y;
        public ITileBrush current, previous;
        public MegaMan.Screen screen;

        public DrawAction(int x, int y, ITileBrush current, ITileBrush previous, MegaMan.Screen screen)
        {
            this.x = x;
            this.y = y;
            this.current = current;
            this.previous = previous;
            this.screen = screen;
        }

        override public String ToString()
        {
            return "(x, y) : (" + x + "," + y + ")  current : " + current.ToString() + " previous : " + previous.ToString();
        }

        public DrawAction Reverse()
        {
            return new DrawAction(x, y, previous, current, screen);
        }
    }

    /* *
    * History - Saves previous actions for "undo"/"redo" functionality
    * */
    public class History
    {
        // TODO: Make the stack a type "Action[]" where an Action is anything
        // we may want to undo.
        public List<DrawAction> stack;
        public int currentAction;

        public History()
        {
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
        public void Push(int x, int y, ITileBrush current, ITileBrush previous, MegaMan.Screen screen)
        {
            currentAction += 1;
            stack.Add(new DrawAction(x, y, current, previous, screen));
            UpdateHistoryForm();
        }

        public DrawAction Undo()
        {
            if (currentAction >= 0)
            {
                var action = stack[currentAction];
                currentAction -= 1;
                //                    stack.RemoveAt(stack.Count - 1);                    
                UpdateHistoryForm();
                return action.Reverse();
            }
            else
            {
                return null;
            }
        }

        public DrawAction Redo()
        {
            if (currentAction < stack.Count)
            {
                currentAction += 1;
                var action = stack[currentAction];
                UpdateHistoryForm();
                return action;
            }
            else
            {
                return null;
            }
        }

        public void UpdateHistoryForm()
        {
            MainForm.Instance.historyForm.UpdateHistory(this);
        }
    }
}