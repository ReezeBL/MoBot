using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;

namespace MoBot.Core.Game.AI.Pathfinding
{
    public static class PathFinder
    {
        private static readonly Dictionary<int, Location> PointSet = new Dictionary<int, Location>();
        private static readonly Dictionary<int, float> WeightSet = new Dictionary<int, float>();

        #region OldPathFinder
        private static readonly FastPriorityQueue<Location> Frontier = new FastPriorityQueue<Location>(30000);

        public static Path StaticPath(LivingEntity entity, Location end)
        {
            return
                StaticPath(
                    new Location(MathHelper.FloorDouble(entity.X), MathHelper.FloorDouble(entity.Y),
                        MathHelper.FloorDouble(entity.Z)), end);
        }

        public static Path StaticPath(Location start, Location end)
        {
            try
            {
                Frontier.Clear();
                var startCost = new Hashtable();
                Frontier.Enqueue(start, 0);
                start.Prev = null;
                startCost.Add(start, 0);
                while (Frontier.Count > 0)
                {
                    var current = Frontier.Dequeue();
                    if (current.Equals(end))
                    {
                        end = current;
                        break;
                    }
                    foreach (var next in GetNeighbours(current))
                    {
                        var cost = (int) startCost[current] + 1;
                        if (!startCost.ContainsKey(next))
                        {
                            startCost.Add(next, cost);
                            Frontier.Enqueue(next, cost + next.DistanceTo(end));
                            next.Prev = current;
                        }
                        else if ((int) startCost[next] > cost)
                        {
                            startCost[next] = cost;
                            Frontier.UpdatePriority(next, cost + next.DistanceTo(end));
                            next.Prev = current;
                        }
                    }
                }
                var pathfrom = new List<Location>();
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

        public static Path DynamicPath(Entity entity, Location endPoint)
        {
            return new Path(DynamicPathGenerator(entity, endPoint));
        }

        public static Path DynamicPath(Location start, Location endPoint)
        {
            return new Path(DynamicPathGenerator(start, endPoint));
        }

        private static IEnumerator<Location> DynamicPathGenerator(Entity entity, Location endPoint)
        {
            return DynamicPathGenerator(new Location((int) entity.X, (int) entity.Y, (int) entity.Z), endPoint);
        }

        private static IEnumerator<Location> DynamicPathGenerator(Location startPoint, Location endPoint)
        {
            while (true)
            {
                var validation = GameController.World.WorldValidation;
                var path = StaticPath(startPoint, endPoint);
                if (path == null || startPoint.Equals(endPoint))
                    yield break;
                foreach (var point in path)
                {
                    if (validation != GameController.World.WorldValidation)
                        break;
                    yield return startPoint = point;
                }
            }
        }

        private static IEnumerable<Location> GetNeighbours(Location point)
        {
            var candidats = new List<Location>();
            var canJump = GameController.World.CanMoveTo(point.X, point.Y + 1, point.Z) ? 1 : 0;
            candidats.Add(GetSafePoint(point.X + 1, point.Y, point.Z, canJump));
            candidats.Add(GetSafePoint(point.X - 1, point.Y, point.Z, canJump));
            candidats.Add(GetSafePoint(point.X, point.Y, point.Z + 1, canJump));
            candidats.Add(GetSafePoint(point.X, point.Y, point.Z - 1, canJump));
            var res = candidats.Where(x => x != null);
            return res;
        }

        private static Location GetSafePoint(int x, int y, int z, int jumpBlocks = 0)
        {
            Location res = null;
            if (GameController.World.CanMoveTo(x, y, z))
                res = CreatePoint(x, y, z);
            else if (jumpBlocks > 0)
            {
                if (GameController.World.CanMoveTo(x, y + jumpBlocks, z))
                {
                    res = CreatePoint(x, y + jumpBlocks, z);
                    y += jumpBlocks;
                }
            }

            if (res == null) return null;
            while (y > 0)
                if (GameController.World.CanMoveTo(x, y - 1, z))
                    y--;
                else
                    break;
            res = CreatePoint(x, y, z);
            return res;
        }



        #endregion

        private static Location CreatePoint(int x, int y, int z)
        {
            var hash = Location.CalcHash(x, y, z);
            if (PointSet.TryGetValue(hash, out Location result))
                return result;
            result = new Location(x, y, z);
            return PointSet[hash] = result;
        }

        public static Path Shovel(Location start, Location end, bool includeLast = true, bool digBlocs = true, float maxDistance = 64f)
        {
            try
            {
                var nodes = new FastPriorityQueue<Location>((int)maxDistance * (int)maxDistance * 255);
                var cost = new Hashtable();
                var succeed = false;

                nodes.Enqueue(start, 0);
                start.Prev = null;
                cost.Add(start, 0f);

                var endCost = GetBlockWeight(end.X, end.Y, end.Z) + GetBlockWeight(end.X, end.Y + 1, end.Z);
                if (endCost < 0)
                    return null;

                while (nodes.Count > 0)
                {
                    var current = nodes.Dequeue();
                    if (current.Equals(end))
                    {
                        end = current;
                        succeed = true;
                        break;
                    }
                    foreach (var node in AdvancedNeighbours(current))
                    {
                        var next = node.Item1;
                        if (next.Equals(end) && !includeLast)
                        {
                            end = current;
                            succeed = true;
                            break;
                        }

                        if(node.Item2 < 0) continue;
                        if(node.Item1.DistanceTo(start) > maxDistance) continue;
                        if(!digBlocs && node.Item2 > 0) continue;

                        var nodeCost = (float)cost[current] + 1f + node.Item2;
                        if (!cost.ContainsKey(next))
                        {
                            cost.Add(next, nodeCost);
                            nodes.Enqueue(next, nodeCost + next.DistanceTo(end));
                            next.Prev = current;
                        }
                        else if ((float)cost[next] > nodeCost)
                        {
                            cost[next] = nodeCost;
                            nodes.UpdatePriority(next, nodeCost + next.DistanceTo(end));
                            next.Prev = current;
                        }
                    }
                    if(succeed)
                        break;
                }
                if (!succeed)
                {
                    PointSet.Clear();
                    WeightSet.Clear();
                    GC.Collect();
                    return null;
                }

                var pathfrom = new List<Location>();

                while (end.Prev != null)
                {
                    pathfrom.Add(end);
                    end = end.Prev;
                }
                PointSet.Clear();
                pathfrom.Reverse();
                return new Path(pathfrom);
            }
            catch (Exception e)
            {
                Program.GetLogger().Error($"Cant create path! Error : {e}");
                PointSet.Clear();
                return null;
            }
        }

        public static IEnumerable<Tuple<Location, float>> AdvancedNeighbours(Location root)
        {
            /*var weightedPoints = new Dictionary<Location, float>()
            {
                {
                    CreatePoint(root.X + 1, root.Y, root.Z),
                    GetBlockWeight(root.X + 1, root.Y, root.Z) + GetBlockWeight(root.X + 1, root.Y + 1, root.Z)
                },
                {
                    CreatePoint(root.X - 1, root.Y, root.Z),
                    GetBlockWeight(root.X - 1, root.Y, root.Z) + GetBlockWeight(root.X - 1, root.Y + 1, root.Z)
                },
                {
                    CreatePoint(root.X, root.Y, root.Z + 1),
                    GetBlockWeight(root.X, root.Y, root.Z + 1) + GetBlockWeight(root.X, root.Y + 1, root.Z + 1)
                },
                {
                    CreatePoint(root.X, root.Y, root.Z - 1),
                    GetBlockWeight(root.X, root.Y, root.Z - 1) + GetBlockWeight(root.X, root.Y + 1, root.Z - 1)
                },

                {
                    CreatePoint(root.X, root.Y + 1, root.Z),
                    GetBlockWeight(root.X, root.Y + 2, root.Z)
                },
            };
            if (root.Y > 0)
            {
                weightedPoints.Add(CreatePoint(root.X, root.Y - 1, root.Z), GetBlockWeight(root.X, root.Y - 1, root.Z));
            }*/

            var weightedPoints = new List<Tuple<Location, float>>
            {
                Tuple.Create(CreatePoint(root.X + 1, root.Y, root.Z),
                    GetBlockWeight(root.X + 1, root.Y, root.Z) + GetBlockWeight(root.X + 1, root.Y + 1, root.Z)),
                Tuple.Create(CreatePoint(root.X - 1, root.Y, root.Z),
                    GetBlockWeight(root.X - 1, root.Y, root.Z) + GetBlockWeight(root.X - 1, root.Y + 1, root.Z)),
                Tuple.Create(CreatePoint(root.X, root.Y, root.Z + 1),
                    GetBlockWeight(root.X, root.Y, root.Z + 1) + GetBlockWeight(root.X, root.Y + 1, root.Z + 1)),
                Tuple.Create(CreatePoint(root.X, root.Y, root.Z - 1),
                    GetBlockWeight(root.X, root.Y, root.Z - 1) + GetBlockWeight(root.X, root.Y + 1, root.Z - 1)),
                Tuple.Create(CreatePoint(root.X, root.Y + 1, root.Z), GetBlockWeight(root.X, root.Y + 2, root.Z))
            };
            if (root.Y > 0)
            {
                weightedPoints.Add(Tuple.Create(CreatePoint(root.X, root.Y - 1, root.Z), GetBlockWeight(root.X, root.Y - 1, root.Z)));
            }
            return weightedPoints;
        }

        private static float GetBlockWeight(int x, int y, int z)
        {
            var hash = Location.CalcHash(x, y, z);
            if (WeightSet.TryGetValue(hash, out float weight))
                return weight;

            var block = Block.GetBlock(GameController.World.GetBlock(x, y, z));
            if (block.Transparent)
                return WeightSet[hash] = 0;
            if (block.Hardness < 0)
                return WeightSet[hash] = - 1e9f;
            return WeightSet[hash] = block.Hardness * 5;
        }

    }
}
