using System;
using System.Runtime.Serialization;
using MoBot.Structure.Game.World;

namespace MoBot.Structure.Game.AI.Pathfinding
{

    class PathPoint : Priority_Queue.FastPriorityQueueNode
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
    }
}
