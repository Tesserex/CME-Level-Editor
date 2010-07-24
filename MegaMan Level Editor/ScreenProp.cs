﻿using System;
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

        public static void CreateScreen(MapDocument map)
        {
            new ScreenProp(map).Show();
        }

        public static void EditScreen(ScreenDocument screen)
        {
            new ScreenProp(screen).Show();
        }

        // this constructor implies a new screen
        private ScreenProp(MapDocument map)
        {
            InitializeComponent();
            is_new = true;
            this.textName.Text = "";
            this.map = map;
        }

        // this implies editing a screen
        private ScreenProp(ScreenDocument screen)
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
                map.AddScreen(this.textName.Text, (int)this.widthField.Value, (int)this.heightField.Value);
            }
            else
            {
                // Rename the screen
                screen.Name = this.textName.Text;
                screen.Resize((int)this.widthField.Value, (int)this.heightField.Value);
            }

            if (OK != null) OK(this);
            this.Close();
        }
    }
}
