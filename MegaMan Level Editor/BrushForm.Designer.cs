namespace MegaMan_Level_Editor
{
    partial class BrushForm
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
            this.buttonNewBrush = new System.Windows.Forms.Button();
            this.brushPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // buttonNewBrush
            // 
            this.buttonNewBrush.Location = new System.Drawing.Point(12, 12);
            this.buttonNewBrush.Name = "buttonNewBrush";
            this.buttonNewBrush.Size = new System.Drawing.Size(76, 23);
            this.buttonNewBrush.TabIndex = 1;
            this.buttonNewBrush.Text = "New Brush";
            this.buttonNewBrush.UseVisualStyleBackColor = true;
            this.buttonNewBrush.Click += new System.EventHandler(this.buttonNewBrush_Click);
            // 
            // brushPanel
            // 
            this.brushPanel.Location = new System.Drawing.Point(10, 47);
            this.brushPanel.Name = "brushPanel";
            this.brushPanel.Size = new System.Drawing.Size(165, 226);
            this.brushPanel.TabIndex = 2;
            // 
            // BrushForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(185, 287);
            this.Controls.Add(this.brushPanel);
            this.Controls.Add(this.buttonNewBrush);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "BrushForm";
            this.ShowInTaskbar = false;
            this.Text = "Brushes";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonNewBrush;
        private System.Windows.Forms.FlowLayoutPanel brushPanel;
    }
}