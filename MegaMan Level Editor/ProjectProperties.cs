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
    public partial class ProjectProperties : Form
    {
        private ProjectEditor editor;
        private int lastWidth = 256, lastHeight = 224;

        public ProjectProperties()
        {
            InitializeComponent();

            this.textDir.Text = Environment.CurrentDirectory;
        }

        public ProjectProperties(ProjectEditor project)
        {
            InitializeComponent();

            this.editor = project;
            this.Text = project.Name + " Properties";
            this.textName.Text = project.Name;
            this.textAuthor.Text = project.Author;
            this.textWidth.Text = project.ScreenWidth.ToString();
            this.textHeight.Text = project.ScreenHeight.ToString();

            this.panelLocation.Visible = false;
            this.Height -= this.panelLocation.Height;
        }

        private void text_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void textHeight_Leave(object sender, EventArgs e)
        {
            if (!int.TryParse(textHeight.Text, out lastHeight))
            {
                MessageBox.Show("Positive integer required for screen height.", "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textHeight.Text = lastHeight.ToString();
            }
        }

        private void textWidth_Leave(object sender, EventArgs e)
        {
            if (!int.TryParse(textWidth.Text, out lastWidth))
            {
                MessageBox.Show("Positive integer required for screen width.", "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textWidth.Text = lastWidth.ToString();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            int width, height;
            if (!int.TryParse(this.textWidth.Text, out width) || !int.TryParse(this.textHeight.Text, out height))
            {
                MessageBox.Show("Positive integers are required for screen size.", "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool gameNew = (editor == null);
            if (gameNew)
            {
                string baseDir = System.IO.Path.Combine(this.textDir.Text, this.textName.Text);
                if (System.IO.Directory.Exists(baseDir))
                {
                    MessageBox.Show(
                        String.Format("Could not create the project because a directory named {0} already exists at the specified location.", this.textName.Text),
                        "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    System.IO.Directory.CreateDirectory(baseDir);
                }
                catch
                {
                    MessageBox.Show(
                        "Could not create the project because the system was unable to create a directory at the specified location.",
                        "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                editor = ProjectEditor.CreateNew(baseDir);
            }

            editor.Name = (this.textName.Text == "")? "Untitled" : this.textName.Text;
            editor.Author = this.textAuthor.Text;
            editor.ScreenWidth = lastWidth;
            editor.ScreenHeight = lastHeight;

            if (gameNew)
            {
                editor.Save();
                MainForm.Instance.projectForm.AddProject(editor);
            }
            Close();
        }

        private void textDir_TextChanged(object sender, EventArgs e)
        {
            this.buttonOK.Enabled = System.IO.Path.IsPathRooted(this.textDir.Text);
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = this.textDir.Text;
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textDir.Text = dialog.SelectedPath;
            }
        }
    }
}
