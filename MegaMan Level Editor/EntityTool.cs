using System.Drawing;

namespace MegaMan.LevelEditor
{
    public class EntityTool : ITool
    {
        private readonly Entity entity;

        public EntityTool(Entity entity)
        {
            this.entity = entity;
        }

        public Image Icon
        {
            get { return entity.MainSprite[0].CutTile; }
        }

        public bool IsIconCursor { get { return false; } }

        public bool IconSnap { get { return false; } }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            var info = surface.Screen.AddEntity(entity, location);
            var action = new AddEntityAction(info, surface);
            surface.EditedWithAction(action);
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
            
        }

        public void Release(ScreenDrawingSurface surface)
        {
            
        }

        public Point IconOffset
        {
            get { return new Point(-entity.MainSprite.HotSpot.X, -entity.MainSprite.HotSpot.Y); }
        }

        public void RightClick(ScreenDrawingSurface surface, Point location)
        {
            
        }
    }
}
