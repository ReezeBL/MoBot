﻿using System;
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

                    double distance = playerPos.DistanceTo(location);
                    Console.WriteLine(
                        $"Target: {Block.GetBlock(GameController.World.GetBlock(location)).Name} {{{location.X}|{location.Y}|{location.Z}}}\r\nDistance: {distance}");

                    if (distance < 1)
                    {
                        Console.WriteLine($"Wierd position, skipping in this turn");
                        continue;
                    }

                    var preBuild = PathFinder.Shovel(playerPos, location);
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
