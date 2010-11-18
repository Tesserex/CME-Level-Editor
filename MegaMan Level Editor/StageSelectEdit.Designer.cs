namespace MegaMan_Level_Editor
{
    partial class StageSelectEdit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBackground = new System.Windows.Forms.TextBox();
            this.backgroundBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textMusic = new System.Windows.Forms.TextBox();
            this.textSound = new System.Windows.Forms.TextBox();
            this.musicBrowse = new System.Windows.Forms.Button();
            this.soundBrowse = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bossY = new System.Windows.Forms.NumericUpDown();
            this.bossX = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.preview = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.frameBrowse = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bossY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bossX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Background:";
            // 
            // textBackground
            // 
            this.textBackground.Location = new System.Drawing.Point(87, 10);
            this.textBackground.Name = "textBackground";
            this.textBackground.Size = new System.Drawing.Size(147, 20);
            this.textBackground.TabIndex = 2;
            // 
            // backgroundBrowse
            // 
            this.backgroundBrowse.Location = new System.Drawing.Point(239, 9);
            this.backgroundBrowse.Name = "backgroundBrowse";
            this.backgroundBrowse.Size = new System.Drawing.Size(66, 21);
            this.backgroundBrowse.TabIndex = 3;
            this.backgroundBrowse.Text = "Browse...";
            this.backgroundBrowse.UseVisualStyleBackColor = true;
            this.backgroundBrowse.Click += new System.EventHandler(this.backgroundBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Music:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Change Sound:";
            // 
            // textMusic
            // 
            this.textMusic.Location = new System.Drawing.Point(87, 60);
            this.textMusic.Name = "textMusic";
            this.textMusic.Size = new System.Drawing.Size(147, 20);
            this.textMusic.TabIndex = 6;
            // 
            // textSound
            // 
            this.textSound.Location = new System.Drawing.Point(87, 85);
            this.textSound.Name = "textSound";
            this.textSound.Size = new System.Drawing.Size(147, 20);
            this.textSound.TabIndex = 7;
            // 
            // musicBrowse
            // 
            this.musicBrowse.Location = new System.Drawing.Point(239, 60);
            this.musicBrowse.Name = "musicBrowse";
            this.musicBrowse.Size = new System.Drawing.Size(66, 21);
            this.musicBrowse.TabIndex = 8;
            this.musicBrowse.Text = "Browse...";
            this.musicBrowse.UseVisualStyleBackColor = true;
            this.musicBrowse.Click += new System.EventHandler(this.musicBrowse_Click);
            // 
            // soundBrowse
            // 
            this.soundBrowse.Location = new System.Drawing.Point(240, 85);
            this.soundBrowse.Name = "soundBrowse";
            this.soundBrowse.Size = new System.Drawing.Size(66, 21);
            this.soundBrowse.TabIndex = 9;
            this.soundBrowse.Text = "Browse...";
            this.soundBrowse.UseVisualStyleBackColor = true;
            this.soundBrowse.Click += new System.EventHandler(this.soundBrowse_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Horizontal:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bossY);
            this.groupBox1.Controls.Add(this.bossX);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(87, 112);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(170, 76);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Boss Spacing";
            // 
            // bossY
            // 
            this.bossY.Location = new System.Drawing.Point(98, 46);
            this.bossY.Name = "bossY";
            this.bossY.Size = new System.Drawing.Size(48, 20);
            this.bossY.TabIndex = 13;
            this.bossY.ValueChanged += new System.EventHandler(this.bossY_ValueChanged);
            // 
            // bossX
            // 
            this.bossX.Location = new System.Drawing.Point(98, 20);
            this.bossX.Name = "bossX";
            this.bossX.Size = new System.Drawing.Size(48, 20);
            this.bossX.TabIndex = 12;
            this.bossX.ValueChanged += new System.EventHandler(this.bossX_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(47, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Vertical:";
            // 
            // preview
            // 
            this.preview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.preview.Location = new System.Drawing.Point(314, 10);
            this.preview.Name = "preview";
            this.preview.Size = new System.Drawing.Size(256, 224);
            this.preview.TabIndex = 0;
            this.preview.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Boss Frame:";
            // 
            // frameBrowse
            // 
            this.frameBrowse.Location = new System.Drawing.Point(87, 34);
            this.frameBrowse.Name = "frameBrowse";
            this.frameBrowse.Size = new System.Drawing.Size(88, 21);
            this.frameBrowse.TabIndex = 14;
            this.frameBrowse.Text = "Edit Sprite...";
            this.frameBrowse.UseVisualStyleBackColor = true;
            this.frameBrowse.Click += new System.EventHandler(this.frameBrowse_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(137, 208);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 15;
            this.buttonSave.Text = "OK";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // StageSelectEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 243);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.frameBrowse);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.soundBrowse);
            this.Controls.Add(this.musicBrowse);
            this.Controls.Add(this.textSound);
            this.Controls.Add(this.textMusic);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.backgroundBrowse);
            this.Controls.Add(this.textBackground);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.preview);
            this.Name = "StageSelectEdit";
            this.Text = "StageSelectEdit";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bossY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bossX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox preview;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBackground;
        private System.Windows.Forms.Button backgroundBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textMusic;
        private System.Windows.Forms.TextBox textSound;
        private System.Windows.Forms.Button musicBrowse;
        private System.Windows.Forms.Button soundBrowse;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown bossY;
        private System.Windows.Forms.NumericUpDown bossX;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button frameBrowse;
        private System.Windows.Forms.Button buttonSave;
    }
}