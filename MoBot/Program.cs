using MoBot.Scripts;
using MoBot.Structure;
using MoBot.Structure.Game;
using MoBot.Structure.Game.Items;
using MoBot.Structure.Windows;
using NLog;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MoBot
{
    public class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static Logger GetLogger()
        {
            return Log;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AllocConsole();

            Console.WriteLine("Loading scripts...");
            ScriptLoader.LoadScripts();

            Console.WriteLine("Loading blocks...");
            Block.LoadBlocks();

            Console.WriteLine("Loading items...");
            Item.LoadItems();

            Console.WriteLine("Loading entites...");
            Entity.LoadEntities();

            Console.WriteLine("Everything is done! Application is ready to launch!");

            var model = NetworkController.Instance;
            var controller = new Controller();
            var viewer = new Viewer { MainController = controller };
            model.Subscribe(viewer);
            //FreeConsole();

            Application.Run(viewer);
        }
    }
}
