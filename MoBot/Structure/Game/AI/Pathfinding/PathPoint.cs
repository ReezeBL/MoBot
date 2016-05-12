using MinecraftEmuPTS.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using MoBot.Structure.Game.AI.Modules;

namespace MoBot.Structure.Game.AI.Pathfinding
{
    
    class PathPoint : Priority_Queue.FastPriorityQueueNode
    {
        [DataMember]
        public int x, y, z;
        public PathPoint(Block block)
        {
            x = block.x;
            y = block.y;
            z = block.z;
        }

        public PathPoint() { }

        public double DistanceTo(PathPoint other)
        {
            return Math.Sqrt((x - other.x) * (x - other.x) + (y - other.y) * (y - other.y) + (z - other.z) * (z - other.z));
        }
        public override int GetHashCode()
        {
            return y & 255 | (x & 32767) << 8 | (z & 32767) << 24 | (x < 0 ? int.MinValue : 0) | (z < 0 ? 32768 : 0);
        }

        public static int CalcHash(int x, int y, int z)
        {
            return y & 255 | (x & 32767) << 8 | (z & 32767) << 24 | (x < 0 ? int.MinValue : 0) | (z < 0 ? 32768 : 0);
        }

        public override bool Equals(object obj)
        {
            if(obj is PathPoint)
            {
                var other = obj as PathPoint;
                return x == other.x && y == other.y && z == other.z;
            }
            return false;
        }

        public override string ToString()
        {
            return $"({x} | {y} | {z})";
        }
        public PathPoint prev;

        public static implicit operator PathPoint(Point v)
        {
            return new PathPoint { x = v.x, y = v.y, z = v.z };
        }
    }
}
