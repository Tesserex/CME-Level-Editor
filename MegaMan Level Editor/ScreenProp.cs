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
        public MegaMan.Screen Screen { get; private set; }
        public string ScreenName { get { return this.textName.Text; } }
        public int ScreenHeight { get { return (int)this.heightField.Value; } }
        public int ScreenWidth { get { return (int)this.widthField.Value; } }

        public event Action<ScreenProp> OK;

        public ScreenProp()
        {
            InitializeComponent();

            this.textName.Text = "";
        }

        public ScreenProp(MegaMan.Screen screen)
        {
            InitializeComponent();

            this.Screen = screen;

            this.textName.Text = screen.Name;
            this.widthField.Value = screen.Width;
            this.heightField.Value = screen.Height;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (ScreenName == "")
            {
                MessageBox.Show("Screen must have a name.", "CME Level Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (OK != null) OK(this);
            this.Close();
        }
    }
}
