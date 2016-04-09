using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketPlayerAbilities : Packet
    {
        public byte Flags;
        public float FlyingSpeed;
        public float ViewModifier;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketPlayerAbliities(this);
        }

        public override void ReadPacketData(PacketBuffer buff)
        {
            Flags = buff.ReadByte();
            FlyingSpeed = buff.ReadSingle();
            ViewModifier = buff.ReadSingle();
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            buff.WriteByte(Flags);
            buff.WriteSingle(FlyingSpeed);
            buff.WriteSingle(ViewModifier);
        }
    }
}
