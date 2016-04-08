using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketCustomPayload : Packet
    {
        public String channel;
        public ushort PayloadLength { get; private set; }
        public byte[] Payload { get; private set; }

        public PacketBuffer.Buffer MyPayload;

        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketCustomPayload(this);
        }

        public override void ReadPacketData(PacketBuffer buff)
        {
            channel = buff.ReadString();
            PayloadLength = (ushort)buff.ReadShort();
            Payload = buff.ReadBytes(PayloadLength);
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            buff.WriteString(channel);
            buff.WriteShort((short)MyPayload.ActualLength);
            buff.WriteBytes(MyPayload);
        }
    }
}
