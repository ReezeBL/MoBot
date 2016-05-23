using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoBot.Structure.Game.World;
using Priority_Queue;

namespace MoBot.Structure.Game.AI.Pathfinding
{
    internal class PathFinder
    {
        public PathFinder(bool canFly = false, bool canDig = false)
        {
            _canFly = canFly;
            _canDig = canDig;
        }

        private readonly GameWorld _world = GameController.World;
        private readonly Hashtable _pointSet = new Hashtable();
        private readonly bool _canFly;
        private readonly bool _canDig;
        private readonly FastPriorityQueue<PathPoint> _frontier = new FastPriorityQueue<PathPoint>(30000);
        public Path StaticPath(LivingEntity entity, PathPoint end)
        {
            return
                StaticPath(
                    new PathPoint(MathHelper.floor_double(entity.X), MathHelper.floor_double(entity.Y),
                        MathHelper.floor_double(entity.Z)), end);
        }

        public Path StaticPath(PathPoint start, PathPoint end)
        {
            try
            {
                _frontier.Clear();
                Hashtable startCost = new Hashtable();
                _frontier.Enqueue(start, 0);
                start.Prev = null;
                startCost.Add(start, 0);
                while (_frontier.Count > 0)
                {
                    var current = _frontier.Dequeue();
                    if (current.Equals(end))
                    {
                        end = current;
                        break;
                    }
                    foreach (var next in GetNeighbours(current))
                    {
                        int cost = (int) startCost[current] + 1;
                        if (!startCost.ContainsKey(next))
                        {
                            startCost.Add(next, cost);
                            _frontier.Enqueue(next, cost + next.DistanceTo(end));
                            next.Prev = current;
                        }
                        else if ((int) startCost[next] > cost)
                        {
                            startCost[next] = cost;
                            _frontier.UpdatePriority(next, cost + next.DistanceTo(end));
                            next.Prev = current;
                        }
                    }
                }
                List<PathPoint> pathfrom = new List<PathPoint>();
                while (end.Prev != null)
                {
                    pathfrom.Add(end);
                    end = end.Prev;
                }
                pathfrom.Reverse();
                return new Path(pathfrom);
            }
            catch (Exception e)
            {
                Program.GetLogger().Error($"Cant create path! Error : {e}");
                return null;
            }
        }

        public Path DynamicPath(Entity entity, PathPoint endPoint)
        {
            return new Path(DynamicPathGenerator(entity, endPoint));
        }
        public Path DynamicPath(PathPoint start, PathPoint endPoint)
        {
            return new Path(DynamicPathGenerator(start, endPoint));
        }
        private IEnumerator<PathPoint> DynamicPathGenerator(Entity entity, PathPoint endPoint)
        {
            return DynamicPathGenerator(new PathPoint((int) entity.X, (int) entity.Y, (int) entity.Z),  endPoint);
        }
        private IEnumerator<PathPoint> DynamicPathGenerator(PathPoint startPoint, PathPoint endPoint)
        {
            while(true)
            {
                var validation = GameController.World.WorldValidation;
                var path = StaticPath(startPoint, endPoint);              
                if (path == null || startPoint.Equals(endPoint))
                    yield break;
                foreach (var point in path)
                {
                    if(validation != GameController.World.WorldValidation)
                        break;
                    yield return startPoint = point;
                }
            }
        }

        private IEnumerable<PathPoint> GetNeighbours(PathPoint point)
        {
            List<PathPoint> candidats = new List<PathPoint>();
            int canJump = _world.CanMoveTo(point.X, point.Y + 1, point.Z) ? 1 : 0;
            candidats.Add(GetSafePoint(point.X + 1, point.Y, point.Z, canJump));
            candidats.Add(GetSafePoint(point.X - 1, point.Y, point.Z, canJump));
            candidats.Add(GetSafePoint(point.X, point.Y, point.Z + 1, canJump));
            candidats.Add(GetSafePoint(point.X, point.Y, point.Z - 1, canJump));
            var res = candidats.Where(x => x != null);
            return res;
        }

        private PathPoint GetSafePoint(int x, int y, int z, int jumpBlocks = 0)
        {
            PathPoint res = null;
            if (_world.CanMoveTo(x, y, z))
                res = CreatePoint(x, y, z);
            else if (jumpBlocks > 0)
            {
                if (_world.CanMoveTo(x, y + jumpBlocks, z))
                {
                    res = CreatePoint(x, y + jumpBlocks, z);
                    y += jumpBlocks;
                }
            }

            if (res == null) return null;
            while (y > 0)
                if (_world.CanMoveTo(x, y - 1, z))
                    y--;
                else
                    break;
            res = CreatePoint(x, y, z);
            return res;
        }

        private PathPoint CreatePoint(int x, int y, int z)
        {
            var p = new PathPoint(x, y, z);
            if (_pointSet.ContainsKey(p))
                return _pointSet[p] as PathPoint;
            _pointSet.Add(p, p);
            return p;
        }
    }
}
