using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game
{
    class Item
    {
        public short ID = -1;
        public byte ItemCount;
        public short ItemDamage;

        public byte[] NBTData;

        private static Dictionary<int, Item> itemRegistry = new Dictionary<int, Item>();
        public static void InitRegistry(){

        }

        public override string ToString()
        {
            return ID.ToString();
        }

        public static Item getItem(int id)
        {
            Item res = null;
            if (!itemRegistry.TryGetValue(id, out res))
                res = new Item() { ID = (short)id };
            return res;
        }

        public virtual float getItemEffectivness(GameBlock block)
        {
            if (block.hardness < 0)
                return -1.0f;
            return 20.0f / (1.0f / block.hardness / 100);
        }
    }
}
