using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketEntity : Packet
    {
        public int EntityID;
        public double x, y, z;

        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketEntity(this);
        }

        public override void ReadPacketData(PacketBuffer buff)
        {
            EntityID = buff.ReadInt();
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            throw new NotImplementedException();
        }

        public class PacketEntityMove : PacketEntity
        {
            public override void ReadPacketData(PacketBuffer buff)
            {
                base.ReadPacketData(buff);
                x = (sbyte)buff.ReadByte() / 32.0;
                y = (sbyte)buff.ReadByte() / 32.0;
                z = (sbyte)buff.ReadByte() / 32.0;
            }
        }
    }
}
