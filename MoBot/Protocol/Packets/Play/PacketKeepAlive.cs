using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketKeepAlive : Packet
    {
        public int Seed;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketKeepAlive(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            Seed = buff.ReadInt();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            buff.WriteInt(Seed);
        }

        public override bool ProceedNow()
        {
            return true;
        }
    }
}
