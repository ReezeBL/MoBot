using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fNbt;
using MoBot.Structure.Game.AI.Pathfinding;

namespace MoBot.Structure.Game
{
    public class TileEntity
    {
        public Location Location;
        public NbtCompound Root;

        public override string ToString()
        {
            return Block.GetBlock(GameController.World.GetBlock(Location)).ToString();
        }
    }
}
