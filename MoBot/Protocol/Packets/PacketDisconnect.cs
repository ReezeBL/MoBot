using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets
{
    class PacketDisconnect : Packet
    {
        public String reason;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketDisconnect(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            reason = buff.ReadString();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }

        public override bool ProceedNow()
        {
            return true;
        }
    }
}
