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
    public partial class LevelProp : Form
    {
        private Map map;

        public event Action OkPressed;
        public event Action Saved;

        public LevelProp()
        {
            InitializeComponent();
        }

        public void LoadMap(Map map)
        {
            this.map = map;
            nameField.Text = map.Name;
            tilesetField.Text = map.TilePath;
        }

        private void tilesetChange_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                tilesetField.Text = dialog.FileName;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            bool success = Save();
            if (success)
            {
                if (OkPressed != null) OkPressed();
                this.Close();
            }
        }

        private bool Save()
        {
            try
            {
                map.Name = nameField.Text;
                map.ChangeTileset(tilesetField.Text);
                if (Saved != null) Saved();
                return true;
            }
            catch
            {
                MessageBox.Show("The tileset specified could not be loaded. Sorry.", "C# Mega Man Level Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private void Cancel()
        {
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            this.Save();
        }
    }
}
