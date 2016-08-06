﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoBot.Structure.Game.World;
using Priority_Queue;

namespace MoBot.Structure.Game.AI.Pathfinding
{
    public static class PathFinder
    {
        #region OldPathFinder

        private static readonly Hashtable PointSet = new Hashtable();
        private static readonly FastPriorityQueue<PathPoint> Frontier = new FastPriorityQueue<PathPoint>(30000);

        public static Path StaticPath(LivingEntity entity, PathPoint end)
        {
            return
                StaticPath(
                    new PathPoint(MathHelper.floor_double(entity.X), MathHelper.floor_double(entity.Y),
                        MathHelper.floor_double(entity.Z)), end);
        }

        public static Path StaticPath(PathPoint start, PathPoint end)
        {
            try
            {
                Frontier.Clear();
                Hashtable startCost = new Hashtable();
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
                        int cost = (int) startCost[current] + 1;
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

        public static Path DynamicPath(Entity entity, PathPoint endPoint)
        {
            return new Path(DynamicPathGenerator(entity, endPoint));
        }

        public static Path DynamicPath(PathPoint start, PathPoint endPoint)
        {
            return new Path(DynamicPathGenerator(start, endPoint));
        }

        private static IEnumerator<PathPoint> DynamicPathGenerator(Entity entity, PathPoint endPoint)
        {
            return DynamicPathGenerator(new PathPoint((int) entity.X, (int) entity.Y, (int) entity.Z), endPoint);
        }

        private static IEnumerator<PathPoint> DynamicPathGenerator(PathPoint startPoint, PathPoint endPoint)
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

        private static IEnumerable<PathPoint> GetNeighbours(PathPoint point)
        {
            List<PathPoint> candidats = new List<PathPoint>();
            int canJump = GameController.World.CanMoveTo(point.X, point.Y + 1, point.Z) ? 1 : 0;
            candidats.Add(GetSafePoint(point.X + 1, point.Y, point.Z, canJump));
            candidats.Add(GetSafePoint(point.X - 1, point.Y, point.Z, canJump));
            candidats.Add(GetSafePoint(point.X, point.Y, point.Z + 1, canJump));
            candidats.Add(GetSafePoint(point.X, point.Y, point.Z - 1, canJump));
            var res = candidats.Where(x => x != null);
            return res;
        }

        private static PathPoint GetSafePoint(int x, int y, int z, int jumpBlocks = 0)
        {
            PathPoint res = null;
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

        private static PathPoint CreatePoint(int x, int y, int z)
        {
            var p = new PathPoint(x, y, z);
            if (PointSet.ContainsKey(p))
                return PointSet[p] as PathPoint;
            PointSet.Add(p, p);
            return p;
        }

        #endregion

        public static Path Shovel(PathPoint start, PathPoint end)
        {
            try
            {
                FastPriorityQueue<PathPoint> nodes = new FastPriorityQueue<PathPoint>(65536);
                Hashtable cost = new Hashtable();

                nodes.Enqueue(start, 0);
                start.Prev = null;
                cost.Add(start, 0);
                while (nodes.Count > 0)
                {
                    var current = nodes.Dequeue();
                    if (current.Equals(end))
                    {
                        end = current;
                        break;
                    }
                    foreach (var node in AdvancedNeighbours(current))
                    {
                        var next = node.Key;
                        float nodeCost = (float)cost[current] + 1f + node.Value;
                        if (!cost.ContainsKey(next))
                        {
                            cost.Add(next, cost);
                            nodes.Enqueue(next, nodeCost + next.DistanceTo(end));
                            next.Prev = current;
                        }
                        else if ((int)cost[next] > nodeCost)
                        {
                            cost[next] = cost;
                            nodes.UpdatePriority(next, nodeCost + next.DistanceTo(end));
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

        public static IEnumerable<KeyValuePair<PathPoint, float>> AdvancedNeighbours(PathPoint root)
        {
            Dictionary<PathPoint, float> weightedPoints = new Dictionary<PathPoint, float>()
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
                    GetBlockWeight(root.X, root.Y, root.Z - 1) + GetBlockWeight(root.X, root.Y - 1, root.Z + 1)
                },

                {
                    CreatePoint(root.X, root.Y + 1, root.Z),
                    GetBlockWeight(root.X, root.Y + 2, root.Z)
                },

                {
                    CreatePoint(root.X, root.Y - 1, root.Z),
                    GetBlockWeight(root.X, root.Y - 1, root.Z)
                },
            };

            return weightedPoints;
        }

        private static float GetBlockWeight(int x, int y, int z)
        {
            GameBlock block = GameBlock.GetBlock(GameController.World.GetBlock(x, y, z).Id);
            return block.Transparent ? 0 : block.Hardness;
        }

    }
}
