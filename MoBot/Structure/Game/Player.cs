using System.Linq;
using MoBot.Structure.Game.Items;

namespace MoBot.Structure.Game
{
    public class Player : LivingEntity
    {

        public Container Inventory = new Container(9);
        public Container CurrentContainer;

        public int HeldItemBar = 0;
        public int HeldItem => HeldItemBar + 36;
        public bool OnGround;

        public short Food;
        public float Saturation;
        public string Name;

        public Item GetHeldItem => Inventory[HeldItem].Item;
        public ItemStack GetHeldItemStack => Inventory[HeldItem];

        public Player(int id, string name) : base(id)
        {
            Name = name;
            CurrentContainer = Inventory;
        }

        public override string ToString()
        {
            return $"Player: {Name}, ({MathHelper.floor_float(X)} | {(int)Y} | {MathHelper.floor_float(Z)})";
        }
    }
}
