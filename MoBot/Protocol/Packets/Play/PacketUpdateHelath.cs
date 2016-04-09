using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketUpdateHelath : Packet
    {
        public float Health;
        public short Food;
        public float Saturation;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketUpdateHealth(this);
        }

        public override void ReadPacketData(PacketBuffer buff)
        {
            Health = buff.ReadSingle();
            Food = buff.ReadShort();
            Saturation = buff.ReadSingle();
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            throw new NotImplementedException();
        }
    }
}
