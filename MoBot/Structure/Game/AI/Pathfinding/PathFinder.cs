using MinecraftEmuPTS.GameData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game.AI.Pathfinding
{
    class PathFinder
    {
        public World world;
        private Hashtable pointSet = new Hashtable();    
        public Path flatPath(LivingEntity entity, PathPoint end)
        {
            return flatPath(new PathPoint { x = MathHelper.floor_double(entity.x), y = MathHelper.floor_double(entity.y), z =MathHelper.floor_double(entity.z) }, end);
        }

        public Path flatPath(PathPoint start, PathPoint end)
        {
            try {              
                var frontier = new Priority_Queue.FastPriorityQueue<PathPoint>(300);
                Hashtable start_cost = new Hashtable();               
                frontier.Enqueue(start, 0);
                start.prev = null;
                start_cost.Add(start, 0);               
                while (frontier.Count > 0)
                {
                    var current = frontier.Dequeue();                                      
                    if (current.Equals(end))
                    {
                        end = current;
                        break;
                    }                    
                    foreach (var next in getNeighbours(current))
                    {
                        int cost = (int)start_cost[current] + 1;
                        if (!start_cost.ContainsKey(next))
                        {
                            start_cost.Add(next, cost);
                            frontier.Enqueue(next, cost + next.DistanceTo(end));
                            next.prev = current;
                        }
                        else if ((int)start_cost[next] > cost)
                        {
                            start_cost[next] = cost;
                            frontier.UpdatePriority(next, cost + next.DistanceTo(end));                          
                            next.prev = current;
                        }
                    }
                    
                }
                List<PathPoint> pathfrom = new List<PathPoint>();
                while (end != null)
                {
                    pathfrom.Add(end);
                    end = end.prev;
                }
                pathfrom.Reverse();
                Console.WriteLine($"Debugging path from {start} to {end}");
                foreach (var pp in pathfrom)
                {
                    Console.WriteLine($"Path point: {pp}");
                }                      
                return new Path(pathfrom);
            }
            catch(Exception e)
            {
                Program.getLogger().Error($"Cant create path! Error : {e.ToString()}");
                return null;
            }
        }

        IEnumerable<PathPoint> getNeighbours(PathPoint point)
        {
            List<PathPoint> candidats = new List<PathPoint>();
            int canJump = canMoveTo(point.x, point.y + 1, point.z) ? 1 : 0;
            candidats.Add(getSafePoint(point.x + 1, point.y, point.z, canJump));
            candidats.Add(getSafePoint(point.x - 1, point.y, point.z, canJump));
            candidats.Add(getSafePoint(point.x, point.y, point.z + 1, canJump));
            candidats.Add(getSafePoint(point.x, point.y, point.z - 1, canJump));
            IEnumerable<PathPoint> res = candidats.Where(x => x != null);
            return res;
        }

        private PathPoint getSafePoint(int x, int y, int z, int jumpBlocks = 0)
        {
            PathPoint res = null;
            if (canMoveTo(x, y, z))
                res = createPoint(x, y, z);
            else if(jumpBlocks > 0)
            {
                if (canMoveTo(x, y + jumpBlocks, z))
                {
                    res = createPoint(x, y + jumpBlocks, z);
                    y += jumpBlocks;
                }
            }

            if(res != null)
            {
                while (y > 0)
                    if (canMoveTo(x, y - 1, z))
                        y--;
                    else
                        break;
                res = createPoint(x, y, z);
            }
            return res;
        }

        private bool canMoveTo(int x, int y, int z)
        {
            x = x < 0 ? x - 1 : x;
            z = z < 0 ? z - 1 : z;

            Block floor = world.GetBlock(x, y, z);
            Block upper = world.GetBlock(x, y + 1, z);

            return isBlockFree(floor) && isBlockFree(upper);
        }

        private bool isBlockFree(Block b)
        {
            return b == null || GameBlock.getBlock(b.ID).transparent;
        }
        private PathPoint createPoint(int x, int y, int z)
        {      
            var p =  new PathPoint { x = x, y = y, z = z };
            if (pointSet.ContainsKey(p))
                return pointSet[p] as PathPoint;
            else
                pointSet.Add(p, p);
            return p;
        }
    }
}
