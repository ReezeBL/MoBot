using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketDestroyEntities : Packet
    {
        public byte Length;
        public int[] IDList;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketDestroyEntities(this);
        }

        public override void ReadPacketData(PacketBuffer buff)
        {
            Length = buff.ReadByte();
            IDList = new int[Length];
            for (int i = 0; i < Length; i++)
                IDList[i] = buff.ReadInt();
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            throw new NotImplementedException();
        }
    }
}
