﻿using MoBot.Structure.Game;
using MoBot.Structure.Game.AI.Modules;
using MoBot.Structure.Game.AI.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure
{
    class Controller
    {
        public Model model;
        public void HandleConnect()
        {
            model.Connect("151.80.33.194", 24444, "NoliSum");
        }

        public void HandleChatMessage(String message)
        {
            if (!message.StartsWith("-"))
                model.controller.SendChatMessage(message);
            else
            {
                string[] split = message.Split(' ');
                if (split[0] == "-elist")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Nearby entities:");
                    foreach (Entity e in model.controller.entityList.Values)
                    {
                        sb.AppendLine($"--{e.ToString()}");
                    }
                    model.viewer.OnNext(new Actions.ActionMessage { message = sb.ToString() });
                }
                else if(split[0] == "-load")
                {
                    model.controller.aiHandler.RegisterInternalModule(split[1]);
                }
                else if(split[0] == "-unload")
                {
                    model.controller.aiHandler.UnregisterModule(split[1]);
                }
                else if(split[0] == "-modules")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Loaded modules:");
                    foreach(String name in model.controller.aiHandler.moduleList.Keys)
                    {
                        sb.AppendLine($"--{name}");
                    }
                    model.viewer.OnNext(new Actions.ActionMessage { message = sb.ToString() });
                }
                else if(split[0] == "-property")
                {
                    model.controller.aiHandler.moduleList[split[1]].SetProperty(split[2], split[3]);
                }
                else if(split[0] == "-move")
                {
                    if (!model.controller.aiHandler.moduleList.ContainsKey("Movement"))
                        model.controller.aiHandler.RegisterModule(typeof(Movement));
                    var move = model.controller.aiHandler.moduleList["Movement"].module as Movement;
                    if (move.destPoint == null)
                        move.destPoint = new PathPoint { x = int.Parse(split[1]), y = (int)(model.controller.model.controller.player.y - 1.6), z = int.Parse(split[2]) };
                    
                }
            }
        }

        internal void HandleConnect(string username, string serverIP)
        {
            String[] split = serverIP.Split(':');
            model.Connect(split[0], int.Parse(split[1]), username);
        }
    }
}
