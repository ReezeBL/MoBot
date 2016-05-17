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
            mainAIController.ActionManager.UpdateMotion().Wait();
            if (pf == null)
            {
                pf = new PathFinder { world = mainAIController.World };
            }
            if (destPoint != null)
            {
                currPath = pf.flatPath(mainAIController.Player, destPoint);
                destPoint = null;   
                if(currPath != null)
                    currPoint = currPath.dequeue();
            }
            if (currPoint != null)
            {
                double dx = (currPoint.x + 0.5 * Math.Sign(currPoint.x)) - mainAIController.Player.x;
                double dz = (currPoint.z + 0.5 * Math.Sign(currPoint.z)) - mainAIController.Player.z;
                double dy = currPoint.y - mainAIController.Player.y;
                double abs = Math.Sqrt(dx * dx + dz * dz + dy * dy);
                double hAbs = Math.Sqrt(dx * dx + dz * dz);
                if (abs <= speed)
                    currPoint = currPath.dequeue();
                else {
                    mainAIController.ActionManager.RotatePlayer(dx, dy, dz);
                    if(dy > 0.1)                  
                        mainAIController.ActionManager.SetPlayerPos(mainAIController.Player.x, mainAIController.Player.y + dy, mainAIController.Player.z).Wait();                    
                    else if(hAbs > 0.2)
                        mainAIController.ActionManager.SetPlayerPos(mainAIController.Player.x + dx / abs * speed, mainAIController.Player.y, mainAIController.Player.z + dz / abs * speed).Wait();
                    else
                        mainAIController.ActionManager.SetPlayerPos(mainAIController.Player.x, mainAIController.Player.y + dy, mainAIController.Player.z).Wait();
                }
            }
        }
    }
}
