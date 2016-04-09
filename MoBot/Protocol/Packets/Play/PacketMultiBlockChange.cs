using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketMultiBlockChange : Packet
    {
        public int chunkXPosiiton;
        public int chunkZPosition;
        public int size;
        public int length;
        public byte[] metadata;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketMultiBlockChange(this);
        }

        public override void ReadPacketData(PacketBuffer buff)
        {
            chunkXPosiiton = buff.ReadInt();
            chunkZPosition = buff.ReadInt();
            size = buff.ReadShort() & 65535;
            length = buff.ReadInt();
            if (length > 0)
                metadata = buff.ReadBytes(length);
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            throw new NotImplementedException();
        }
    }
}
