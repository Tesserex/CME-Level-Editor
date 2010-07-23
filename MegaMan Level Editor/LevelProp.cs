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
        private MapDocument map;

        public static void CreateStage(ProjectEditor project)
        {
            var form = new StageProp();
            form.project = project;
            form.Text = "New Stage Properties";
            form.Show();
        }

        public static void EditStage(MapDocument map)
        {
            var form = new StageProp();
            form.project = map.Project;
            form.LoadMap(map);
            form.Show();
        }

        private StageProp()
        {
            InitializeComponent();
        }

        private void LoadMap(MapDocument map)
        {
            this.map = map;
            this.Text = map.Name + " Properties";
            nameField.Text = map.Name;
            tilesetField.Text = map.Tileset.FilePath;
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
                if (map == null) // new
                {
                    project.AddStage(nameField.Text, tilesetField.Text);
                    return true;
                }
                else
                {
                    map.Name = nameField.Text;
                    map.ChangeTileset(tilesetField.Text);
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

        private void buttonApply_Click(object sender, EventArgs e)
        {
            this.Save();
        }
    }
}
