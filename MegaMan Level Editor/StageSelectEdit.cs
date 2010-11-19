using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public partial class StageSelectEdit : Form
    {
        private ProjectEditor project;
        private Bitmap background;

        public StageSelectEdit(ProjectEditor project)
        {
            InitializeComponent();
            this.project = project;
            this.preview.Image = new Bitmap(project.ScreenWidth, project.ScreenHeight);

            bossX.Value = project.BossSpacingHorizontal;
            bossY.Value = project.BossSpacingVertical;

            if (project.StageSelectBackground != null)
            {
                textBackground.Text = project.StageSelectBackground.Absolute;
                try
                {
                    this.background = (Bitmap)Image.FromFile(project.StageSelectBackground.Absolute);
                    this.background.SetResolution(this.preview.Image.HorizontalResolution, this.preview.Image.VerticalResolution);
                }
                catch
                {
                    this.textBackground.Text = "";
                }
            }

            if (project.StageSelectMusic != null) textMusic.Text = project.StageSelectMusic.Absolute;
            if (project.StageSelectChangeSound != null) textSound.Text = project.StageSelectChangeSound.Absolute;

            ReDraw();
        }

        private void ReDraw()
        {
            using (Graphics g = Graphics.FromImage(preview.Image))
            {
                g.Clear(Color.Black);
                if (background != null) g.DrawImage(background, 0, 0);
            }
            this.preview.Refresh();
        }

        private void backgroundBrowse_Click(object sender, EventArgs e)
        {
            var browse = new OpenFileDialog();
            browse.Filter = "Images (png, gif, bmp, jpg)|*.png;*.gif;*.bmp;*.jpg";
            var result = browse.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    this.background = (Bitmap)Image.FromFile(browse.FileName);
                    this.background.SetResolution(this.preview.Image.HorizontalResolution, this.preview.Image.VerticalResolution);

                    project.StageSelectBackground = FilePath.FromAbsolute(browse.FileName, project.BaseDir);
                }
                catch
                {
                    MessageBox.Show("Sorry, that image could not be loaded.", "CME Project Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            this.textBackground.Text = browse.FileName;
            ReDraw();
        }

        private void frameBrowse_Click(object sender, EventArgs e)
        {
            var editor = new SpriteEditor(this.project);
            if (project.BossFrameSprite != null) editor.Sprite = project.BossFrameSprite;
            editor.SpriteChange += () =>
            {
                project.BossFrameSprite = editor.Sprite;
            };
            editor.Show();
        }

        private void musicBrowse_Click(object sender, EventArgs e)
        {
            var browse = new OpenFileDialog();
            browse.Filter = "Music (wav, mp3, ogg)|*.wav;*.mp3;*.ogg";
            var result = browse.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.textMusic.Text = browse.FileName;
                project.StageSelectMusic = FilePath.FromAbsolute(browse.FileName, project.BaseDir);
            }
        }

        private void soundBrowse_Click(object sender, EventArgs e)
        {
            var browse = new OpenFileDialog();
            browse.Filter = "Music (wav, mp3, ogg)|*.wav;*.mp3;*.ogg";
            var result = browse.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.textSound.Text = browse.FileName;
                project.StageSelectChangeSound = FilePath.FromAbsolute(browse.FileName, project.BaseDir);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bossX_ValueChanged(object sender, EventArgs e)
        {
            project.BossSpacingHorizontal = (int)bossX.Value;
        }

        private void bossY_ValueChanged(object sender, EventArgs e)
        {
            project.BossSpacingVertical = (int)bossY.Value;
        }
    }
}
