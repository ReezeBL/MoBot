using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace MoBot.Structure.Game.World
{
    public class GameWorld
    {
        private readonly List<Chunk> _chunks = new List<Chunk>();
        private readonly Dictionary<int, Dictionary<int, Chunk>> _chunkDictionary = new Dictionary<int, Dictionary<int, Chunk>>();
        private readonly object _monitor = new object();
        private readonly object _validationLocker = new object();
        private int _validation;

        public Chunk this[int x, int y]
        {
            get
            {
                if (_chunkDictionary.ContainsKey(x))
                {
                    var xChunk = _chunkDictionary[x];
                    if (xChunk.ContainsKey(y))
                        return xChunk[y];
                }
                return null;
            }
            set
            {
                if (_chunkDictionary.ContainsKey(x))
                {
                    var xChunk = _chunkDictionary[x];
                    if (xChunk.ContainsKey(y))
                        xChunk[y] = value;
                    else
                        xChunk.Add(y, value);
                }
                else
                {
                    var xChunk = new Dictionary<int, Chunk> {{y, value}};
                    _chunkDictionary.Add(x, xChunk);
                }
            }
        }


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
            this[c.X, c.Z] = c;
        }

        public void RemoveChunk(int x, int z)
        {
            this[x, z] = null;
        }

        public Block GetBlock(int x, int y, int z)
        {
            return this[x >> 4, z >> 4]?.GetBlock(x & 15, y, z & 15);
        }

        public void UpdateBlock(int x, int y, int z, int id)
        {
            var block = GetBlock(x, y, z);
            if (block != null)
                block.Id = id;
        }

        public Block SearchBlock(HashSet<int> ids)
        {
            int x = MathHelper.floor_float(GameController.Player.X);
            int z = MathHelper.floor_float(GameController.Player.Z);
            int y = (int) GameController.Player.Y;

            return SearchBlock(x,y,z, ids.Contains);
        }

        public Block SearchBlock(int x, int y, int z, Func<int, bool> idPredicate )
        {
            Block result = null;
            int maxDistance = Settings.ScanRange;

            for (int d = 1; d < maxDistance; d++)
            {
                for (int i = 0; i <= d; i++)
                {
                    for (int j = 0; j <= d - i; j++)
                    {
                        int x1 = x + i;
                        int z1 = z + j;
                        int y1 = y + (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out result))
                            return result;

                        x1 = x - i;
                        z1 = z + j;
                        y1 = y + (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out result))
                            return result;

                        x1 = x + i;
                        z1 = z - j;
                        y1 = y + (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out result))
                            return result;

                        x1 = x + i;
                        z1 = z + j;
                        y1 = y - (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out result))
                            return result;

                        x1 = x - i;
                        z1 = z - j;
                        y1 = y + (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out result))
                            return result;

                        x1 = x - i;
                        z1 = z + j;
                        y1 = y - (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out result))
                            return result;

                        x1 = x + i;
                        z1 = z - j;
                        y1 = y - (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out result))
                            return result;

                        x1 = x - i;
                        z1 = z - j;
                        y1 = y - (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out result))
                            return result;
                    }
                }
            }

            return result;
        }

        public List<Block> SearchBlocks(int x, int y, int z, Func<int, bool> idPredicate)
        {
            HashSet<Block> result = new HashSet<Block>();
            Block tmp;

            int maxDistance = Settings.ScanRange;

            if (Check(x, y, z, idPredicate, out tmp))
                result.Add(tmp);

            for (int d = 1; d < maxDistance; d++)
            {
                for (int i = 0; i <= d; i++)
                {
                    for (int j = 0; j <= d - i; j++)
                    {
                        int x1 = x + i;
                        int z1 = z + j;
                        int y1 = y + (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out tmp))
                            result.Add(tmp);

                        x1 = x - i;
                        z1 = z + j;
                        y1 = y + (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out tmp))
                            result.Add(tmp);

                        x1 = x + i;
                        z1 = z - j;
                        y1 = y + (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out tmp))
                            result.Add(tmp);

                        x1 = x + i;
                        z1 = z + j;
                        y1 = y - (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out tmp))
                            result.Add(tmp);

                        x1 = x - i;
                        z1 = z - j;
                        y1 = y + (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out tmp))
                            result.Add(tmp);

                        x1 = x - i;
                        z1 = z + j;
                        y1 = y - (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out tmp))
                            result.Add(tmp);

                        x1 = x + i;
                        z1 = z - j;
                        y1 = y - (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out tmp))
                            result.Add(tmp);

                        x1 = x - i;
                        z1 = z - j;
                        y1 = y - (d - i - j);

                        if (Check(x1, y1, z1, idPredicate, out tmp))
                            result.Add(tmp);
                    }
                }
            }

            return result.ToList();
        }

        public List<Block> SearchBlocks(HashSet<int> ids)
        {
            int x = MathHelper.floor_float(GameController.Player.X);
            int z = MathHelper.floor_float(GameController.Player.Z);
            int y = (int)GameController.Player.Y;

            return SearchBlocks(x,y,z, ids.Contains);
        }

        private bool Check(int x, int y, int z, Func<int, bool> idPredicate , out Block result)
        {
            result = GetBlock(x, y, z);
            return result != null && idPredicate(result.Id);
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
