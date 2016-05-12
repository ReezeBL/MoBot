using MoBot.Structure;
using NLog;
using System;
using System.Windows.Forms;

namespace MoBot
{
    class Program
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        public static Logger getLogger()
        {
            return log;
        }
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Protocol.Channel.InitDicts();
            Structure.Game.GameBlock.loadBlocks();
            Model model = new Model();
            Controller controller = new Controller { model = model };
            Viewer viewer = new Viewer { mainController = controller };
            model.Subscribe(viewer);
            Application.Run(viewer);
        }
    }
}
