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

        public short Food;
        public float Saturation;
        public string Name = "";

        public Player(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"Player: {Name}, ({(int)x} | {(int)y} | {(int)z})";
        }
    }
}
