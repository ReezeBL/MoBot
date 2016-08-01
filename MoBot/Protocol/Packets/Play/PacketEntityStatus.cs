using System;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketEntityStatus : Packet
    {
        public int EntityId;
        public byte EntityStatus;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketEntityStatus(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            EntityId = buff.ReadInt();
            EntityStatus = buff.ReadByte();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}
