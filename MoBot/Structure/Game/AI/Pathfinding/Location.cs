﻿using System;
using System.Runtime.Serialization;
using AForge.Math;
using MoBot.Structure.Game.World;
using Priority_Queue;

namespace MoBot.Structure.Game.AI.Pathfinding
{
    public class Location : FastPriorityQueueNode
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public Location(Entity entity)
        {
            X = (int) entity.X;
            Y = (int) entity.Y;
            Z = (int) entity.Z;
        }

        public Location() { }

        public float DistanceTo(Location other)
        {
            return (float)Math.Sqrt((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y) + (Z - other.Z) * (Z - other.Z));
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
            var point = obj as Location;
            if (point == null) return false;
            var other = point;
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override string ToString()
        {
            return $"({X} | {Y} | {Z})";
        }
        public Location Prev;

        public Location(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Location(Vector3 vector)
        {
            return new Location(MathHelper.floor_float(vector.X), (int) vector.Y, MathHelper.floor_float(vector.Z));
        }

        public static implicit operator Vector3(Location point)
        {
            return new Vector3(point.X - 0.5f * Math.Sign(point.X), point.Y, point.Z - 0.5f * Math.Sign(point.Z));
        }
    }
}
