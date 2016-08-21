using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;
using MoBot.Structure.Game;

namespace MoBot.Protocol.Packets.Play
{
    internal class PacketPlayerBlockPlacement : Packet
    {
        public int X, Y, Z;
        public ItemStack Item;
        public byte Face;


        public override void HandlePacket(IHandler handler)
        {
            throw new NotImplementedException();
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            buff.WriteInt(X);
            buff.WriteByte((byte) Y);
            buff.WriteInt(Z);
            buff.WriteByte(Face);
            WriteItem(buff, Item);
            buff.WriteByte(7);
            buff.WriteByte(7);
            buff.WriteByte(7);
        }
    }
}
