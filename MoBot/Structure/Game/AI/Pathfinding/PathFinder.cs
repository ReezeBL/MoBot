﻿using MinecraftEmuPTS.GameData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MoBot.Structure.Game.AI.Pathfinding
{
    class PathFinder
    {
        public World World;
        private readonly Hashtable _pointSet = new Hashtable();

        public Path FlatPath(LivingEntity entity, PathPoint end)
        {
            return
                FlatPath(
                    new PathPoint(MathHelper.floor_double(entity.X), MathHelper.floor_double(entity.Y),
                        MathHelper.floor_double(entity.Z)), end);
        }

        public Path FlatPath(PathPoint start, PathPoint end)
        {
            try
            {
                var frontier = new Priority_Queue.FastPriorityQueue<PathPoint>(300);
                Hashtable startCost = new Hashtable();
                frontier.Enqueue(start, 0);
                start.Prev = null;
                startCost.Add(start, 0);
                while (frontier.Count > 0)
                {
                    var current = frontier.Dequeue();
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
                            frontier.Enqueue(next, cost + next.DistanceTo(end));
                            next.Prev = current;
                        }
                        else if ((int) startCost[next] > cost)
                        {
                            startCost[next] = cost;
                            frontier.UpdatePriority(next, cost + next.DistanceTo(end));
                            next.Prev = current;
                        }
                    }
                }
                List<PathPoint> pathfrom = new List<PathPoint>();
                while (end != null)
                {
                    pathfrom.Add(end);
                    end = end.Prev;
                }
                pathfrom.Reverse();
                foreach (var pp in pathfrom)
                {
                    Console.WriteLine($"Path point: {pp}");
                }
                return new Path(pathfrom);
            }
            catch (Exception e)
            {
                Program.GetLogger().Error($"Cant create path! Error : {e}");
                return null;
            }
        }

        private IEnumerable<PathPoint> GetNeighbours(PathPoint point)
        {
            List<PathPoint> candidats = new List<PathPoint>();
            int canJump = CanMoveTo(point.X, point.Y + 1, point.Z) ? 1 : 0;
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
            if (CanMoveTo(x, y, z))
                res = CreatePoint(x, y, z);
            else if (jumpBlocks > 0)
            {
                if (CanMoveTo(x, y + jumpBlocks, z))
                {
                    res = CreatePoint(x, y + jumpBlocks, z);
                    y += jumpBlocks;
                }
            }

            if (res == null) return null;
            while (y > 0)
                if (CanMoveTo(x, y - 1, z))
                    y--;
                else
                    break;
            res = CreatePoint(x, y, z);
            return res;
        }

        private bool CanMoveTo(int x, int y, int z)
        {
            x = x < 0 ? x - 1 : x;
            z = z < 0 ? z - 1 : z;

            Block floor = World.GetBlock(x, y, z);
            Block upper = World.GetBlock(x, y + 1, z);

            return IsBlockFree(floor) && IsBlockFree(upper);
        }

        private static bool IsBlockFree(Block b)
        {
            return b == null || GameBlock.getBlock(b.ID).transparent;
        }

        private PathPoint CreatePoint(int x, int y, int z)
        {
            var p = new PathPoint(x, y, z);
            if (_pointSet.ContainsKey(p))
                return _pointSet[p] as PathPoint;
            else
                _pointSet.Add(p, p);
            return p;
        }
    }
}
