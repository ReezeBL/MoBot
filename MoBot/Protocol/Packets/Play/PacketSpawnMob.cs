using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketSpawnMob : Packet
    {
        public int EntityID;
        public byte Type;
        public double X, Y, Z;

        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketSpawnMoob(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            EntityID = buff.ReadVarInt();
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
