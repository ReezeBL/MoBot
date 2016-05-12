using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;
using MoBot.Structure.Game;

namespace MoBot.Protocol.Packets.Play
{
    class PacketClickWindow : Packet
    {
        public byte WindowID;
        public short Slot;
        public byte Button;
        public short ActionNumber;
        public byte Mode;
        public Item ItemStack;
        public override void HandlePacket(IHandler handler)
        {
            throw new NotImplementedException();
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }

        public override void WritePacketData(StreamWrapper buff)
        {           
            buff.WriteByte(WindowID);
            buff.WriteShort(Slot);
            buff.WriteByte(Button);
            buff.WriteShort(ActionNumber);
            buff.WriteByte(Mode);
            Packet.WriteItem(buff, ItemStack);
        }
    }
}
