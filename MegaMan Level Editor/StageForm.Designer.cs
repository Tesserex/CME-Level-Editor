using System.Windows.Forms;
using System.Drawing;

namespace MegaMan_Level_Editor {
    partial class StageForm {
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
            this.sizingPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // sizingPanel
            // 
            this.sizingPanel.AutoScroll = true;
            this.sizingPanel.BackColor = System.Drawing.SystemColors.Control;
            this.sizingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sizingPanel.Location = new System.Drawing.Point(0, 0);
            this.sizingPanel.Name = "sizingPanel";
            this.sizingPanel.Size = new System.Drawing.Size(892, 471);
            this.sizingPanel.TabIndex = 1;
            this.sizingPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.sizingPanel_Paint);
            // 
            // StageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 471);
            this.Controls.Add(this.sizingPanel);
            this.Name = "StageForm";
            this.Text = "StageForm";
            this.Load += new System.EventHandler(this.StageForm_Load);
            this.GotFocus += new System.EventHandler(this.StageForm_GotFocus);
            this.ResumeLayout(false);

        }

        void sizingPanel_Paint(object sender, PaintEventArgs e) {
            // Get the graphics object 
            var gfx = e.Graphics;
            var pen = new Pen(Color.Black);

            var tilesWide = sizingPanel.Width  / stage.Tileset.TileSize;
            var tilesHigh = sizingPanel.Height / stage.Tileset.TileSize;

            var tileSize = stage.Tileset.TileSize;

            for (int i = 0; i < tilesWide; i++) {
                for (int j = 0; j < tilesHigh; j++) {
                    gfx.DrawRectangle(pen,  i * tileSize, j * tileSize, (i+1)*tileSize, (j+1)*tileSize);
//                    gfx.DrawRectangle(pen,  Util.rectAt(i, j, tilesize()));
                }
            }

        }

        #endregion

        public System.Windows.Forms.Panel sizingPanel;
        // private System.Windows.Forms.Panel screenContainer;
    }
}