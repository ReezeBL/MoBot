using System;
using MoBot.Protocol.Handlers;
using MoBot.Structure.Game;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketSetSlot : Packet
    {
        public byte WindowId;
        public short Slot;
        public Item Item;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketSetSlot(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            WindowId = buff.ReadByte();
            Slot = buff.ReadShort();
            Item = ReadItem(buff);
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}
