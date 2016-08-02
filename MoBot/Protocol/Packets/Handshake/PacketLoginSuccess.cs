using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Handshake
{
    public class PacketLoginSuccess : Packet
    {
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketLoginSucess(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new System.NotImplementedException();
        }
    }
}