using MoBot.Structure.Game;
using System;
using System.Text;

namespace MoBot.Structure
{
    internal class Controller
    {
        public Model Model;
  
        public void HandleConnect()
        {
            Model.Connect("151.80.33.194", 24444, "NoliSum");
        }

        public void HandleChatMessage(string message)
        {
            var manager = GameController.GetInstance().ActionManager;
            var controller = GameController.GetInstance();
            if (!message.StartsWith("-"))
                manager.SendChatMessage(message);
            else
            {
                var split = message.Split(' ');
                switch (split[0])
                {
                    case "-elist":
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Nearby entities:");
                        foreach (var e in controller.GetEntities<LivingEntity>())
                        {
                            sb.AppendLine($"--{e}");
                        }
                        Model.Viewer.OnNext(new Actions.ActionMessage { message = sb.ToString() });
                    }
                        break;
                    case "-inventory":
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("Player inventory:");
                        for(var i = 1;i<=4;i++)
                        {
                            for(var  j=0;j<9;j++)
                                sb.Append($"{i * 9 + j} : {controller.Player.Inventory[i * 9 + j]} ");
                            sb.AppendLine();
                        }
                        Console.WriteLine(sb.ToString());
                    }
                        break;
                    case "-swap":
                        manager.ExchangeInventorySlots(int.Parse(split[1]), int.Parse(split[2]));
                        break;
                }
            }
        }

        internal void HandleConnect(string username, string serverIp)
        {
            String[] split = serverIp.Split(':');
            Model.Connect(split[0], int.Parse(split[1]), username);
        }
    }
}
