using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MegaMan_Level_Editor {
    public class HistoryForm : Form {
        private ListBox historyView;
    
        public HistoryForm() {
            InitializeComponent();
        }

        public void UpdateHistory(History history) {
            historyView.Items.Clear();

            for (int i = 0; i < history.stack.Count; i++) {
                if (history.currentAction == i)
                    historyView.Items.Add(" ->" + history.stack[i].ToString());                    
                else
                    historyView.Items.Add(" * " + history.stack[i].ToString());
            }
        }

        void InitializeComponent() {
            this.historyView = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // historyView
            // 
            this.historyView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyView.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.historyView.FormattingEnabled = true;
            this.historyView.ItemHeight = 18;
            this.historyView.Location = new System.Drawing.Point(0, 0);
            this.historyView.Name = "historyView";
            this.historyView.Size = new System.Drawing.Size(340, 238);
            this.historyView.TabIndex = 0;
            this.historyView.SelectedIndexChanged += new System.EventHandler(this.historyView_SelectedIndexChanged);
            // 
            // HistoryForm
            // 
            this.ClientSize = new System.Drawing.Size(340, 241);
            this.Controls.Add(this.historyView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "HistoryForm";
            this.Text = "History";
            this.Load += new System.EventHandler(this.HistoryForm_Load);
            this.ResumeLayout(false);

        }

        private void historyView_SelectedIndexChanged(object sender, EventArgs e) {
            
        }

        private void HistoryForm_Load(object sender, EventArgs e) {
            
        }
    }
}
