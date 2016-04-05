using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Handshake
{
    class PacketResponse : Packet
    {
        public string JSONResponse;

        public override void HandlePacket(IHandler handler)
        {
            throw new NotImplementedException();
        }

        public override void ReadPacketData(PacketBuffer buff)
        {
            JSONResponse = buff.ReadString();
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            throw new NotImplementedException();
        }
    }
}
