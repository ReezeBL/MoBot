using System;
using System.Runtime.Serialization;
using AForge.Math;
using MoBot.Structure.Game.World;
using Priority_Queue;

namespace MoBot.Structure.Game.AI.Pathfinding
{
    public class PathPoint : FastPriorityQueueNode
    {
        [DataMember]
        public readonly int X;

        [DataMember]
        public readonly int Y;

        [DataMember]
        public readonly int Z;

        public PathPoint(Block block)
        {
            X = block.X;
            Y = block.Y;
            Z = block.Z;
        }

        public PathPoint(Entity entity)
        {
            X = (int) entity.X;
            Y = (int) entity.Y;
            Z = (int) entity.Z;
        }

        public PathPoint() { }

        public double DistanceTo(PathPoint other)
        {
            return Math.Sqrt((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y) + (Z - other.Z) * (Z - other.Z));
        }
        public override int GetHashCode()
        {
            return Y & 255 | (X & 32767) << 8 | (Z & 32767) << 24 | (X < 0 ? int.MinValue : 0) | (Z < 0 ? 32768 : 0);
        }

        public static int CalcHash(int x, int y, int z)
        {
            return y & 255 | (x & 32767) << 8 | (z & 32767) << 24 | (x < 0 ? int.MinValue : 0) | (z < 0 ? 32768 : 0);
        }

        public override bool Equals(object obj)
        {
            var point = obj as PathPoint;
            if (point == null) return false;
            var other = point;
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override string ToString()
        {
            return $"({X} | {Y} | {Z})";
        }
        public PathPoint Prev;

        public PathPoint(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator PathPoint(Vector3 vector)
        {
            return new PathPoint((int) vector.X, (int) vector.Y, (int) vector.Z);
        }

        public static implicit operator Vector3(PathPoint point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }
    }
}
