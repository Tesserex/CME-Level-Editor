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
            // 
            // StageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 471);
            this.Controls.Add(this.sizingPanel);
            this.Name = "StageForm";
            this.Text = "StageForm";
            this.ResumeLayout(false);

        }
        #endregion

        public System.Windows.Forms.Panel sizingPanel;
        // private System.Windows.Forms.Panel screenContainer;
    }
}