using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftEmuPTS.GameData
{
    class World
    {
        private List<Chunk> chunks = new List<Chunk>();
        private object chunkLocker = new object();                
        public void AddChunk(Chunk c)
        {
            lock (chunkLocker)
            {
                chunks.Add(c);
            }
        }
        public void RemoveChunk(int x, int z)
        {
            lock (chunkLocker)
            {
                Chunk c = GetChunk(x, z);
                if (c != null)
                    chunks.Remove(c);
            }
        }
        public Block GetBlock(int x, int y, int z)
        {
            Chunk chunk = this.GetChunk(x, z);
            if (chunk != null)
            {
                return chunk.getBlock(x, y, z);
            }
            else
                return null;
        }
        public void UpdateBlock(int x, int y, int z, int ID)
        {
            Chunk chunk = this.GetChunk(x, z);
            lock (chunkLocker)
            {
                if (chunk != null)
                {
                    chunk.updateBlock(x, y, z, ID);
                }
            }
        }
        public Chunk GetChunk(int x, int z)
        {
            lock (chunkLocker)
            {
                int chunkX = (int)Math.Floor(decimal.Divide(x, 16));
                int chunkZ = (int)Math.Floor(decimal.Divide(z, 16));

                Chunk thisChunk = null;

                foreach (Chunk b in chunks)
                {
                    if (b.x == chunkX & b.z == chunkZ)
                    {
                        thisChunk = b;
                        break;
                    }
                }
                return thisChunk;
            }
        }   
    }
}
