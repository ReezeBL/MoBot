﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftEmuPTS.GameData
{
    class Chunk
    {
        public int x, z, numBlocks, aBlocks;
        public bool lighting, groundup = false;
        public List<Section> sections;

        public short pbitmap, abitmap;
        private byte[] blocks;

        public Chunk(int X, int Z, short Pbitmap, short Abitmap, bool inLighting, bool Groundup)
        {
            // Create chunk sections.
            groundup = Groundup;
            lighting = inLighting;
            pbitmap = Pbitmap;
            abitmap = Abitmap;
            x = X;
            z = Z;
            sections = new List<Section>();

            numBlocks = 0;
            aBlocks = 0;

            for (int i = 0; i < 16; i++)
            {
                if (Convert.ToBoolean(Pbitmap & (1 << i)))
                {
                    numBlocks++; // "Sections"
                    sections.Add(new Section((byte)i));
                }
            }

            for (int i = 0; i < 16; i++)
            {
                if (Convert.ToBoolean(Abitmap & (1 << i)))
                {
                    aBlocks++;
                }
            }        
            numBlocks = numBlocks * 4096;           
        }
        private void populate()
        {
           int offset = 0, current = 0;

            for (int i = 0; i < 16; i++)
            {
                if (Convert.ToBoolean(pbitmap & (1 << i)))
                {

                    byte[] temp = new byte[4096];

                    Array.Copy(blocks, offset, temp, 0, 4096);
                    Section mySection = sections[current];

                    mySection.blocks = temp;
                    offset += 4096;
                    current += 1;
                }
            }
        }
        public int getBlockId(int Bx, int By, int Bz)
        {
            Section thisSection = GetSectionByNumber(By);
            return thisSection.getBlock(getXinSection(Bx), GetPositionInSection(By), getZinSection(Bz)).ID;
        }
        public Block getBlock(int Bx, int By, int Bz)
        {
            Section thisSection = GetSectionByNumber(By);
            return thisSection.getBlock(getXinSection(Bx), GetPositionInSection(By), getZinSection(Bz));
        }

        public void updateBlock(int Bx, int By, int Bz, int id)
        {           
            Section thisSection = GetSectionByNumber(By);
            thisSection.setBlock(getXinSection(Bx), GetPositionInSection(By), getZinSection(Bz), id);
        }
        public byte[] getData(byte[] deCompressed)
        {           
            blocks = new byte[numBlocks];
            byte[] temp;
            int removeable = numBlocks + aBlocks * 2048;

            if (lighting)
                removeable += (numBlocks / 2);
            if (groundup)
                removeable += 256;
            Array.Copy(deCompressed, 0, blocks, 0, numBlocks);
            temp = new byte[deCompressed.Length - (numBlocks + removeable)];
            Array.Copy(deCompressed, (numBlocks + removeable), temp, 0, temp.Length);
            populate();
            return temp;
        }

        #region Helping Methods
        private Section GetSectionByNumber(int blockY)
        {
            Section thisSection = null;

            foreach (Section y in sections)
            {
                if (y.y == blockY / 16)
                {
                    thisSection = y;
                    break;
                }
            }

            if (thisSection == null)
            { // Add a new section, if it doesn't exist yet.
                thisSection = new Section((byte)(blockY / 16));
                sections.Add(thisSection);
            }

            return thisSection;
        }
        private int getXinSection(int BlockX)
        {
            return BlockX - (x * 16);
        }
        private int GetPositionInSection(int blockY)
        {
            return blockY & (16 - 1); // Credits: SirCmpwn Craft.net
        }
        private int getZinSection(int BlockZ)
        {
            return BlockZ - (z * 16);
        }
        #endregion
    }
}
