using System;
using System.Collections.Generic;
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
            var block = GameController.World.SearchBlock(Settings.IntrestedBlocks);
            if (block != null)
            {
                var playerPos = (PathPoint)GameController.Player.Position;
                double distance = playerPos.DistanceTo(block);
                Console.WriteLine(
                    $"Target: {GameBlock.GetBlock(block.Id).Name} {{{block.X}|{block.Y}|{block.Z}}}\r\nDistance: {distance}");

                GameController.AiHandler.Mover.SetShovelDestination(block);
            }
        }

        public Digger()
        {
            _root = new Decorator(o => enableDig, new TreeSharp.Action(o=> SetupDigger(o)));
        }
    }
}
