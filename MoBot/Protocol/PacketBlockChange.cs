using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol
{
    class PacketBlockChange : Packet
    {
        public int X, Z;
        public byte Y;
        public int BlockID;
        public byte BlockMetadata;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketBlockChange(this);
        }

        public override void ReadPacketData(PacketBuffer buff)
        {
            X = buff.ReadInt();
            Y = buff.ReadByte();
            Z = buff.ReadInt();
            BlockID = buff.ReadVarInt();
            BlockMetadata = buff.ReadByte();
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            throw new NotImplementedException();
        }
    }
}
