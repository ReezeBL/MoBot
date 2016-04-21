using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game.AI.Modules
{
    class AntiAFK : AIModule
    {       
        sbyte dir = 1;       
        public override void tick()
        {
            double diff = (DateTime.Now - mainAIController.lastMove).TotalSeconds;
            if (diff >= 5)
            {
                Player player = mainAIController.player;
                mainAIController.SetPlayerPos(player.x + dir * 2, player.y, player.z).Wait();
                dir *= -1;
                mainAIController.model.viewer.OnNext(new Actions.ActionMessage { message = $"Anti-afk move to {(int)player.x}|{(int)player.y}|{(int)player.z}" });           
            }
        }
    }
}
