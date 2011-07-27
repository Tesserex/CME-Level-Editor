using System;
using System.Windows.Forms;

namespace MegaMan.LevelEditor {
    static class Program {
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

            timer = new Timer {Interval = (int) (1000/Const.FPS)};
            timer.Tick += timer_Tick;

            Application.Run(new MainForm());
        }

        static void timer_Tick(object sender, EventArgs e)
        {
            if (FrameTick != null) FrameTick();
        }

        public static bool Animated
        {
            get { return timer.Enabled; }
            set { timer.Enabled = value; }
        }
    }
}
