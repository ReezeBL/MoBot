using MoBot.Structure.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketWindowItems : Packet
    {
        public byte WindowID;
        public short ItemCount;
        public Item[] Items;

        public override void ReadPacketData(PacketBuffer buff)
        {
            WindowID = buff.ReadByte();
            ItemCount = buff.ReadShort();
            Items = new Item[ItemCount];
            for(int i = 0; i < ItemCount; i++)
            {
                Items[i] = Packet.ReadItem(buff);
            }
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            throw new NotImplementedException();
        }

        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketWindowItems(this);
        }
    }
}
