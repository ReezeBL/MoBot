using System;
using System.Collections.Generic;
using System.Linq;

namespace MoBot.Structure.Game.World
{
    class GameWorld
    {
        private readonly List<Chunk> _chunks = new List<Chunk>();
        private readonly object _chunkLocker = new object();
        public int WorldValidation { get; private set; }

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
    }
}
