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
            model.controller.SendChatMessage(message);
        }
    }
}
