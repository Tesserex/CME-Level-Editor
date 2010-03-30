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
    public partial class LevelProp : Form
    {
        private Map map;
        private string tileset;

        public LevelProp()
        {
            InitializeComponent();
        }

        public void LoadMap(Map map)
        {
            this.map = map;
            tilesetLabel.Text = map.TilePath;
            tileset = map.TilePath;
        }

        private void tilesetChange_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                tileset = tilesetLabel.Text = dialog.FileName;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Save();
            Cancel();
        }

        private void Save()
        {
            map.ChangeTileset(tileset);
        }

        private void Cancel()
        {
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }
    }
}
