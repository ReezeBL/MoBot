﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MoBot.Structure.Game.World
{
    public class GameWorld
    {
        private readonly List<Chunk> _chunks = new List<Chunk>();
        private readonly object _monitor = new object();
        private readonly object _validationLocker = new object();
        private int _validation;

        public int WorldValidation
        {
            get
            {
                lock (_validationLocker)
                {
                    return _validation;
                }
            }
            private set
            {
                lock (_validationLocker)
                {
                    _validation = value;
                }
            }
        }


        public void Invalidate()
        {
            WorldValidation++;
        }

        public void AddChunk(Chunk c)
        {
            lock (_monitor)
            {
                _chunks.Add(c);
            }
        }

        public void RemoveChunk(int x, int z)
        {
            lock (_monitor)
            {
                Chunk c = GetChunk(x, z);
                if (c != null)
                    _chunks.Remove(c);
            }
        }

        public Block GetBlock(int x, int y, int z)
        {
            Chunk chunk = GetChunk(x, z);
            return chunk?.GetBlock(x, y, z);
        }

        public void UpdateBlock(int x, int y, int z, int id)
        {
            Chunk chunk = GetChunk(x, z);
            lock (_monitor)
            {
                chunk?.UpdateBlock(x, y, z, id);
            }
        }

        public List<Block> GetBlocks(HashSet<int> ids)
        {
            lock (_monitor)
            {
                List<Block> result = new List<Block>();
                foreach (var chunk in _chunks)
                {
                    foreach (var section in chunk.Sections)
                    {
                        for(int x=0;x<16;x++)
                            for(int y = 0;y<16;y++)
                                for (int z = 0; z < 16; z++)
                                {
                                    var id = section.Blocks[x + y*16 + z*256];
                                    if(ids.Contains(id))
                                        result.Add(new Block(id, x + chunk.X*16, y + section.Y*16, z + chunk.Z * 16));
                                }
                    }
                }
                return result;
            }
        }

        public Block SearchBlock(HashSet<int> ids)
        {
            Block result;
            int X = MathHelper.floor_float(GameController.Player.X);
            int Z = MathHelper.floor_float(GameController.Player.Z);
            int Y = (int) GameController.Player.Y;

            int maxDistance = Settings.ScanRange;

            if (Check(X, Y, Z, ids, out result))
                return result;

            for (int d = 1; d < maxDistance; d++)
            {
                for (int i = 0; i <= d; i++)
                {
                    for (int j = 0; j <= d - i; j++)
                    {
                        int x1 = X + i;
                        int z1 = Z + j;
                        int y1 = Y + (d - i - j);

                        if (Check(x1, y1, z1, ids, out result))
                            return result;

                        x1 = X - i;
                        z1 = Z + j;
                        y1 = Y + (d - i - j);

                        if (Check(x1, y1, z1, ids, out result))
                            return result;

                        x1 = X + i;
                        z1 = Z - j;
                        y1 = Y + (d - i - j);

                        if (Check(x1, y1, z1, ids, out result))
                            return result;

                        x1 = X + i;
                        z1 = Z + j;
                        y1 = Y - (d - i - j);

                        if (Check(x1, y1, z1, ids, out result))
                            return result;

                        x1 = X - i;
                        z1 = Z - j;
                        y1 = Y + (d - i - j);

                        if (Check(x1, y1, z1, ids, out result))
                            return result;

                        x1 = X - i;
                        z1 = Z + j;
                        y1 = Y - (d - i - j);

                        if (Check(x1, y1, z1, ids, out result))
                            return result;

                        x1 = X + i;
                        z1 = Z - j;
                        y1 = Y - (d - i - j);

                        if (Check(x1, y1, z1, ids, out result))
                            return result;

                        x1 = X - i;
                        z1 = Z - j;
                        y1 = Y - (d - i - j);

                        if (Check(x1, y1, z1, ids, out result))
                            return result;
                    }
                }
            }

            return result;
        }

        private bool Check(int x, int y, int z, HashSet<int> ids, out Block result)
        {
            result = GetBlock(x, y, z);
            return result != null && ids.Contains(result.Id);
        }

        public Chunk GetChunk(int x, int z)
        {
            lock (_monitor)
            {
                int chunkX = (int) Math.Floor(decimal.Divide(x, 16));
                int chunkZ = (int) Math.Floor(decimal.Divide(z, 16));
                return _chunks.FirstOrDefault(b => b.X == chunkX & b.Z == chunkZ);
            }
        }

        public void Clear()
        {
            lock (_monitor)
            {
                _chunks.Clear();
            }
        }

        public bool CanMoveTo(int x, int y, int z)
        {
            Block floor = GetBlock(x, y, z);
            Block upper = GetBlock(x, y + 1, z);

            return IsBlockFree(floor) && IsBlockFree(upper);
        }

        public bool IsBlockFree(int x, int y, int z)
        {
            return IsBlockFree(GetBlock(x, y, z));
        }

        private static bool IsBlockFree(Block b)
        {
            return b == null || GameBlock.GetBlock(b.Id).Transparent;
        }
    }
}
