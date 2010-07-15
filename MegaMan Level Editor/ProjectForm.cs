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
        }

        void projectView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            (e.Node.Tag as ProjectTreeHandler).DoubleClick();
        }

        public void AddProject(ProjectEditor project)
        {
            var projectNode = this.projectView.Nodes.Add(project.Name, project.Name);
            projectNode.Tag = new ProjectNodeHandler(projectNode, project);
            foreach (var stage in project.StageNames)
            {
                var stagenode = projectNode.Nodes.Add(stage, stage);
                stagenode.Tag = new StageNodeHandler(stagenode, project, stage);
            }
        }
    }
}
