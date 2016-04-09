﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketJoinGame : Packet
    {
        public int EntityID;
        public byte Gamemode;
        public int Dimension;
        public byte Difficulty;
        public byte MaxPlayers;
        public String LevelType;

        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketJoinGame(this);
        }

        public override void ReadPacketData(PacketBuffer buff)
        {
            EntityID = buff.ReadInt();
            Gamemode = buff.ReadByte();
            Dimension = buff.ReadByte();
            Difficulty = buff.ReadByte();
            MaxPlayers = buff.ReadByte();
            LevelType = buff.ReadString();
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            throw new NotImplementedException();
        }
    }
}
