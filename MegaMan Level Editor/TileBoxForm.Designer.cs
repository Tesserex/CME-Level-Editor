namespace MegaMan_Level_Editor
{
    partial class TileBoxForm
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
            this.tilesetImage = new System.Windows.Forms.PictureBox();
            this.tilePanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.tilesetImage)).BeginInit();
            this.tilePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tilesetImage
            // 
            this.tilesetImage.Location = new System.Drawing.Point(9, 5);
            this.tilesetImage.Margin = new System.Windows.Forms.Padding(0);
            this.tilesetImage.Name = "tilesetImage";
            this.tilesetImage.Size = new System.Drawing.Size(281, 29);
            this.tilesetImage.TabIndex = 0;
            this.tilesetImage.TabStop = false;
            this.tilesetImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tilesetImage_MouseMove);
            // 
            // tilePanel
            // 
            this.tilePanel.AutoScroll = true;
            this.tilePanel.Controls.Add(this.tilesetImage);
            this.tilePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilePanel.Location = new System.Drawing.Point(0, 0);
            this.tilePanel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tilePanel.Name = "tilePanel";
            this.tilePanel.Size = new System.Drawing.Size(505, 43);
            this.tilePanel.TabIndex = 2;
            // 
            // TileBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(505, 43);
            this.Controls.Add(this.tilePanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.Name = "TileBoxForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Tileset";
            this.TopMost = true;
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tilesetImage_MouseClick);
            ((System.ComponentModel.ISupportInitialize)(this.tilesetImage)).EndInit();
            this.tilePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox tilesetImage;
        private System.Windows.Forms.Panel tilePanel;
    }
}