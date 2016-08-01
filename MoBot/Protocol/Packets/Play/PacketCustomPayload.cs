using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketCustomPayload : Packet
    {
        public string Channel;
        public ushort PayloadLength { get; private set; }
        public byte[] Payload { get; private set; }

        public StreamWrapper.Buffer MyPayload;

        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketCustomPayload(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            Channel = buff.ReadString();
            PayloadLength = (ushort)buff.ReadShort();
            Payload = buff.ReadBytes(PayloadLength);
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            buff.WriteString(Channel);
            buff.WriteShort((short)MyPayload.ActualLength);
            buff.WriteBytes(MyPayload);
        }
    }
}
