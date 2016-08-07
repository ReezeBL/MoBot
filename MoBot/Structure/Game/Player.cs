using MoBot.Structure.Game.Items;

namespace MoBot.Structure.Game
{
    public class Player : LivingEntity
    {
        public ItemStack[] Inventory = new ItemStack[45];
        public int HeldItem = 0;
        public bool OnGround;

        public short Food;
        public float Saturation;
        public string Name;

        public Item GetHeldItem => Inventory[HeldItem].Item;

        public Player(int id, string name) : base(id)
        {
            Name = name;
        }

        public int GetItemSlot(int id)
        {
            for (int i = 9; i < 45; i++)
            {
                if (Inventory[i] != null && Inventory[i].Item.Id == id)
                    return i;
            }
            return -1;
        }

        public int GetFreeSlot()
        {
            for (int i = 9; i < 45; i++)
            {
                if (Inventory[i] == null || Inventory[i].Item.Id == -1)
                    return i;
            }
            return -1;
        }

        public override string ToString()
        {
            return $"Player: {Name}, ({MathHelper.floor_float(X)} | {(int)Y} | {MathHelper.floor_float(Z)})";
        }
    }
}
