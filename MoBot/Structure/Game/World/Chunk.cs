using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MoBot.Structure.Game.World
{
    public class Chunk
    {
        public readonly int X, Z;
        private readonly int _numBlocks;
        private readonly int _aBlocks;

        public bool Lighting, Groundup;
        public List<Section> Sections;

        public short Pbitmap, Abitmap;

        private readonly Section[] _sections = new Section[16];

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

            _numBlocks = 0;
            _aBlocks = 0;

            for (int i = 0; i < 16; i++)
            {
                if (!Convert.ToBoolean(pbitmap & (1 << i))) continue;
                _numBlocks++; // "Sections"
            }

            for (int i = 0; i < 16; i++)
            {
                if (Convert.ToBoolean(abitmap & (1 << i)))
                {
                    _aBlocks++;
                }
            }        
            _numBlocks = _numBlocks * 4096;           
        }
        private void Populate(byte[] blocks)
        {
            int offset = 0;
            for (int i = 0; i < 16; i++)
            {
                if (!Convert.ToBoolean(Pbitmap & (1 << i))) continue;

                _sections[i] = new Section(blocks, offset, X, i, Z);
                offset += 4096;
            }
        }
        public int GetBlock(int bx, int y, int bz)
        {
            return _sections[y >> 4]?.GetBlock(bx, GetPositionInSection(y & 15), bz) ?? -1;
        }

        public void SetBlock(int bx, int y, int bz, int id)
        {
            _sections[y >> 4]?.SetBlock(bx, GetPositionInSection(y & 15), bz, id);
        }

        public byte[] GetData(byte[] deCompressed)
        {           
            byte[] blocks = new byte[_numBlocks];
            int removeable = _numBlocks + _aBlocks * 2048;

            if (Lighting)
                removeable += (_numBlocks / 2);
            if (Groundup)
                removeable += 256;

            Array.Copy(deCompressed, 0, blocks, 0, _numBlocks);
            var temp = new byte[deCompressed.Length - (_numBlocks + removeable)];
            Array.Copy(deCompressed, (_numBlocks + removeable), temp, 0, temp.Length);
            Populate(blocks);
            return temp;
        }

        #region Helping Methods
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
