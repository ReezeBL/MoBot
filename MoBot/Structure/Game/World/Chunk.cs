using System;
using System.Collections.Generic;

namespace MoBot.Structure.Game.World
{
    public class Chunk
    {
        public readonly int X, Z;
        private readonly int numBlocks;
        private readonly int aBlocks;

        public bool Lighting, Groundup;
        public List<Section> Sections;

        public short Pbitmap, Abitmap;

        private readonly Section[] sections = new Section[16];

        public Chunk(int x, int z, short pbitmap, short abitmap, bool inLighting, bool groundup)
        {
            // Create chunk sections.
            Groundup = groundup;
            Lighting = inLighting;
            Pbitmap = pbitmap;
            Abitmap = abitmap;
            X = x;
            Z = z;
            Sections = new List<Section>();

            numBlocks = 0;
            aBlocks = 0;

            for (var i = 0; i < 16; i++)
            {
                if (!Convert.ToBoolean(pbitmap & (1 << i))) continue;
                numBlocks++; // "Sections"
            }

            for (var i = 0; i < 16; i++)
            {
                if (Convert.ToBoolean(abitmap & (1 << i)))
                {
                    aBlocks++;
                }
            }        
            numBlocks = numBlocks * 4096;           
        }
        private void Populate(byte[] blocks, byte[] msb)
        {
            var offset = 0;
            for (var i = 0; i < 16; i++)
            {
                if (!Convert.ToBoolean(Pbitmap & (1 << i))) continue;

                sections[i] = new Section(blocks, offset);
                offset += 4096;
            }
            offset = 0;
            for (var i = 0; i < 16; i++)
            {
                if(!Convert.ToBoolean(Abitmap & (1 << i) )) continue;
                sections[i].ApplyMsb(msb, offset);
                offset += 2048;
            }
        }
        public int GetBlock(int bx, int y, int bz)
        {
            return sections[y >> 4]?.GetBlock(bx, GetPositionInSection(y & 15), bz) ?? -1;
        }

        public void SetBlock(int bx, int y, int bz, int id)
        {
            sections[y >> 4]?.SetBlock(bx, GetPositionInSection(y & 15), bz, id);
        }

        public byte[] GetData(byte[] deCompressed)
        {
            var lsb = new byte[numBlocks];
            var msb = new byte[aBlocks * 2048];

            var offset = 2 * numBlocks;

            if (Lighting)
                offset += numBlocks / 2;

            Array.Copy(deCompressed, 0, lsb, 0, lsb.Length);
            Array.Copy(deCompressed, offset, msb, 0, msb.Length);

            if (Groundup)
                offset += 256;
            offset += msb.Length;

            var temp = new byte[deCompressed.Length - offset];
            Array.Copy(deCompressed, offset, temp, 0, temp.Length);
            Populate(lsb, msb);
            return temp;
        }

        #region Helping Methods
        private static int GetPositionInSection(int blockY)
        {
            return blockY & (16 - 1); 
        }

        #endregion
    }
}
