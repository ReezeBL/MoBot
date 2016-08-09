using System;
using System.Collections.Generic;
using System.Text;
using MoBot.Structure.Game.AI.Pathfinding;

namespace MoBot.Structure.Game.World
{
    public class Block
    {
        public readonly int Id;
        public string Name;
        public readonly int X;
        public readonly int Y;
        public readonly int Z;
        public Block(int id, int x, int y, int z)
        {
            Id = id;
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator GameBlock(Block block)
        {
            return GameBlock.GetBlock(block.Id);
        }

        public override int GetHashCode()
        {
            return Y & 255 | (X & 32767) << 8 | (Z & 32767) << 24 | (X < 0 ? int.MinValue : 0) | (Z < 0 ? 32768 : 0);
        }

        public override string ToString()
        {
            var playerPos = (PathPoint)GameController.Player.Position;
            double distance = playerPos.DistanceTo(this);
            return
                $"{GameBlock.GetBlock(Id).Name} {{{X}|{Y}|{Z}}}\r\nDistance: {distance}";
        }
    }
}
