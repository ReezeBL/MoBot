using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketChat : Packet
    {
        public string Message;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketChat(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            Message = buff.ReadString();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            buff.WriteString(Message);
        }
    }
}
