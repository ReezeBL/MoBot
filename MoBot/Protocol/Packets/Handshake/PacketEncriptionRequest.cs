using System;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Handshake
{
    public class PacketEncriptionRequest : Packet
    {
        public byte[] Key;
        public string ServerId;
        public byte[] Token { get; set; }
        public override void HandlePacket(IHandler handler)
        {
            throw new NotImplementedException();
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}