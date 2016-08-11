using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Structure.Game.AI.Pathfinding;
using NLog;
using TreeSharp;

namespace MoBot.Structure.Game.AI.Tasks
{
    public class Digger : Task
    {
        public bool enableDig;
        private readonly Logger _logger = Program.GetLogger();

        private void SetupDigger(object context)
        {
            var location = GameController.World.SearchBlock(Settings.IntrestedBlocks);
            if (location != null)
            {
                var playerPos = (Location) GameController.Player.Position;
                double distance = playerPos.DistanceTo(location);
                Console.WriteLine(
                    $"Target: {Block.GetBlock(GameController.World.GetBlock(location)).Name} {{{location.X}|{location.Y}|{location.Z}}}\r\nDistance: {distance}");

                GameController.AiHandler.Mover.SetShovelDestination(location);
            }
            else
            {
                _logger.Info("Cant find any interesting blocks, diiging stopped!");
                enableDig = false;
            }
        }

        public Digger()
        {
            _root = new Decorator(o => enableDig, new TreeSharp.Action(o=> SetupDigger(o)));
        }
    }
}
