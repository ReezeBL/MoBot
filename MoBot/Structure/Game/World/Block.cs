using MoBot.Structure.Game.AI.Pathfinding;

namespace MoBot.Structure.Game.World
{
    public class Block
    {
        public short Id;
        public byte Meta;
        public Block(int id, int meta = 0)
        {
            Id = (short)id;
            Meta = (byte) meta;

        }

        public static implicit operator GameBlock(Block block)
        {
            return GameBlock.GetBlock(block.Id);
        }

        public override string ToString()
        {
            return
                $"{GameBlock.GetBlock(Id).Name}";
        }
    }
}
