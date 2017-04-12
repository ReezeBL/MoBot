using fNbt;
using JetBrains.Annotations;
using MoBot.Core.Game.Items;

namespace MoBot.Core.Game
{
    public class ItemStack
    {
        public Item Item { get; }
        public byte ItemCount;
        public short ItemDamage;

        public NbtCompound NbtRoot;
        public byte[] NbtData;

        public static ItemStack CreateItemStack(int itemId)
        {
            var item = Item.GetItem(itemId);
            return item == null ? null : new ItemStack(item);
        }

        private ItemStack([NotNull] Item item)
        {
            Item = item;
        }

        public override string ToString()
        {
            return Item.Name ?? "";
        }
    }
}
