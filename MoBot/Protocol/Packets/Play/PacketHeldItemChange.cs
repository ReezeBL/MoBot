using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketHeldItemChange : Packet
    {
        public byte Slot;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketHeldItemChange(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            Slot = buff.ReadByte();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            buff.WriteByte(Slot);
        }
    }
}
