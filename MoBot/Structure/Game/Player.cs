using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game
{
    class Player : LivingEntity
    {
        public Item[] inventory = new Item[45];
        public int HeldItem = 0;
        public bool onGround;

        public short Food;
        public float Saturation;
        public string Name = "";

        public Player(string name)
        {
            Name = name;
        }
   
        public int getItemSlot(int id)
        {
            for (int i = 9; i < 45; i++)
            {
                if (inventory[i] != null && inventory[i].ID == id)
                    return i;
            }
            return -1;
        }

        public int getFreeSlot()
        {
            for (int i = 9; i < 45; i++)
            {
                if (inventory[i] == null || inventory[i].ID == -1)
                    return i;
            }
            return -1;
        }

        public override string ToString()
        {
            return $"Player: {Name}, ({(int)x} | {(int)y} | {(int)z})";
        }
    }
}
