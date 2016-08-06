namespace MoBot.Structure.Game
{
    public class Player : LivingEntity
    {
        public Item[] Inventory = new Item[45];
        public int HeldItem = 0;
        public bool OnGround;

        public short Food;
        public float Saturation;
        public string Name;

        public Item GetHeldItem => Inventory[HeldItem];

        public Player(int id, string name) : base(id)
        {
            Name = name;
        }

        public int GetItemSlot(int id)
        {
            for (int i = 9; i < 45; i++)
            {
                if (Inventory[i] != null && Inventory[i].Id == id)
                    return i;
            }
            return -1;
        }

        public int GetFreeSlot()
        {
            for (int i = 9; i < 45; i++)
            {
                if (Inventory[i] == null || Inventory[i].Id == -1)
                    return i;
            }
            return -1;
        }

        public override string ToString()
        {
            return $"Player: {Name}, ({(int)X} | {(int)Y} | {(int)Z})";
        }
    }
}
