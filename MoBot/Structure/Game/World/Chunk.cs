using System;
using System.Collections.Generic;
using System.Linq;

namespace MoBot.Structure.Game.World
{
    public class Chunk
    {
        public int X, Z, NumBlocks, ABlocks;
        public bool Lighting, Groundup;
        public List<Section> Sections;

        public short Pbitmap, Abitmap;
        private byte[] _blocks;

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

            NumBlocks = 0;
            ABlocks = 0;

            for (int i = 0; i < 16; i++)
            {
                if (!Convert.ToBoolean(pbitmap & (1 << i))) continue;
                NumBlocks++; // "Sections"
                Sections.Add(new Section((byte)i));
            }

            for (int i = 0; i < 16; i++)
            {
                if (Convert.ToBoolean(abitmap & (1 << i)))
                {
                    ABlocks++;
                }
            }        
            NumBlocks = NumBlocks * 4096;           
        }
        private void Populate()
        {
           int offset = 0, current = 0;

            for (int i = 0; i < 16; i++)
            {
                if (!Convert.ToBoolean(Pbitmap & (1 << i))) continue;
                byte[] temp = new byte[4096];

                Array.Copy(_blocks, offset, temp, 0, 4096);
                Section mySection = Sections[current];

                mySection.Blocks = temp;
                offset += 4096;
                current += 1;
            }
        }
        public int GetBlockId(int bx, int by, int bz)
        {
            return GetBlock(bx, by, bz).Id;
        }
        public Block GetBlock(int bx, int by, int bz)
        {
            Section thisSection = GetSectionByNumber(by);
            return thisSection.GetBlock(GetXinSection(bx), GetPositionInSection(by), GetZinSection(bz), X, Z);
        }

        public void UpdateBlock(int bx, int by, int bz, int id)
        {           
            Section thisSection = GetSectionByNumber(by);
            thisSection.SetBlock(GetXinSection(bx), GetPositionInSection(by), GetZinSection(bz), id);
        }
        public byte[] GetData(byte[] deCompressed)
        {           
            _blocks = new byte[NumBlocks];
            int removeable = NumBlocks + ABlocks * 2048;

            if (Lighting)
                removeable += (NumBlocks / 2);
            if (Groundup)
                removeable += 256;
            Array.Copy(deCompressed, 0, _blocks, 0, NumBlocks);
            var temp = new byte[deCompressed.Length - (NumBlocks + removeable)];
            Array.Copy(deCompressed, (NumBlocks + removeable), temp, 0, temp.Length);
            Populate();
            return temp;
        }

        #region Helping Methods
        private Section GetSectionByNumber(int blockY)
        {
            Section thisSection = Sections.FirstOrDefault(y => y.Y == blockY/16);
            if (thisSection != null) return thisSection;
            thisSection = new Section((byte)(blockY / 16));
            Sections.Add(thisSection);

            return thisSection;
        }
        private int GetXinSection(int blockX)
        {
            return blockX - (X * 16);
        }
        private int GetPositionInSection(int blockY)
        {
            return blockY & (16 - 1); 
        }
        private int GetZinSection(int blockZ)
        {
            return blockZ - (Z * 16);
        }
        #endregion
    }
}
