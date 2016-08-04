using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    internal class PacketUseEntity : Packet
    {
        public int TargetId { get; set; }
        public int Type { get; set; }

        public override void HandlePacket(IHandler handler)
        {
            throw new System.NotImplementedException();
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            throw new System.NotImplementedException();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            buff.WriteVarInt(TargetId);
            buff.WriteVarInt(Type);
        }
    }
}