namespace MegaMan_Level_Editor
{
    partial class ScreenForm
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
            this.screenImage = new System.Windows.Forms.PictureBox();
            this.sizingPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.screenImage)).BeginInit();
            this.sizingPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // screenImage
            // 
            this.screenImage.BackColor = System.Drawing.SystemColors.Control;
            this.screenImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.screenImage.Location = new System.Drawing.Point(25, 18);
            this.screenImage.Name = "screenImage";
            this.screenImage.Size = new System.Drawing.Size(232, 199);
            this.screenImage.TabIndex = 0;
            this.screenImage.TabStop = false;
            this.screenImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.screenImage_MouseMove);
            this.screenImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.screenImage_MouseDown);
            this.screenImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.screenImage_MouseUp);
            // 
            // sizingPanel
            // 
            this.sizingPanel.AutoScroll = true;
            this.sizingPanel.BackColor = System.Drawing.SystemColors.Control;
            this.sizingPanel.Controls.Add(this.screenImage);
            this.sizingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sizingPanel.Location = new System.Drawing.Point(0, 0);
            this.sizingPanel.Name = "sizingPanel";
            this.sizingPanel.Size = new System.Drawing.Size(275, 234);
            this.sizingPanel.TabIndex = 1;
            // 
            // ScreenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 234);
            this.Controls.Add(this.sizingPanel);
            this.Name = "ScreenForm";
            this.Text = "MapForm";
            this.Resize += new System.EventHandler(this.mapForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.screenImage)).EndInit();
            this.sizingPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox screenImage;
        private System.Windows.Forms.Panel sizingPanel;
    }
}