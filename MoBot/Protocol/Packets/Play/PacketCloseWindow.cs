using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketCloseWindow : Packet
    {
        public byte WindowId;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketCloseWindow(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            WindowId = buff.ReadByte();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            buff.WriteByte(WindowId);
        }
    }
}
