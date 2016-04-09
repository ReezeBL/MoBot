using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;
using MoBot.Structure.Game;

namespace MoBot.Protocol.Packets.Play
{
    class PacketSetSlot : Packet
    {
        public byte WindowID;
        public short Slot;
        public Item item;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketSetSlot(this);
        }

        public override void ReadPacketData(PacketBuffer buff)
        {
            WindowID = buff.ReadByte();
            Slot = buff.ReadShort();
            item = Packet.ReadItem(buff);
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            throw new NotImplementedException();
        }
    }
}
