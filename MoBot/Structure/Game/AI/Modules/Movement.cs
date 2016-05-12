using MoBot.Structure.Game.AI.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game.AI.Modules
{
    class Movement : AIModule
    {
        private PathFinder pf = null;
        private Path currPath = null;
        private PathPoint currPoint = null;
        public PathPoint destPoint = null;
        public float speed = 0.3f;
        public override void tick()
        {
            mainAIController.updateMotion().Wait();
            if (pf == null)
            {
                pf = new PathFinder { world = mainAIController.world };
            }
            if (destPoint != null)
            {
                currPath = pf.flatPath(mainAIController.player, destPoint);
                destPoint = null;   
                if(currPath != null)
                    currPoint = currPath.dequeue();
            }
            if (currPoint != null)
            {
                double dx = (currPoint.x + 0.5 * Math.Sign(currPoint.x)) - mainAIController.player.x;
                double dz = (currPoint.z + 0.5 * Math.Sign(currPoint.z)) - mainAIController.player.z;
                double dy = currPoint.y - mainAIController.player.y;
                double abs = Math.Sqrt(dx * dx + dz * dz + dy * dy);
                double hAbs = Math.Sqrt(dx * dx + dz * dz);
                if (abs <= speed)
                    currPoint = currPath.dequeue();
                else {
                    mainAIController.RotatePlayer(dx, dy, dz);
                    if(dy > 0.1)                  
                        mainAIController.SetPlayerPos(mainAIController.player.x, mainAIController.player.y + dy, mainAIController.player.z).Wait();                    
                    else if(hAbs > 0.2)
                        mainAIController.SetPlayerPos(mainAIController.player.x + dx / abs * speed, mainAIController.player.y, mainAIController.player.z + dz / abs * speed).Wait();
                    else
                        mainAIController.SetPlayerPos(mainAIController.player.x, mainAIController.player.y + dy, mainAIController.player.z).Wait();
                }
            }
        }
    }
}
