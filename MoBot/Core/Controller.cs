using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MoBot.Core.Game;
using MoBot.Core.Game.AI.Pathfinding;
using MoBot.Core.Game.AI.Tasks;

namespace MoBot.Core
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
                    case "-help":
                        const string helpMessage = @"Список комманд:
-open X Y Z - открывает сундук на координатах X Y Z
-openBase - открывает сундук на базе
-close - закрывает текущий инвентарь
-returnBase - форсит складирование предметов
-returnDig - телепортирует на сохраненную точку копания";
                        NetworkController.NotifyViewer(helpMessage);
                        break;
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
                        foreach (var e in GameController.GetEntities<Entity>())
                        {
                            sb.AppendLine($"--{e}");
                        }
                        Console.WriteLine(sb.ToString());
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
                        var p = ActionManager.ExchangeInventorySlots(int.Parse(split[1]), int.Parse(split[2]));
                        while (p.MoveNext())
                            Thread.Sleep(50);
                        break;
                    case "-move":
                    {
                        GameController.AiHandler.Mover.SetDestination(
                            new Location(int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3])));
                    }
                        break;

                    case "-open":
                    {
                        ActionManager.RightClick(int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3]));
                        break;
                    }
                    case "-openBase":
                        ActionManager.RightClick(CustomEvents.StoreChest);
                        break;
                    case "-close":
                        ActionManager.CloseWindow();
                        break;
                    case "-returnBase":
                        GameController.AiHandler.CustomEvents.Store = true;
                        break;
                    case "-returnDig":
                        ActionManager.SendChatMessage(GameController.AiHandler.CustomEvents.TeleportCheckpoint);
                        break;
                    case "-test":
                        var ids = new HashSet<int> {54, 130, 146, 181, 191, 506, };
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
                        GameController.AiHandler.Digger.EnableDig = !GameController.AiHandler.Digger.EnableDig;
                        break;
                    case "-stop":
                        GameController.AiHandler.Mover.Stop();
                        GameController.AiHandler.Digger.EnableDig = false;
                        break;
                    case "-reset":
                        GameController.AiHandler.CustomEvents.MinFoodDanger = 5;
                        break;
                    case "-use":
                        ActionManager.UseItem();
                        break;
                    case "-tp":
                        ActionManager.SetPlayerPos(int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3]));
                        break;
                    case "-u":
                        ActionManager.UpdatePosition();
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
            var split = serverIp.Split(':');
            if (split.Length != 2)
            {
                Console.WriteLine("Invalid server string format!");
                NetworkController.Disconnect();
                return;
            }
            NetworkController.ConnectAsync(split[0], int.Parse(split[1]), username, delay);
        }
    }
}
