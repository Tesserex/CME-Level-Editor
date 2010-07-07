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
    public partial class ProjectForm : Form
    {
        public ProjectForm()
        {
            InitializeComponent();
            projectView.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(projectView_NodeMouseDoubleClick);
            projectView.NodeMouseClick += new TreeNodeMouseClickEventHandler(projectView_NodeMouseClick);
        }

        void projectView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                HandleRightClick(e.Node);
        }

        void projectView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            HandleDoubleClick(e.Node);
        }

        public String StagePath()
        {
            return Path.Combine(MainForm.Instance.rootPath, "stages");
        }

        public String LocalStagePathFor(String stageName)
        {
            return Path.Combine("stages", stageName);
        }

        public void AddProject(ProjectEditor project)
        {
            var projectNode = this.projectView.Nodes.Add(project.Name, project.Name);
            projectNode.Tag = project;
            foreach (var stage in project.StageNames)
            {
                projectNode.Nodes.Add(stage, stage);
            }
        }

        public void HandleDoubleClick(TreeNode node)
        {
            if (node.Parent.Name == "stages")
            {
                OpenStage(node);
            }
            else if (node.Parent.Parent.Name == "stages")
            {
                OpenScreenProperties(node);
            }
        }

        private void OpenStage(TreeNode node)
        {
            MapDocument stage = MainForm.Instance.OpenStage(node.Name, LocalStagePathFor(node.Name));
            LoadScreenSubtree(node, stage.Screens);

            stage.ScreenAdded += (screen) =>
                {
                    var stageNode = projectView.Nodes.Find(screen.Map.Name, true).First();
                    node.Nodes.Add(screen.Name, screen.Name);
                };

            stage.ReFocus();
        }

        private void LoadScreenSubtree(TreeNode node, IEnumerable<ScreenDocument> screens)
        {
            node.Nodes.Clear();
            foreach (var screen in screens)
            {
                node.Nodes.Add(screen.Name, screen.Name);
            }
        }

        private void OpenScreenProperties(TreeNode node)
        {
            var stageName = node.Parent.Name;
            var screenName = node.Name;
            var screen = MainForm.GetScreen(stageName, screenName);
            var screenProps = new ScreenProp(screen);
            screenProps.Show();
        }

        public void HandleRightClick(TreeNode node)
        {
            
        }
    }
}
