using System.Collections.Generic;

namespace MoBot.Structure.Game.World
{
    public class Block
    {
        public int Id;
        public string Name;
        public int X;
        public int Y;
        public int Z;
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
    }
}
