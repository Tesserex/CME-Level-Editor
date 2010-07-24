using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MegaMan_Level_Editor
{
    public partial class ProjectForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public ProjectForm()
        {
            InitializeComponent();
            projectView.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(projectView_NodeMouseDoubleClick);
            var imagelist = new ImageList();
            imagelist.ColorDepth = ColorDepth.Depth32Bit;
            imagelist.Images.Add(Properties.Resources.Folder_16x16);
            imagelist.Images.Add(Properties.Resources.FolderOpen_16x16_72);
            imagelist.Images.Add(Properties.Resources.stage);
            imagelist.Images.Add(Properties.Resources.screen);
            projectView.ImageList = imagelist;
            projectView.BeforeCollapse += new TreeViewCancelEventHandler(projectView_BeforeCollapse);
            projectView.BeforeExpand += new TreeViewCancelEventHandler(projectView_BeforeExpand);
        }

        void projectView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ImageIndex == 0) e.Node.ImageIndex = 1;
        }

        void projectView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ImageIndex == 1) e.Node.ImageIndex = 0;
        }

        void projectView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var tag = (e.Node.Tag as ProjectTreeHandler);
            if (tag != null) tag.DoubleClick();
        }

        public void AddProject(ProjectEditor project)
        {
            var projectNode = this.projectView.Nodes.Add(project.Name);
            projectNode.NodeFont = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
            projectNode.Tag = new ProjectNodeHandler(project, projectNode);

            var stagesNode = projectNode.Nodes.Add("Stages");
            stagesNode.ImageIndex = 0;
            foreach (var stage in project.StageNames)
            {
                var stagenode = stagesNode.Nodes.Add(stage);
                stagenode.ImageIndex = stagenode.SelectedImageIndex = 2;
                stagenode.Tag = new StageNodeHandler(project, stagenode, stage);
            }

            project.StageAdded += (stage) =>
            {
                var stagenode = stagesNode.Nodes.Add(stage.Name);
                stagenode.ImageIndex = stagenode.SelectedImageIndex = 2;
                stagenode.Tag = new StageNodeHandler(project, stagenode, stage);
            };
        }

        private void buttonNewStage_Click(object sender, EventArgs e)
        {
            if (projectView.SelectedNode != null)
            {
                var node = projectView.SelectedNode;
                var tag = node.Tag as ProjectTreeHandler;
                while (tag == null && node.Parent != null)
                {
                    node = node.Parent;
                    tag = node.Tag as ProjectTreeHandler;
                }

                if (tag != null)
                {
                    StageProp.CreateStage(tag.Project);
                }
            }
        }

        private void buttonNewScreen_Click(object sender, EventArgs e)
        {
            if (projectView.SelectedNode != null)
            {
                var node = projectView.SelectedNode;
                var tag = node.Tag as StageNodeHandler;
                while (tag == null && node.Parent != null)
                {
                    node = node.Parent;
                    tag = node.Tag as StageNodeHandler;
                }

                if (tag != null)
                {
                    ScreenProp.CreateScreen(tag.Stage);
                }
            }
        }
    }
}
