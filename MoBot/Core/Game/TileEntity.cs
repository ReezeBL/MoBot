using System.Collections.Generic;
using fNbt;
using MoBot.Core.Game.AI.Pathfinding;

namespace MoBot.Core.Game
{
    public class TileEntity
    {
        public Location Location;
        public NbtCompound Root;
        public Dictionary<string, object> Tags;
        public int Id;

        public override string ToString()
        {
            return Block.GetBlock(GameController.World.GetBlock(Location)).ToString();
        }
    }
}
