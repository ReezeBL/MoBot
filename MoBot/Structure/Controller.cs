using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure
{
    class Controller
    {
        internal Model model;
        internal void HandleConnect()
        {
            model.Connect("151.80.33.194", 24444, "NoliSum");
        }
    }
}
