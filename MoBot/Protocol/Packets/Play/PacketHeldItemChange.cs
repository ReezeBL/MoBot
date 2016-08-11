using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketHeldItemChange : Packet
    {
        public byte Slot;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketHeldItemChange(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            Slot = buff.ReadByte();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            buff.WriteShort(Slot);
        }
    }
}
