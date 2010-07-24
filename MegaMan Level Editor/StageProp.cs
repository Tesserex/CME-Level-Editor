using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MegaMan_Level_Editor
{
    public partial class StageProp : Form
    {
        private ProjectEditor project;
        private StageDocument stage;

        public static void CreateStage(ProjectEditor project)
        {
            var form = new StageProp();
            form.project = project;
            form.Text = "New Stage Properties";
            form.Show();
        }

        public static void EditStage(StageDocument stage)
        {
            var form = new StageProp();
            form.project = stage.Project;
            form.LoadStage(stage);
            form.Show();
        }

        private StageProp()
        {
            InitializeComponent();
        }

        private void LoadStage(StageDocument stage)
        {
            this.stage = stage;
            this.Text = stage.Name + " Properties";
            nameField.Text = stage.Name;
            tilesetField.Text = stage.Tileset.FilePath;
        }

        private void tilesetChange_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                tilesetField.Text = dialog.FileName;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            bool success = Save();
            if (success)
            {
                this.Close();
            }
        }

        private bool Save()
        {
            //try
            //{
                if (stage == null) // new
                {
                    project.AddStage(nameField.Text, tilesetField.Text);
                    return true;
                }
                else
                {
                    stage.Name = nameField.Text;
                    stage.ChangeTileset(tilesetField.Text);
                    return true;
                }
            //}
            //catch
            //{
            //    MessageBox.Show("The tileset specified could not be loaded. Sorry.", "C# Mega Man Level Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            return false;
        }

        private void Cancel()
        {
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }
    }
}
