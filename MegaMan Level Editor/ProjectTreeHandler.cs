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
        public StageDocument Stage { get; private set; }
        private string stageName;

        public StageNodeHandler(ProjectEditor project, TreeNode node, string stageName) : base(project, node)
        {
            this.Stage = null;
            this.stageName = stageName;
        }

        public StageNodeHandler(ProjectEditor project, TreeNode node, StageDocument stage) : base(project, node)
        {
            this.Stage = stage;
            this.stageName = stage.Name;
        }

        public override void DoubleClick()
        {
            if (this.Stage == null)
            {
                this.Stage = this.Project.StageByName(stageName);
                if (this.Stage == null) return;

                parentNode.Nodes.Clear();
                foreach (var screen in Stage.Screens)
                {
                    var node = new TreeNode(screen.Name);
                    node.ImageIndex = node.SelectedImageIndex = 3;
                    node.Tag = new ScreenNodeHandler(this.Project, node, screen);
                    parentNode.Nodes.Add(node);
                }

                Stage.ScreenAdded += (screen) =>
                {
                    var node = new TreeNode(screen.Name);
                    node.ImageIndex = node.SelectedImageIndex = 3;
                    node.Tag = new ScreenNodeHandler(this.Project, node, screen);
                    parentNode.Nodes.Add(node);
                };
            }

            Stage.ReFocus();
        }

        public override void Delete() { }

        public override void Properties()
        {
            if (this.Stage != null) StageProp.EditStage(this.Stage);
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
            ScreenProp.EditScreen(this.screen);
        }
    }
}
