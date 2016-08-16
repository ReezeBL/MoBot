using System;

namespace MoBot.Structure.Game.World
{
    public class Section
    {
        //private readonly Block[,,] _data = new Block[16,16,16];
        private readonly short[,,] _rawData = new short[16,16,16];
        private readonly int X, Y, Z;

        public Section(byte[] blocks, int offset, int cx, int cy, int cz)
        {
            X = cx;
            Y = cy;
            Z = cz;

            for(int y = 0;y<16;y++)
                for(int z=0;z<16;z++)
                    for (int x = 0; x < 16; x++)
                        _rawData[x,y,z] = blocks[offset + x + (z * 16) + (y * 16 * 16)];
        }

        public void ApplyMsb(byte[] msbArray, int offset)
        {
            for (int y = 0; y < 16; y++)
                for (int z = 0; z < 16; z++)
                    for (int x = 0; x < 16; x++)
                    {
                        int index = y << 8 | z << 4 | x;
                        int dev = index >> 1;
                        byte b = (byte)(((index & 1) == 0) ? msbArray[offset + dev] & 15 : msbArray[offset + dev] >> 4 & 15);
                        _rawData[x, y, z] |= (short)(b << 8);
                    }
        
       
        }

        public int GetBlock(int x, int y, int z)
        {
            return _rawData[x, y, z];
        }

        public void SetBlock(int x, int y, int z, int id)
        {
            _rawData[x, y, z] = (short)id;
        }
    }
}
