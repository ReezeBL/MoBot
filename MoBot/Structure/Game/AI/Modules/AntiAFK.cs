using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game.AI.Modules
{
    class AntiAFK : AIModule
    {
        public sbyte dir { get; set; } = 1;
        public double delay { get; set; } = 20;     
        public override void tick()
        {
            double diff = (DateTime.Now - mainAIController.lastMove).TotalSeconds;
            if (diff >= delay)
            {
                Player player = mainAIController.player;
                mainAIController.SetPlayerPos(player.x + dir, player.y, player.z).Wait();
                dir *= -1;
                mainAIController.model.viewer.OnNext(new Actions.ActionMessage { message = $"Anti-afk move to {(int)player.x}|{(int)player.y}|{(int)player.z}" });           
            }
        }
    }
}
