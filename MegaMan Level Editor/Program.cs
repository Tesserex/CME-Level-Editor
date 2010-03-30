using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MegaMan_Level_Editor
{
    static class Program
    {
        private static Timer timer;
        public static event Action FrameTick;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            timer = new Timer();
            timer.Interval = (int)(1000 / Const.FPS);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            Application.Run(new MainForm());
        }

        static void timer_Tick(object sender, EventArgs e)
        {
            if (FrameTick != null) FrameTick();
        }

        public static void Animate(bool animate)
        {
            timer.Enabled = animate;
        }
    }
}
