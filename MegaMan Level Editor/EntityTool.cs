using System.Drawing;

namespace MegaMan_Level_Editor
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

        public bool IconSnap { get { return false; } }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            var action = new AddEntityAction(entity, surface, location);
            action.Run();
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
            // delete nearest entity
            var info = surface.Screen.FindEntityAt(location);
            if (info.enemy == null) return;
            
            var nearest = surface.Screen.Stage.Project.EntityByName(info.enemy);

            var action = new RemoveEntityAction(nearest, surface, new Point((int)info.screenX, (int)info.screenY));
            action.Run();
            surface.EditedWithAction(action);
        }
    }
}
