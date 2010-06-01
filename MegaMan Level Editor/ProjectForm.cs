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

        public void OpenProject(String rootPath)
        {
            var subdirectories = Directory.GetDirectories(StagePath()).ToList<String>();
            subdirectories = subdirectories.Select(dir => { return dir.Replace(StagePath(), "").Replace("\\", ""); }).ToList<String>();
            this.AddStages(subdirectories);
            this.Show();
        }

        public String StagePath()
        {
            return Path.Combine(MainForm.Instance.rootPath, "stages");
        }

        public String LocalStagePathFor(String stageName)
        {
            return Path.Combine("stages", stageName);
        }

        public void AddStages(List<string> stages)
        {
            var stageNode = this.projectView.Nodes.Add("stages", "stages");
            foreach (var stage in stages)
            {
                stageNode.Nodes.Add(stage, stage);
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
            else
            {
                MessageBox.Show("Parent name is " + node.Parent.Name + ". How do I handle this?");
            }
        }

        public void OpenStage(TreeNode node)
        {
            LoadScreenSubtree(node, MainForm.Instance.OpenStage(node.Name, LocalStagePathFor(node.Name)));
        }

        public void LoadScreenSubtree(TreeNode node, List<MegaMan.Screen> screens)
        {
            node.Nodes.Clear();
            foreach (var screen in screens)
            {
                node.Nodes.Add(screen.Name, screen.Name);
            }
        }

        public void OpenScreenProperties(TreeNode node)
        {
            var stageName = node.Parent.Name;
            var screenName = node.Name;
            var screen = MainForm.GetScreen(stageName, screenName);
            var screenProps = new ScreenProp(screen);
            screenProps.Show();
        }

        public void HandleRightClick(TreeNode node)
        {
            MessageBox.Show("You right clicked " + node.Name);
        }

        private void StageForm_Load(object sender, EventArgs e)
        {

        }

        private void projectView_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
