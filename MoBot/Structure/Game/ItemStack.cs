using System;
using System.Collections.Generic;
using fNbt;
using MoBot.Structure.Game.Items;

namespace MoBot.Structure.Game
{
    public class ItemStack
    {
        public Item Item;
        public byte ItemCount;
        public short ItemDamage;

        public NbtCompound NbtRoot;
        public byte[] NbtData;
        public ItemStack(int id)
        {
            Item = Item.GetItem(id);
        }

        public override string ToString()
        {
            return Item?.Name ?? "";
        }

        
    }
}
