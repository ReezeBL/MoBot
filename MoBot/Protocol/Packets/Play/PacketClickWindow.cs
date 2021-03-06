﻿using System;
using MoBot.Protocol.Handlers;
using MoBot.Core.Game;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketClickWindow : Packet
    {
        public byte WindowId;
        public short Slot;
        public byte Button;
        public short ActionNumber;
        public byte Mode;
        public ItemStack ItemStack;
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
            buff.WriteByte(WindowId);
            buff.WriteShort(Slot);
            buff.WriteByte(Button);
            buff.WriteShort(ActionNumber);
            buff.WriteByte(Mode);
            WriteItem(buff, ItemStack);
        }
    }
}
