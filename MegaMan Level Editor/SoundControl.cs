using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MegaMan;

namespace MegaMan_Level_Editor
{
    public partial class SoundControl : UserControl
    {
        private AudioType type = AudioType.Unknown;

        public SoundControl()
        {
            InitializeComponent();
        }

        public SoundInfo GetInfo(string basePath)
        {
            SoundInfo info = new SoundInfo();
            info.Type = this.type;
            if (type == AudioType.NSF)
            {
                info.NsfTrack = (int)trackNumeric.Value;
            }
            else if (type == AudioType.Wav)
            {
                info.Path = FilePath.FromAbsolute(pathText.Text, basePath);
            }
            info.Priority = (byte)priorityNumeric.Value;
            return info;
        }

        private void typeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (typeCombo.SelectedIndex == 0)
            {
                type = AudioType.NSF;
                nsfPanel.Visible = true;
                wavPanel.Visible = false;
            }
            else
            {
                type = AudioType.Wav;
                nsfPanel.Visible = false;
                wavPanel.Visible = true;
            }
        }
    }
}
