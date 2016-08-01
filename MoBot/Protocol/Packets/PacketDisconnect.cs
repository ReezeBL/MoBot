using System;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets
{
    public class PacketDisconnect : Packet
    {
        public string Reason;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketDisconnect(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            Reason = buff.ReadString();
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
