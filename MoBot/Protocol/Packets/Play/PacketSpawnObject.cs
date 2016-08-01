using System;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketSpawnObject : Packet
    {
        public int EntityId;
        public byte Type;
        public double X, Y, Z;

        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketSpawnObject(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            EntityId = buff.ReadVarInt();
            Type = buff.ReadByte();
            X = buff.ReadInt() / 32.0;
            Y = buff.ReadInt() / 32.0;
            Z = buff.ReadInt() / 32.0;
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}
