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
        private ScreenDocument screen;
        private MapDocument map;

        private bool is_new = false;

        public event Action<ScreenProp> OK;

        // this constructor implies a new screen
        public ScreenProp(MapDocument map)
        {
            InitializeComponent();
            is_new = true;
            this.textName.Text = "";
            this.map = map;
        }

        // this implies editing a screen
        public ScreenProp(ScreenDocument screen)
        {
            InitializeComponent();

            this.screen = screen;
            this.map = screen.Map;

            this.textName.Text = screen.Name;
            this.widthField.Value = screen.Width;
            this.heightField.Value = screen.Height;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (this.textName.Text == "")
            {
                MessageBox.Show("Screen must have a name.", "CME Level Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (is_new)
            {
                map.AddScreen(this.textName.Text, (int)this.heightField.Value, (int)this.widthField.Value);
            }
            else
            {
                // Rename the screen
                screen.Name = this.textName.Text;
                screen.Resize((int)this.heightField.Value, (int)this.widthField.Value);
            }

            if (OK != null) OK(this);
            this.Close();
        }
    }
}
