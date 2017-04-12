using System.Collections.Generic;

namespace MoBot.Core.Game.World
{
    public class Section
    {
        //private readonly Block[,,] _data = new Block[16,16,16];
        private readonly short[,,] rawData = new short[16,16,16];

        public Section(IReadOnlyList<byte> blocks, int offset)
        {
            for(var y = 0;y<16;y++)
                for(var z=0;z<16;z++)
                    for (var x = 0; x < 16; x++)
                        rawData[x,y,z] = blocks[offset + x + z * 16 + y * 16 * 16];
        }

        public void ApplyMsb(byte[] msbArray, int offset)
        {
            for (var y = 0; y < 16; y++)
                for (var z = 0; z < 16; z++)
                    for (var x = 0; x < 16; x++)
                    {
                        var index = y << 8 | z << 4 | x;
                        var dev = index >> 1;
                        var b = (byte)((index & 1) == 0 ? msbArray[offset + dev] & 15 : msbArray[offset + dev] >> 4 & 15);
                        rawData[x, y, z] |= (short)(b << 8);
                    }
        
       
        }

        public int GetBlock(int x, int y, int z)
        {
            return rawData[x, y, z];
        }

        public void SetBlock(int x, int y, int z, int id)
        {
            rawData[x, y, z] = (short)id;
        }
    }
}
