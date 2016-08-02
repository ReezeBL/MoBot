using System;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol
{
    public class PacketRespawn : Packet
    {
        public byte Difficulty;
        public byte GameMode;
        public string LevelType;
        public int WorldType;

        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketRespawn(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            WorldType = buff.ReadInt();
            Difficulty = buff.ReadByte();
            GameMode = buff.ReadByte();
            LevelType = buff.ReadString();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}