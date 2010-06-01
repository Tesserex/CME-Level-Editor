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
        public string stageName, screenName;
        public bool Confirmed { get; private set; }
        public Action<string, string, string> onClose;

        public ScreenProp()
        {
            InitializeComponent();
            Confirmed = false;

            this.textName.Text = "";
            this.textHeight.Text = "";
            this.textWidth.Text = "";
        }

        public ScreenProp(string stageName, string screenName)
        {
            InitializeComponent();
            Confirmed = false;
            this.stageName = stageName;
            this.screenName = screenName;

            var screen = MainForm.GetScreen(stageName, screenName);
            this.textName.Text = screen.Name;
            this.textHeight.Text = screen.Height.ToString();
            this.textWidth.Text = screen.Width.ToString();
        }

        public void buttonOK_Click(object sender, EventArgs e)
        {
            Confirmed = true;
            if (stageName != null && screenName != null)
                SaveScreenOnClose(this.textName.Text, this.textWidth.Text, this.textHeight.Text);
            else
                CreateScreenOnClose(this.textName.Text, this.textWidth.Text, this.textHeight.Text);
            this.Close();
        }

        public void CreateScreenOnClose(string name, string width, string height)
        {
            var screen = new MegaMan.Screen(int.Parse(width), int.Parse(height), MainForm.Instance.ActiveMap.Map);
            screen.Name = name;
            screen.Resize(int.Parse(width), int.Parse(height));
            MainForm.Instance.ActiveMap.Map.Screens.Add(name, screen);
            screen.Save(MainForm.ScreenPathFor(MainForm.Instance.ActiveMap.Map.Name, name));
        }

        // TODO: 
        //   The pattern "MainForm.Instance.stages[stageName].Map.Screens[screenName]"
        //   will slowly become the model with the View/Controller being the UI.
        public void SaveScreenOnClose(string newScreenName, string width, string height)
        {
            var oldScreenName = screenName;

            // Rename the screen
            var screen = MainForm.GetScreen(stageName, oldScreenName);
            screen.Resize(int.Parse(width), int.Parse(height));
            MainForm.UpdateScreenName(stageName, oldScreenName, newScreenName);

            // Update the project tree
            var projectForm = MainForm.Instance.projectForm;
            var stageNode = projectForm.projectView.Nodes.Find(stageName, true).First();
            var screens = MainForm.GetStage(stageName).Screens.Select((pair) => { return pair.Value; }).ToList();
            projectForm.LoadScreenSubtree(stageNode, screens);

            var stageForm = MainForm.Instance.stageForms[stageName];
            var surface = stageForm.GetSurface(oldScreenName);

            // Update the screen surfaces
            if (oldScreenName != newScreenName)
            {
                stageForm.RenameSurface(oldScreenName, newScreenName);
            }

            surface.ResizeLayers();
            stageForm.AlignScreenSurfaces();
        }
    }
}
