using System;
using System.Collections.Generic;
using System.Linq;

namespace MoBot.Structure.Game.World
{
    public class GameWorld
    {
        private readonly List<Chunk> _chunks = new List<Chunk>();
        private readonly object _chunkLocker = new object();
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
            lock (_chunkLocker)
            {
                _chunks.Add(c);
            }
        }

        public void RemoveChunk(int x, int z)
        {
            lock (_chunkLocker)
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
            lock (_chunkLocker)
            {
                chunk?.UpdateBlock(x, y, z, id);
            }
        }

        public Chunk GetChunk(int x, int z)
        {
            lock (_chunkLocker)
            {
                int chunkX = (int) Math.Floor(decimal.Divide(x, 16));
                int chunkZ = (int) Math.Floor(decimal.Divide(z, 16));
                return _chunks.FirstOrDefault(b => b.X == chunkX & b.Z == chunkZ);
            }
        }

        public void Clear()
        {
            lock (_chunkLocker)
            {
                _chunks.Clear();
            }
        }

        public bool CanMoveTo(int x, int y, int z)
        {
            x = x < 0 ? x - 1 : x;
            z = z < 0 ? z - 1 : z;

            Block floor = GetBlock(x, y, z);
            Block upper = GetBlock(x, y + 1, z);

            return IsBlockFree(floor) && IsBlockFree(upper);
        }

        private static bool IsBlockFree(Block b)
        {
            return b == null || GameBlock.GetBlock(b.Id).Transparent;
        }
    }
}
