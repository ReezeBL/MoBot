using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game
{
    class Item
    {
        public short ID = 0;
        public byte ItemCount;
        public short ItemDamage;

        public byte[] NBTData;
    }
}
