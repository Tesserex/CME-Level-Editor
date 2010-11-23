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
            if (nameField.Text == "")
            {
                MessageBox.Show("Stage name cannot be blank.", "CME Level Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tilesetField.Text == "")
            {
                MessageBox.Show("Stage tileset path cannot be blank.", "CME Level Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = Save();
            if (success)
            {
                this.Close();
            }
        }

        private bool Save()
        {
                if (stage == null) // new
                {
                    try
                    {
                        project.AddStage(nameField.Text, tilesetField.Text);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("There was an error creating the stage.\nPerhaps your tileset path is incorrect?", "CME Level Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    return true;
                }
                else
                {
                    stage.Name = nameField.Text;
                    try
                    {
                        stage.ChangeTileset(tilesetField.Text);
                    }
                    catch
                    {
                        MessageBox.Show("The tileset specified could not be loaded. Sorry.", "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    return true;
                }
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
