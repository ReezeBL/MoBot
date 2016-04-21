using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Handshake
{
    class EmptyPacket : Packet
    {
        public override void HandlePacket(IHandler handler)
        {
            
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            
        }
    }
}
