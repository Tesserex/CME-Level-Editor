using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MegaMan_Level_Editor
{
    // all nodes in the project tree should have one of these as its tag
    public abstract class ProjectTreeHandler
    {
        protected TreeNode parentNode;

        public abstract void DoubleClick();
        public abstract void Properties();
        public abstract void Delete();
    }

    public class ProjectNodeHandler : ProjectTreeHandler
    {
        private ProjectEditor project;

        public ProjectNodeHandler(TreeNode node, ProjectEditor project)
        {
            this.parentNode = node;
            this.project = project;
        }

        public override void Properties()
        {
            new ProjectProperties(this.project).Show();
        }

        public override void Delete() { }

        public override void DoubleClick() { }
    }

    public class StageNodeHandler : ProjectTreeHandler
    {
        private ProjectEditor project;
        private MapDocument stage;
        private string stageName;

        public StageNodeHandler(TreeNode node, ProjectEditor project, string stageName)
        {
            this.parentNode = node;
            this.project = project;
            this.stage = null;
            this.stageName = stageName;
        }

        public StageNodeHandler(TreeNode node, ProjectEditor project, MapDocument stage)
        {
            this.parentNode = node;
            this.project = project;
            this.stage = stage;
            this.stageName = stage.Name;
        }

        public override void DoubleClick()
        {
            if (this.stage == null)
            {
                this.stage = this.project.StageByName(stageName);
                parentNode.Nodes.Clear();
                foreach (var screen in stage.Screens)
                {
                    var node = new TreeNode(screen.Name);
                    node.ImageIndex = node.SelectedImageIndex = 3;
                    node.Tag = new ScreenNodeHandler(node, screen);
                    parentNode.Nodes.Add(node);
                }

                stage.ScreenAdded += (screen) =>
                {
                    var node = new TreeNode(screen.Name);
                    node.Tag = new ScreenNodeHandler(node, screen);
                    parentNode.Nodes.Add(node);
                };
            }

            stage.ReFocus();
        }

        public override void Delete() { }

        public override void Properties()
        {
            var stageprop = new StageProp();
            stageprop.LoadMap(this.stage);
            stageprop.Show();
        }
    }

    public class ScreenNodeHandler : ProjectTreeHandler
    {
        private ScreenDocument screen;

        public ScreenNodeHandler(TreeNode node, ScreenDocument screen)
        {
            this.parentNode = node;
            this.screen = screen;
        }

        public override void DoubleClick() { }

        public override void Delete() { }

        public override void Properties()
        {
            new ScreenProp(this.screen).Show();
        }
    }
}
