using System;
using System.Windows.Forms;
using MoBot.Structure;
using MoBot.Structure.Game;
using NLog;

namespace MoBot
{
    class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static Logger GetLogger()
        {
            return Log;
        }
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GameBlock.LoadBlocks();
            NetworkController model = NetworkController.GetInstance();
            Controller controller = new Controller();
            Viewer viewer = new Viewer { MainController = controller };
            model.Subscribe(viewer);
            Application.Run(viewer);
        }
    }
}
