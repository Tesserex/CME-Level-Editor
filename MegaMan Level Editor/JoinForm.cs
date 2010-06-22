using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public partial class JoinForm : Form
    {
        private JoinType type;

        public JoinType Type
        {
            get { return type; }
            set
            {
                type = value;
                AdjustLabels();
                joinType.SelectedIndex = (type == JoinType.Vertical) ? 0 : 1;
            }
        }

        public int JoinWidth
        {
            get { return (int)width.Value; }
            set { width.Value = value; }
        }

        public string ScreenOne
        {
            get { return screenOne.SelectedItem.ToString(); }
            set { screenOne.SelectedItem = value; }
        }

        public string ScreenTwo
        {
            get { return screenTwo.SelectedItem.ToString(); }
            set { screenTwo.SelectedItem = value; }
        }

        public int OffsetOne
        {
            get { return (int)offsetOne.Value; }
            set { offsetOne.Value = value; }
        }

        public int OffsetTwo
        {
            get { return (int)offsetTwo.Value; }
            set { offsetTwo.Value = value; }
        }

        public JoinDirection Direction
        {
            get
            {
                if (forward.Checked) return JoinDirection.ForwardOnly;
                if (backward.Checked) return JoinDirection.BackwardOnly;
                return JoinDirection.Both;
            }
            set
            {
                if (value == JoinDirection.ForwardOnly) forward.Checked = true;
                else if (value == JoinDirection.BackwardOnly) backward.Checked = true;
                else bidirectional.Checked = true;
            }
        }

        public event Action OK;

        public JoinForm()
        {
            InitializeComponent();
            joinType.SelectedIndex = 0;
        }

        public void Init(IEnumerable<MegaMan.Screen> screens)
        {
            foreach (MegaMan.Screen s in screens)
            {
                screenOne.Items.Add(s.Name);
                screenTwo.Items.Add(s.Name);
            }
        }

        private void AdjustLabels()
        {
            if (type == JoinType.Horizontal)
            {
                s1label.Text = "Upper Screen";
                s2label.Text = "Lower Screen";
                forward.Text = "Downward Only";
                backward.Text = "Upward Only";
            }
            else
            {
                s1label.Text = "Left Screen";
                s2label.Text = "Right Screen";
                forward.Text = "Rightward Only";
                backward.Text = "Leftward Only";
            }
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (OK != null) OK();
            this.Close();
        }

        private void joinType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Type = (joinType.SelectedIndex == 0) ? JoinType.Vertical : JoinType.Horizontal;
        }
    }
}
