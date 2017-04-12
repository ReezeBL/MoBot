using System;
using System.Collections.Generic;
using MoBot.Core.Game.AI.Pathfinding;
using NLog;
using TreeSharp;

namespace MoBot.Core.Game.AI.Tasks
{
    public class Digger : Task
    {
        public bool EnableDig;

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly HashSet<Location> ignoredLocations = new HashSet<Location>();
        private void SetupDigger(object context)
        {
            var locations = GameController.World.SearchBlocks(Settings.IntrestedBlocks);
            if (locations.Count > 0)
            {
                Location playerPos = GameController.Player.Position;
                foreach (var location in locations)
                {
                    if (ignoredLocations.Contains(location)) continue;

                    var distance = playerPos.DistanceTo(location);
                    Console.WriteLine(
                        $"Target: {Block.GetBlock(GameController.World.GetBlock(location)).Name} {{{location.X}|{location.Y}|{location.Z}}}\r\nDistance: {distance}");

                    if (distance < 1)
                    {
                        Console.WriteLine($"Wierd position, skipping in this turn");
                        continue;
                    }

                    var preBuild = PathFinder.Shovel(playerPos, location, maxDistance: 2*Settings.ScanRange);
                    if (preBuild == null)
                    {
                        ignoredLocations.Add(location);
                        Console.WriteLine("Failed to buid path,ignoring this point");
                        continue;
                    }
                    GameController.AiHandler.Mover.SetShovelDestination(location, preBuild);
                    break;
                }
                
            }
            else
            {
                Log.Info("Cant find any interesting blocks, diiging stopped!");
                EnableDig = false;
                ActionManager.SendChatMessage(GameController.AiHandler.CustomEvents.TeleportHome);
            }
        }

        public Digger()
        {
            Root = new Decorator(o => EnableDig, new TreeSharp.Action(o=> SetupDigger(o)));
        }
    }
}
