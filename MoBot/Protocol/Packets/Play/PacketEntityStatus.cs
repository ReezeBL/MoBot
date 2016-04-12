using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketEntityStatus : Packet
    {
        public int EntityID;
        public byte EntityStatus;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketEntityStatus(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            EntityID = buff.ReadInt();
            EntityStatus = buff.ReadByte();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}
