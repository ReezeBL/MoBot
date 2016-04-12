using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketChat : Packet
    {
        public String message;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketChat(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            message = buff.ReadString();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            buff.WriteString(message);
        }
    }
}
