using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketEntityTeleport : Packet
    {
        public int EntityID;
        public double x, y, z;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketEntityTeleport(this);
        }

        public override void ReadPacketData(PacketBuffer buff)
        {
            EntityID = buff.ReadInt();
            x = buff.ReadInt() / 32.0;
            y = buff.ReadInt() / 32.0;
            z = buff.ReadInt() / 32.0;
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            throw new NotImplementedException();
        }
    }
}
