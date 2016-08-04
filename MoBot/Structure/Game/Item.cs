﻿using System.Collections.Generic;

namespace MoBot.Structure.Game
{
    public class Item
    {
        public short Id = -1;
        public byte ItemCount;
        public short ItemDamage;

        public byte[] NbtData;

        private static readonly Dictionary<int, Item> ItemRegistry = new Dictionary<int, Item>();
        public static void InitRegistry(){
            ItemRegistry.Add(0, null);
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public static Item GetItem(int id)
        {
            Item res;
            return !ItemRegistry.TryGetValue(id, out res) ? new Item { Id = (short)id } : res.CloneJson();
        }

        public virtual float GetItemEffectivness(GameBlock block)
        {
            if (block.Hardness < 0)
                return -1.0f;
            return 20.0f / (1.0f / block.Hardness / 100);
        }
    }
}
