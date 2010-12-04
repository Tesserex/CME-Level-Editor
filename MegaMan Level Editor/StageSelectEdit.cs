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

            comboSlot.SelectedIndex = -1;
            comboStages.Items.AddRange(project.StageNames.ToArray());

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

            if (project.StageSelectIntro != null) textMusicIntro.Text = project.StageSelectIntro.Absolute;
            if (project.StageSelectLoop != null) textMusicLoop.Text = project.StageSelectLoop.Absolute;
            if (project.StageSelectChangeSound != null) textSound.Text = project.StageSelectChangeSound.Absolute;

            ReDraw();
        }

        private void ReDraw()
        {
            using (Graphics g = Graphics.FromImage(preview.Image))
            {
                g.Clear(Color.Black);
                if (background != null) g.DrawImage(background, 0, 0);

                if (this.project.BossFrameSprite != null) 
                {
                    int mid_x = this.project.ScreenWidth / 2 - this.project.BossFrameSprite.Width / 2;
                    int mid_y = this.project.ScreenHeight / 2 - this.project.BossFrameSprite.Height / 2 + project.BossOffset;

                    int space_x = this.project.BossSpacingHorizontal + this.project.BossFrameSprite.Width;
                    int space_y = this.project.BossSpacingVertical + this.project.BossFrameSprite.Height;

                    this.project.BossFrameSprite.Draw(g, mid_x - space_x, mid_y - space_y);
                    this.project.BossFrameSprite.Draw(g, mid_x, mid_y - space_y);
                    this.project.BossFrameSprite.Draw(g, mid_x + space_x, mid_y - space_y);
                    this.project.BossFrameSprite.Draw(g, mid_x - space_x, mid_y);
                    this.project.BossFrameSprite.Draw(g, mid_x, mid_y);
                    this.project.BossFrameSprite.Draw(g, mid_x + space_x, mid_y);
                    this.project.BossFrameSprite.Draw(g, mid_x - space_x, mid_y + space_y);
                    this.project.BossFrameSprite.Draw(g, mid_x, mid_y + space_y);
                    this.project.BossFrameSprite.Draw(g, mid_x + space_x, mid_y + space_y);
                }
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
            editor.FormClosed += (s, ev) =>
                {
                    ReDraw();
                };
            editor.Show();
        }

        private void musicIntroBrowse_Click(object sender, EventArgs e)
        {
            var browse = new OpenFileDialog();
            browse.Filter = "Music (wav, mp3, ogg)|*.wav;*.mp3;*.ogg";
            var result = browse.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.textMusicIntro.Text = browse.FileName;
                project.StageSelectIntro = FilePath.FromAbsolute(browse.FileName, project.BaseDir);
            }
        }

        private void musicLoopBrowse_Click(object sender, EventArgs e)
        {
            var browse = new OpenFileDialog();
            browse.Filter = "Music (wav, mp3, ogg)|*.wav;*.mp3;*.ogg";
            var result = browse.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.textMusicLoop.Text = browse.FileName;
                project.StageSelectLoop = FilePath.FromAbsolute(browse.FileName, project.BaseDir);
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
            this.ReDraw();
        }

        private void bossY_ValueChanged(object sender, EventArgs e)
        {
            project.BossSpacingVertical = (int)bossY.Value;
            this.ReDraw();
        }

        private void comboSlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            BossInfo info = this.project.BossAtSlot(comboSlot.SelectedIndex);

            if (info.PortraitPath != null && info.PortraitPath.Relative != "") textPortrait.Text = info.PortraitPath.Absolute;
            else textPortrait.Text = "";

            textBossName.Text = info.Name;

            if (info.Stage == null) comboStages.SelectedIndex = -1;
            else comboStages.SelectedIndex = comboStages.Items.IndexOf(info.Stage);
        }

        private void comboStages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboStages.SelectedItem != null)
            {
                this.project.BossAtSlot(comboSlot.SelectedIndex).Stage = comboStages.SelectedItem.ToString();
            }
        }

        private void portraitBrowse_Click(object sender, EventArgs e)
        {
            var browse = new OpenFileDialog();
            browse.Filter = "Images (png, gif, bmp, jpg)|*.png;*.gif;*.bmp;*.jpg";
            var result = browse.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.project.BossAtSlot(comboSlot.SelectedIndex).PortraitPath = FilePath.FromAbsolute(browse.FileName, this.project.BaseDir);
                this.textPortrait.Text = browse.FileName;
            }
        }

        private void textBossName_TextChanged(object sender, EventArgs e)
        {
            if (comboStages.SelectedItem != null)
            {
                this.project.BossAtSlot(comboSlot.SelectedIndex).Name = textBossName.Text;
            }
        }

        private void bossOffset_ValueChanged(object sender, EventArgs e)
        {
            project.BossOffset = (int)bossOffset.Value;
            this.ReDraw();
        }
    }
}
