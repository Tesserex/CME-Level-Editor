namespace MegaMan_Level_Editor
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteAsNewBrushToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.newBrushToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showBackgroundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showBlockingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showEnemiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.levelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screensMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addScreenToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.browseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageJoinsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.addEnemyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageEnemiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearEnemiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mergeScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.brushesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.levelToolStripMenuItem,
            this.screenToolStripMenuItem,
            this.windowsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(572, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.recentMenuItem,
            this.toolStripSeparator4,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator5,
            this.closeToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = global::MegaMan_Level_Editor.Properties.Resources.NewDocumentHS;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::MegaMan_Level_Editor.Properties.Resources.openHS;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // recentMenuItem
            // 
            this.recentMenuItem.Name = "recentMenuItem";
            this.recentMenuItem.Size = new System.Drawing.Size(183, 22);
            this.recentMenuItem.Text = "Recent";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(180, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::MegaMan_Level_Editor.Properties.Resources.saveHS;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(180, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator6,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.pasteAsNewBrushToolStripMenuItem,
            this.toolStripSeparator7,
            this.newBrushToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Image = global::MegaMan_Level_Editor.Properties.Resources.Undo;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Image = global::MegaMan_Level_Editor.Properties.Resources.Redo;
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(243, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::MegaMan_Level_Editor.Properties.Resources.CopyHS;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = global::MegaMan_Level_Editor.Properties.Resources.PasteHS;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            // 
            // pasteAsNewBrushToolStripMenuItem
            // 
            this.pasteAsNewBrushToolStripMenuItem.Name = "pasteAsNewBrushToolStripMenuItem";
            this.pasteAsNewBrushToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.V)));
            this.pasteAsNewBrushToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.pasteAsNewBrushToolStripMenuItem.Text = "Paste as New Brush";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(243, 6);
            // 
            // newBrushToolStripMenuItem
            // 
            this.newBrushToolStripMenuItem.Name = "newBrushToolStripMenuItem";
            this.newBrushToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.newBrushToolStripMenuItem.Text = "New Brush";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showBackgroundsToolStripMenuItem,
            this.showBlockingToolStripMenuItem,
            this.showEnemiesToolStripMenuItem,
            this.showGridToolStripMenuItem,
            this.animateToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // showBackgroundsToolStripMenuItem
            // 
            this.showBackgroundsToolStripMenuItem.Checked = true;
            this.showBackgroundsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showBackgroundsToolStripMenuItem.Name = "showBackgroundsToolStripMenuItem";
            this.showBackgroundsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.showBackgroundsToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.showBackgroundsToolStripMenuItem.Text = "Tiles";
            this.showBackgroundsToolStripMenuItem.Click += new System.EventHandler(this.showBackgroundsToolStripMenuItem_Click);
            // 
            // showBlockingToolStripMenuItem
            // 
            this.showBlockingToolStripMenuItem.Name = "showBlockingToolStripMenuItem";
            this.showBlockingToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.showBlockingToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.showBlockingToolStripMenuItem.Text = "Blocking";
            this.showBlockingToolStripMenuItem.Click += new System.EventHandler(this.showBlockingToolStripMenuItem_Click);
            // 
            // showEnemiesToolStripMenuItem
            // 
            this.showEnemiesToolStripMenuItem.Checked = true;
            this.showEnemiesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showEnemiesToolStripMenuItem.Name = "showEnemiesToolStripMenuItem";
            this.showEnemiesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.showEnemiesToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.showEnemiesToolStripMenuItem.Text = "Entities";
            // 
            // showGridToolStripMenuItem
            // 
            this.showGridToolStripMenuItem.Name = "showGridToolStripMenuItem";
            this.showGridToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.showGridToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.showGridToolStripMenuItem.Text = "Grid";
            this.showGridToolStripMenuItem.Click += new System.EventHandler(this.gridToolStripMenuItem_Click);
            // 
            // animateToolStripMenuItem
            // 
            this.animateToolStripMenuItem.Name = "animateToolStripMenuItem";
            this.animateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.animateToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.animateToolStripMenuItem.Text = "Animate";
            this.animateToolStripMenuItem.Click += new System.EventHandler(this.animateToolStripMenuItem_Click);
            // 
            // levelToolStripMenuItem
            // 
            this.levelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.screensMenuItem,
            this.manageJoinsMenu,
            this.propertiesToolStripMenuItem});
            this.levelToolStripMenuItem.Name = "levelToolStripMenuItem";
            this.levelToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.levelToolStripMenuItem.Text = "&Level";
            // 
            // screensMenuItem
            // 
            this.screensMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addScreenToolStripMenuItem1,
            this.browseToolStripMenuItem});
            this.screensMenuItem.Name = "screensMenuItem";
            this.screensMenuItem.Size = new System.Drawing.Size(182, 22);
            this.screensMenuItem.Text = "Screens";
            // 
            // addScreenToolStripMenuItem1
            // 
            this.addScreenToolStripMenuItem1.Name = "addScreenToolStripMenuItem1";
            this.addScreenToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.addScreenToolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
            this.addScreenToolStripMenuItem1.Text = "Add Screen";
            this.addScreenToolStripMenuItem1.Click += new System.EventHandler(this.addScreenToolStripMenuItem1_Click);
            // 
            // browseToolStripMenuItem
            // 
            this.browseToolStripMenuItem.Name = "browseToolStripMenuItem";
            this.browseToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.browseToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.browseToolStripMenuItem.Text = "Browse";
            this.browseToolStripMenuItem.Click += new System.EventHandler(this.browseToolStripMenuItem_Click);
            // 
            // manageJoinsMenu
            // 
            this.manageJoinsMenu.Name = "manageJoinsMenu";
            this.manageJoinsMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
            this.manageJoinsMenu.Size = new System.Drawing.Size(182, 22);
            this.manageJoinsMenu.Text = "Manage Joins";
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // screenToolStripMenuItem
            // 
            this.screenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripSeparator3,
            this.mergeScreenToolStripMenuItem,
            this.splitScreenToolStripMenuItem,
            this.toolStripSeparator2,
            this.clearScreenToolStripMenuItem});
            this.screenToolStripMenuItem.Name = "screenToolStripMenuItem";
            this.screenToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.screenToolStripMenuItem.Text = "&Screen";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addEnemyToolStripMenuItem,
            this.manageEnemiesToolStripMenuItem,
            this.clearEnemiesToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(219, 22);
            this.toolStripMenuItem2.Text = "Entities";
            // 
            // addEnemyToolStripMenuItem
            // 
            this.addEnemyToolStripMenuItem.Image = global::MegaMan_Level_Editor.Properties.Resources.AddEntity;
            this.addEnemyToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.addEnemyToolStripMenuItem.Name = "addEnemyToolStripMenuItem";
            this.addEnemyToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.addEnemyToolStripMenuItem.Text = "Add Entity";
            // 
            // manageEnemiesToolStripMenuItem
            // 
            this.manageEnemiesToolStripMenuItem.Name = "manageEnemiesToolStripMenuItem";
            this.manageEnemiesToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.manageEnemiesToolStripMenuItem.Text = "Manage Entities";
            // 
            // clearEnemiesToolStripMenuItem
            // 
            this.clearEnemiesToolStripMenuItem.Name = "clearEnemiesToolStripMenuItem";
            this.clearEnemiesToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.clearEnemiesToolStripMenuItem.Text = "Clear Entities";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(216, 6);
            // 
            // mergeScreenToolStripMenuItem
            // 
            this.mergeScreenToolStripMenuItem.Name = "mergeScreenToolStripMenuItem";
            this.mergeScreenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.M)));
            this.mergeScreenToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.mergeScreenToolStripMenuItem.Text = "Merge Screen";
            // 
            // splitScreenToolStripMenuItem
            // 
            this.splitScreenToolStripMenuItem.Name = "splitScreenToolStripMenuItem";
            this.splitScreenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.X)));
            this.splitScreenToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.splitScreenToolStripMenuItem.Text = "Split Screen";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(216, 6);
            // 
            // clearScreenToolStripMenuItem
            // 
            this.clearScreenToolStripMenuItem.Name = "clearScreenToolStripMenuItem";
            this.clearScreenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Delete)));
            this.clearScreenToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.clearScreenToolStripMenuItem.Text = "Clear Screen";
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.brushesToolStripMenuItem});
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.windowsToolStripMenuItem.Text = "&Windows";
            // 
            // brushesToolStripMenuItem
            // 
            this.brushesToolStripMenuItem.Name = "brushesToolStripMenuItem";
            this.brushesToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.brushesToolStripMenuItem.Text = "Brushes";
            this.brushesToolStripMenuItem.Click += new System.EventHandler(this.brushesToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 436);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "C# MegaMan Engine Level Editor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem levelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screensMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addScreenToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem browseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem addEnemyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageEnemiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearEnemiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showBlockingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showBackgroundsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showEnemiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem clearScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergeScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitScreenToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderDialog;
        private System.Windows.Forms.ToolStripMenuItem showGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteAsNewBrushToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem newBrushToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem brushesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem animateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageJoinsMenu;
    }
}

