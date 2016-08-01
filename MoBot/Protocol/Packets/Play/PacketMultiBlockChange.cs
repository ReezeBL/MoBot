using System;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketMultiBlockChange : Packet
    {
        public int ChunkXPosiiton;
        public int ChunkZPosition;
        public int Size;
        public int Length;
        public byte[] Metadata;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketMultiBlockChange(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            ChunkXPosiiton = buff.ReadInt();
            ChunkZPosition = buff.ReadInt();
            Size = buff.ReadShort() & 65535;
            Length = buff.ReadInt();
            if (Length > 0)
                Metadata = buff.ReadBytes(Length);
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}
