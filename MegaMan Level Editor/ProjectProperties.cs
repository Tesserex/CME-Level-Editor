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
            if (!int.TryParse(this.textWidth.Text, out width) || !int.TryParse(this.textHeight.Text, out height)) return;

            bool gameNew = (editor == null);
            if (gameNew)
            {
                editor = ProjectEditor.CreateNew();
            }

            editor.Name = (this.textName.Text == "")? "Untitled" : this.textName.Text;
            editor.Author = this.textAuthor.Text;
            editor.ScreenWidth = lastWidth;
            editor.ScreenHeight = lastHeight;

            if (gameNew) MainForm.Instance.projectForm.AddProject(editor);
            Close();
        }
    }
}
