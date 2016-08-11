using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MoBot.Structure.Game;
using MoBot.Structure.Game.AI.Pathfinding;

namespace MoBot.Structure
{
    public class Controller
    {       
        public void HandleConnect()
        {
            if (NetworkController.Connected)
                return;
            NetworkController.Connect("151.80.33.194", 24444, "NoliSum");
        }

        public void HandleChatMessage(string message)
        {
            if (!NetworkController.Connected)
                return;

            if (!message.StartsWith("-"))
            {
                ActionManager.SendChatMessage(message);
            }
            else
            {
                var split = message.Split(' ');
                switch (split[0])
                {
                    case "-disconnect":
                    {
                        NetworkController.Disconnect();
                        NetworkController.NotifyViewer("Client diconnected!");
                    }
                        break;
                    case "-elist":
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("Nearby entities:");
                        foreach (var e in GameController.GetEntities<LivingEntity>())
                        {
                            sb.AppendLine($"--{e}");
                        }
                        NetworkController.NotifyViewer(sb.ToString());
                    }
                        break;
                    case "-inventory":
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("Player inventory:");
                        sb.Append(GameController.Player.CurrentContainer);
                        Console.WriteLine(sb.ToString());
                    }
                        break;
                    case "-swap":
                        foreach (var p in ActionManager.ExchangeInventorySlots(int.Parse(split[1]), int.Parse(split[2]))
                            )
                            Thread.Sleep(50);
                        break;
                    case "-move":
                    {
                        GameController.AiHandler.Mover.SetShovelDestination(
                            new Location(int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3])));
                    }
                        break;

                    case "-testOpen":
                    {
                        ActionManager.RightClick(int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3]));
                        break;
                    }
                    case "-testClose":
                        ActionManager.CloseWindow();
                        break;
                    case "-testReturn":
                        GameController.AiHandler.CustomEvents.Store = true;
                        break;
                    case "-test":
                        var ids = new HashSet<int> {54, 130, 146, 181, 191, 506};
                        var locations = GameController.World.SearchBlocks(ids);
                        Location playerLocation = GameController.Player.Position;
                        foreach (var location in locations)
                        {
                            var block = Block.GetBlock(GameController.World.GetBlock(location));
                            Console.WriteLine(
                                $"{block.Name} {location} \r\nDistance: {location.DistanceTo(playerLocation)}");
                        }
                        break;
                    case "-dig":
                        GameController.AiHandler.Digger.enableDig = !GameController.AiHandler.Digger.enableDig;
                        break;
                    case "-use":
                        ActionManager.UseItem();
                        break;
                    default:
                    {
                        NetworkController.NotifyViewer("Unknown command!");
                    }
                        break;
                }
            }
        }

        public void HandleConnect(string username, string serverIp, int delay = 0)
        {
            if (NetworkController.Connected)
                return;
            var split = serverIp.Split(':');
            NetworkController.ConnectAsync(split[0], int.Parse(split[1]), username, delay);
        }
    }
}
