using System.Collections.Generic;
using MoBot.Structure.Game.Items;

namespace MoBot.Structure.Game
{
    public class ItemStack
    {
        public Item Item;
        public byte ItemCount;
        public short ItemDamage;

        public ItemStack(int id)
        {
            Item = Item.GetItem(id);
        }

        public byte[] NbtData;

        public override string ToString()
        {
            return Item?.Name ?? "";
        }

        
    }
}
