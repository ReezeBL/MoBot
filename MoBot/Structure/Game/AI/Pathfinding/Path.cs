using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game.AI.Pathfinding
{
    class Path
    {
        Queue<PathPoint> pathQueue = new Queue<PathPoint>();
        public void addPoint(PathPoint point)
        {
            pathQueue.Enqueue(point);
        }

        public PathPoint dequeue()
        {
            if (hasNext())
                return pathQueue.Dequeue();
            else return null;
        }

        public Path(List<PathPoint> list)
        {
            pathQueue = new Queue<PathPoint>(list);
        }

        public bool hasNext()
        {
            return pathQueue.Count > 0;
        }
        public Path()
        {
        }
    }
}
