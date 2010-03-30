namespace MegaMan_Level_Editor
{
    partial class BrowseForm
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
            this.screensBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.screensBox)).BeginInit();
            this.SuspendLayout();
            // 
            // screensBox
            // 
            this.screensBox.Location = new System.Drawing.Point(0, 0);
            this.screensBox.Name = "screensBox";
            this.screensBox.Size = new System.Drawing.Size(292, 174);
            this.screensBox.TabIndex = 0;
            this.screensBox.TabStop = false;
            this.screensBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.screensBox_MouseMove);
            this.screensBox.Click += new System.EventHandler(this.screensBox_Click);
            // 
            // BrowseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(294, 175);
            this.Controls.Add(this.screensBox);
            this.Name = "BrowseForm";
            this.Text = "Browse Screens";
            ((System.ComponentModel.ISupportInitialize)(this.screensBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox screensBox;
    }
}