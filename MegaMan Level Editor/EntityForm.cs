using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public partial class EntityForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public event Action<Entity> EntityChanged;

        public EntityForm()
        {
            InitializeComponent();
            Program.FrameTick += new Action(Program_FrameTick);
        }

        private void Program_FrameTick()
        {
            container.Refresh();
        }

        public void Deselect()
        {
            foreach (Control c in container.Controls) c.BackColor = container.BackColor;
        }

        public void LoadEntities(ProjectEditor project)
        {
            foreach (Entity entity in project.Entities)
            {
                if (entity.MainSprite == null) continue;

                var button = new EntityButton(entity);

                button.Click += (snd, args) =>
                {
                    if (EntityChanged != null) EntityChanged(button.Entity);
                    Deselect();
                    button.BackColor = Color.Orange;
                };

                container.Controls.Add(button);
            }
            container.Refresh();
        }

        public void Unload()
        {
            container.Controls.Clear();
        }
    }
}
