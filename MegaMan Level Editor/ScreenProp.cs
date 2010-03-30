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
    public partial class ScreenProp : Form
    {
        private MegaMan.Screen screen;
        public bool Confirmed { get; private set; }

        public ScreenProp()
        {
            InitializeComponent();
            Confirmed = false;
        }

        public void LoadScreen(MegaMan.Screen screen)
        {
            this.screen = screen;
            this.textName.Text = screen.Name;
            this.textHeight.Text = screen.Height.ToString();
            this.textWidth.Text = screen.Width.ToString();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            screen.Name = this.textName.Text;
            screen.Resize(int.Parse(this.textWidth.Text), int.Parse(this.textHeight.Text));

            Confirmed = true;
            this.Close();
        }
    }
}
