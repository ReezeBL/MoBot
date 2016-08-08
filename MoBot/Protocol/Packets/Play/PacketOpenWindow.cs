﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketOpenWindow : Packet
    {
        public int WorldId;
        public int WindowId;
        public String WindowName;
        public int SlotNumber;
        public bool HasCustomInventory;
        public int EntityId;

        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketOpenWindow(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            WindowId = buff.ReadByte();
            WorldId = buff.ReadByte();
            WindowName = buff.ReadString();
            SlotNumber = buff.ReadByte();
            HasCustomInventory = buff.ReadBool();

            if (WindowId == 11)
                EntityId = buff.ReadInt();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}
