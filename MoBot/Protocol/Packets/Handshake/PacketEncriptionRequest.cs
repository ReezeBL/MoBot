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
            handler.HandlePacketEncriptionRequest(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            ServerId = buff.ReadString();
            short length = buff.ReadShort();
            Key = buff.ReadBytes(length);
            length = buff.ReadShort();
            Token = buff.ReadBytes(length);
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