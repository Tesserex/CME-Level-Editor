using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan_Level_Editor
{
    public class EntityTool : ITool
    {
        private Entity entity;

        public EntityTool(Entity entity)
        {
            this.entity = entity;
        }

        public System.Drawing.Image Icon
        {
            get { return entity.MainSprite[0].CutTile; }
        }

        public bool IconSnap { get { return false; } }

        public void Click(ScreenDrawingSurface surface, System.Drawing.Point location)
        {
            surface.Screen.AddEntity(entity, location);
            surface.RaiseDrawnOn(new AddEntityAction(entity, surface, location));
        }

        public void Move(ScreenDrawingSurface surface, System.Drawing.Point location)
        {
            
        }

        public void Release(ScreenDrawingSurface surface, System.Drawing.Point location)
        {
            
        }

        public System.Drawing.Point IconOffset
        {
            get { return new System.Drawing.Point(-entity.MainSprite.HotSpot.X, -entity.MainSprite.HotSpot.Y); }
        }
    }
}
