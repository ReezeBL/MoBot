using System;

namespace MoBot.Structure.Game.World
{
    public class Section
    {
        private readonly Block[,,] _data = new Block[16,16,16];
        //==private readonly int[,,] _rawData = new int[16,16,16];

        private readonly int X, Y, Z;

        public Section(byte[] blocks, int offset, int cx, int cy, int cz)
        {
            X = cx;
            Y = cy;
            Z = cz;

            for(int y = 0;y<16;y++)
                for(int z=0;z<16;z++)
                    for (int x = 0; x < 16; x++)
                        _data[x,y,z] = new Block(blocks[offset + x + (z * 16) + (y * 16 * 16)]);
        }

        public Block GetBlock(int x, int y, int z)
        {
            return _data[x, y, z];
        }
    }
}
