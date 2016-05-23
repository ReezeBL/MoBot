using MoBot.Structure;
using NLog;
using System;
using System.Windows.Forms;

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
            Protocol.Channel.InitDicts();
            Structure.Game.GameBlock.LoadBlocks();
            NetworkController model = NetworkController.GetInstance();
            Controller controller = new Controller();
            Viewer viewer = new Viewer { mainController = controller };
            model.Subscribe(viewer);
            Application.Run(viewer);
        }
    }
}
