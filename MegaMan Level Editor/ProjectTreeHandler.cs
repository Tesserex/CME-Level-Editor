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
        public ProjectEditor Project { get; private set; }

        public ProjectTreeHandler(ProjectEditor project, TreeNode node)
        {
            this.Project = project;
            this.parentNode = node;
        }

        public abstract void DoubleClick();
        public abstract void Properties();
        public abstract void Delete();
    }

    public class ProjectNodeHandler : ProjectTreeHandler
    {
        public ProjectNodeHandler(ProjectEditor project, TreeNode node) : base(project, node) { }

        public override void Properties()
        {
            new ProjectProperties(this.Project).Show();
        }

        public override void Delete() { }

        public override void DoubleClick() { }
    }

    public class StageNodeHandler : ProjectTreeHandler
    {
        private MapDocument stage;
        private string stageName;

        public StageNodeHandler(ProjectEditor project, TreeNode node, string stageName) : base(project, node)
        {
            this.stage = null;
            this.stageName = stageName;
        }

        public StageNodeHandler(ProjectEditor project, TreeNode node, MapDocument stage) : base(project, node)
        {
            this.stage = stage;
            this.stageName = stage.Name;
        }

        public override void DoubleClick()
        {
            if (this.stage == null)
            {
                this.stage = this.Project.StageByName(stageName);
                parentNode.Nodes.Clear();
                foreach (var screen in stage.Screens)
                {
                    var node = new TreeNode(screen.Name);
                    node.ImageIndex = node.SelectedImageIndex = 3;
                    node.Tag = new ScreenNodeHandler(this.Project, node, screen);
                    parentNode.Nodes.Add(node);
                }

                stage.ScreenAdded += (screen) =>
                {
                    var node = new TreeNode(screen.Name);
                    node.Tag = new ScreenNodeHandler(this.Project, node, screen);
                    parentNode.Nodes.Add(node);
                };
            }

            stage.ReFocus();
        }

        public override void Delete() { }

        public override void Properties()
        {
            StageProp.EditStage(this.stage);
        }
    }

    public class ScreenNodeHandler : ProjectTreeHandler
    {
        private ScreenDocument screen;

        public ScreenNodeHandler(ProjectEditor project, TreeNode node, ScreenDocument screen) : base(project, node)
        {
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
