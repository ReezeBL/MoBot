using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Handshake
{
    public class EmptyPacket : Packet
    {
        public override void HandlePacket(IHandler handler)
        {
            
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            
        }
    }
}
