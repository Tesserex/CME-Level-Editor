using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan_Level_Editor
{
    public interface ICommand
    {
        void Execute();
    }

    public class UndoRedo
    {
        private Stack<ICommand> undoStack, redoStack;
        private bool undoing;   // a state that says whether executing commands are due to an undo
        private bool redoing;   // same idea

        public UndoRedo()
        {
            undoStack = new Stack<ICommand>();
            redoStack = new Stack<ICommand>();
            undoing = false;
            redoing = false;
        }

        public void Clear()
        {
            undoStack.Clear();
            redoStack.Clear();
        }

        public void Commit(ICommand command)
        {
            // if we're undoing, it means this was submitted because of the undo - hence it should go on the REDO stack
            if (undoing) redoStack.Push(command);
            // in the normal case, put it on the undo stack
            else
            {
                undoStack.Push(command);
                // only clear the redo stack if the action wasn't due to a redo
                if (!redoing) redoStack.Clear();
            }
        }

        public void Undo()
        {
            if (undoStack.Count == 0) return;
            undoing = true;
            ICommand c = undoStack.Pop();
            c.Execute();
            undoing = false;
        }

        public void Redo()
        {
            if (redoStack.Count == 0) return;
            redoing = true;
            ICommand c = redoStack.Pop();
            c.Execute();
            redoing = false;
        }
    }
}
