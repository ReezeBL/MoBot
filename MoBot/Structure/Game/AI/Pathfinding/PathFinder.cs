using MinecraftEmuPTS.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game.AI.Pathfinding
{
    class PathFinder
    {
        public World world;        
        public Path flatPath(LivingEntity entity, PathPoint end)
        {
            return flatPath(new PathPoint { x = (int)entity.x, y = (int)(entity.y - 1.6), z = (int)entity.z }, end);
        }

        public Path flatPath(PathPoint start, PathPoint end)
        {
            var frontier = new Priority_Queue.FastPriorityQueue<PathPoint>(1000);
            Dictionary<PathPoint, int> start_cost = new Dictionary<PathPoint, int>();
            frontier.Enqueue(start, 0);
            start.prev = null;
            start_cost.Add(start, 0);
            while(frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (current.Equals(end))
                {
                    end = current;
                    break;
                }
                foreach(var next in getNeighbours(current))
                {
                    int cost = start_cost[current] + 1;
                    if (!start_cost.ContainsKey(next))
                    {
                        start_cost.Add(next, cost);
                        frontier.Enqueue(next, cost + next.DistanceTo(end));
                        next.prev = current;
                    }
                    else if (start_cost[next] > cost)
                    {
                        start_cost[next] = cost;
                        frontier.Enqueue(next, cost + next.DistanceTo(end));
                        next.prev = current;
                    }
                }
            }
            List<PathPoint> pathfrom = new List<PathPoint>();
            while(end != null)
            {
                pathfrom.Add(end);
                end = end.prev;
            }
            pathfrom.Reverse();
            return new Path(pathfrom);
        }

        IEnumerable<PathPoint> getNeighbours(PathPoint point)
        {
            List<PathPoint> candidats = new List<PathPoint>();
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (i != 0 || j != 0)
                        candidats.Add(isCollideable(world.GetBlock(point.x + i, point.y, point.z + j)) ? new PathPoint { x = point.x + i, y = point.y, z = point.z + j } : null);
            var result = candidats.Where(a => a != null);
            return result;
        }

        private bool isCollideable(Block a)
        {
            return a==null || a.ID == 0;
        }
    }
}
