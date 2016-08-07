using System;

namespace MoBot.Structure.Game.World
{
    public class Section
    {
        public byte[] Blocks;
        public byte Y;

        public Section(byte y)
        {
            Y = y;
            Blocks = new byte[4096];
        }

        public void SetBlock(int x, int y, int z, int id)
        {
            int index = x + (z * 16) + (y * 256);
            Blocks[index] = (byte)id;
        }

        public Block GetBlock(int x, int y, int z,int Cx, int Cz)
        {
            int index = x + (z * 16) + (y * 16 * 16);
            Block thisBlock = new Block(Blocks[index], Cx*16 + x, y + Y*16, Cz * 16 + z);

            return thisBlock;
        }
    }
}
