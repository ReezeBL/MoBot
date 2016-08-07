using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MoBot.Structure.Game;
using MoBot.Structure.Game.AI.Pathfinding;
using MoBot.Structure.Game.World;

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
                ActionManager.SendChatMessage(message);
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
                        for(var i = 1;i<=4;i++)
                        {
                            for(var  j=0;j<9;j++)
                                sb.Append($"{i * 9 + j} : {GameController.Player.Inventory[i * 9 + j]} ");
                            sb.AppendLine();
                        }
                        Console.WriteLine(sb.ToString());
                    }
                        break;
                    case "-swap":
                        ActionManager.ExchangeInventorySlots(int.Parse(split[1]), int.Parse(split[2]));
                        break;
                    case "-move":
                    {
                            GameController.AiHandler.Mover.SetDestination(
                                new PathPoint(int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3])));
                        }
                        break;
                    case "-test":
                        var blocks = GameController.World.GetBlocks(Settings.IntrestedBlocks);
                        var grouped = blocks.GroupBy(block => block.Id);
                        foreach (var group in grouped)
                        {
                            Console.WriteLine($"{group.Key} : {group.Count()}");
                        }
                        break;
                    default:
                    {
                        NetworkController.NotifyViewer("Unknown command!");
                    }
                        break;
                }
            }
        }

        public void HandleConnect(string username, string serverIp)
        {
            if (NetworkController.Connected)
                return;
            var split = serverIp.Split(':');
            NetworkController.Connect(split[0], int.Parse(split[1]), username);
        }
    }
}
