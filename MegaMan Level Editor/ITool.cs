using System;
using System.Drawing;

namespace MegaMan_Level_Editor
{
    public interface ITool
    {
        Image Icon { get; }
        bool IconSnap { get; }
        void Click(ScreenDrawingSurface surface, Point location);
        void Move(ScreenDrawingSurface surface, Point location);
        void Release(ScreenDrawingSurface surface);
        void RightClick(ScreenDrawingSurface surface, Point location);
        Point IconOffset { get; }
    }

    public class ToolChangedEventArgs : EventArgs
    {
        public ToolChangedEventArgs(ITool tool)
        {
        }
    }

    public enum ToolType
    {
        Brush,
        Bucket,
        Join,
        Start,
        Entity
    }
}
