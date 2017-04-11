using fNbt;
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

        protected static void WriteItem(StreamWrapper buff, ItemStack itemStack)
        {
            if (itemStack == null || itemStack.Item.Id < 0)
                buff.WriteShort(-1);
            else
            {
                buff.WriteShort((short)itemStack.Item.Id);
                buff.WriteByte(itemStack.ItemCount);
                buff.WriteShort(itemStack.ItemDamage);
                if (itemStack.NbtRoot != null)
                {
                    buff.WriteShort((short)itemStack.NbtData.Length);
                    buff.WriteBytes(itemStack.NbtData);
                }
                else
                {
                    buff.WriteShort(-1);
                }
            }
        }

        protected static ItemStack ReadItem(StreamWrapper buff)
        {
            var id = buff.ReadShort();
            var item = new ItemStack(id);
            if (id < 0) return item;

            item.ItemCount = buff.ReadByte();
            item.ItemDamage = buff.ReadShort();
            var nbtLength = buff.ReadShort();
            if (nbtLength < 0)
                return item;

            item.NbtData = buff.ReadBytes(nbtLength);

            var reader = new NbtFile() {BigEndian =  true};
            reader.LoadFromBuffer(item.NbtData, 0, nbtLength, NbtCompression.AutoDetect);

            item.NbtRoot = reader.RootTag;
            return item;
        }
    }
}