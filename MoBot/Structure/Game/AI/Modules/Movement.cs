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
            if( pf == null)
            {
                pf = new PathFinder { world = mainAIController.world };
            }
            if(destPoint != null)
            {
                currPath = pf.flatPath(mainAIController.player, destPoint);
                destPoint = null;
                currPoint = currPath.dequeue();
            }
            if(currPoint != null)
            {
                double dx = mainAIController.player.x - (currPoint.x + 0.5);
                double dz = mainAIController.player.z - (currPoint.z + 0.5);
                double abs = Math.Sqrt(dx * dx + dz * dz);
                if (abs <= speed)
                    currPoint = currPath.dequeue();
                else
                    mainAIController.SetPlayerPos(mainAIController.player.x - dx / abs * speed, mainAIController.player.y, mainAIController.player.z - dz / abs * speed).Wait();
            }
        }
    }
}
