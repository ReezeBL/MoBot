using System;
using MoBot.Protocol.Handlers;
using MoBot.Structure.Game;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketWindowItems : Packet
    {
        public byte WindowId;
        public short ItemCount;
        public ItemStack[] ItemsStack;

        public override void ReadPacketData(StreamWrapper buff)
        {
            WindowId = buff.ReadByte();
            ItemCount = buff.ReadShort();
            ItemsStack = new ItemStack[ItemCount];
            for(var i = 0; i < ItemCount; i++)
            {
                ItemsStack[i] = ReadItem(buff);
            }
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }

        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketWindowItems(this);
        }
    }
}
