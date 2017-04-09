using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using MoBot.Structure.Game.AI.Pathfinding;

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
            var toRemove = GameController.TileEntities.Where(tileEntity => tileEntity.Location.X >> 4 == x && tileEntity.Location.Z >> 4 == z).ToList();
            foreach (var tileEntity in toRemove)
            {
                GameController.TileEntities.Remove(tileEntity);
            }
        }

        public int GetBlock(int x, int y, int z)
        {
            return this[x >> 4, z >> 4]?.GetBlock(x & 15, y, z & 15) ?? -1;
        }

        public int GetBlock(Location location)
        {
            return GetBlock(location.X, location.Y, location.Z);
        }

        public void SetBlock(int x, int y, int z, int id)
        {
            this[x >> 4, z >> 4]?.SetBlock(x & 15, y, z & 15, id);
        }

        public Location SearchBlock(HashSet<int> ids)
        {
            var x = MathHelper.FloorFloat(GameController.Player.X);
            var z = MathHelper.FloorFloat(GameController.Player.Z);
            var y = (int) GameController.Player.Y;

            return SearchBlock(x,y,z, ids.Contains);
        }

        public Location SearchBlock(int x, int y, int z, Func<int, bool> idPredicate )
        {
            Location result = null;
            var maxDistance = Settings.ScanRange;

            for (var d = 1; d < maxDistance; d++)
            {
                for (var i = 0; i <= d; i++)
                {
                    for (var j = 0; j <= d - i; j++)
                    {
                        var x1 = x + i;
                        var z1 = z + j;
                        var y1 = y + (d - i - j);

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

        public List<Location> SearchBlocks(int x, int y, int z, Func<int, bool> idPredicate)
        {
            var result = new HashSet<Location>();
            Location tmp;

            var maxDistance = Settings.ScanRange;

            if (Check(x, y, z, idPredicate, out tmp))
                result.Add(tmp);

            for (var d = 1; d < maxDistance; d++)
            {
                for (var i = 0; i <= d; i++)
                {
                    for (var j = 0; j <= d - i; j++)
                    {
                        var x1 = x + i;
                        var z1 = z + j;
                        var y1 = y + (d - i - j);

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

        public List<Location> SearchBlocks(HashSet<int> ids)
        {
            var x = MathHelper.FloorFloat(GameController.Player.X);
            var z = MathHelper.FloorFloat(GameController.Player.Z);
            var y = (int)GameController.Player.Y;

            return SearchBlocks(x,y,z, ids.Contains);
        }

        private bool Check(int x, int y, int z, Func<int, bool> idPredicate , out Location result)
        {
            if (y < 0)
            {
                result = null;
                return false;
            }
            var id = GetBlock(x, y, z);
            if (idPredicate(id))
            {
                result = new Location(x,y,z);
                return true;
            }
            result = null;
            return false;
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
            var floor = GetBlock(x, y, z);
            var upper = GetBlock(x, y + 1, z);

            return IsBlockFree(floor) && IsBlockFree(upper);
        }

        public bool IsBlockFree(Location location)
        {
            return IsBlockFree(GetBlock(location.X, location.Y, location.Z));
        }

        public bool IsBlockFree(int x, int y, int z)
        {
            return IsBlockFree(GetBlock(x, y, z));
        }

        private static bool IsBlockFree(int id)
        {
            return Block.GetBlock(id).Transparent;
        }
    }
}
