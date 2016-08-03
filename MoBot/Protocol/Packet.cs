using MoBot.Protocol.Handlers;
using MoBot.Structure.Game;

namespace MoBot.Protocol
{
    public abstract class Packet
    {
        public abstract void HandlePacket(IHandler handler);
        public abstract void ReadPacketData(StreamWrapper buff);
        public abstract void WritePacketData(StreamWrapper buff);

        public virtual bool ProceedNow()
        {
            return false;
        }

        protected static void WriteItem(StreamWrapper buff, Item itemStack)
        {
            if (itemStack == null || itemStack.Id < 0)
                buff.WriteShort(-1);
            else
            {
                buff.WriteShort(itemStack.Id);
                buff.WriteByte(itemStack.ItemCount);
                buff.WriteShort(itemStack.ItemDamage);
                if (itemStack.NbtData != null)
                {
                    buff.WriteShort((short) itemStack.NbtData.Length);
                    buff.WriteBytes(itemStack.NbtData);
                }
                else
                {
                    buff.WriteShort(-1);
                }
            }
        }

        protected static Item ReadItem(StreamWrapper buff)
        {
            var item = new Item {Id = buff.ReadShort()};
            if (item.Id < 0) return item;

            item.ItemCount = buff.ReadByte();
            item.ItemDamage = buff.ReadShort();
            var nbtLength = buff.ReadShort();
            if (nbtLength < 0)
                return item;
            item.NbtData = buff.ReadBytes(nbtLength);
            return item;
        }
    }
}