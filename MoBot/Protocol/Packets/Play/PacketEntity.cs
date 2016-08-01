using System;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketEntity : Packet
    {
        public int EntityId;
        public double X, Y, Z;

        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketEntity(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            EntityId = buff.ReadInt();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }

        public class PacketEntityMove : PacketEntity
        {
            public override void ReadPacketData(StreamWrapper buff)
            {
                base.ReadPacketData(buff);
                X = (sbyte)buff.ReadByte() / 32.0;
                Y = (sbyte)buff.ReadByte() / 32.0;
                Z = (sbyte)buff.ReadByte() / 32.0;
            }
        }
    }
}
