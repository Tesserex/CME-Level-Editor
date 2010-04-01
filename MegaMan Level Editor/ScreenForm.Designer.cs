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
            ((System.ComponentModel.ISupportInitialize)(this.screenImage)).BeginInit();
            this.SuspendLayout();
            // 
            // screenImage
            // 
            this.screenImage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.screenImage.BackColor = System.Drawing.SystemColors.Control;
            this.screenImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.screenImage.Location = new System.Drawing.Point(0, 0);
            this.screenImage.Name = "screenImage";
            this.screenImage.Size = new System.Drawing.Size(232, 199);
            this.screenImage.TabIndex = 0;
            this.screenImage.TabStop = false;
            this.screenImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.screenImage_MouseMove);
            this.screenImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.screenImage_MouseDown);
            this.screenImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.screenImage_MouseUp);
            // 
            // ScreenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(292, 267);
            this.Controls.Add(this.screenImage);
            this.Name = "ScreenForm";
            this.Text = "MapForm";
            this.Resize += new System.EventHandler(this.mapForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.screenImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox screenImage;
    }
}