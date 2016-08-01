using System;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketJoinGame : Packet
    {
        public int EntityId;
        public byte Gamemode;
        public int Dimension;
        public byte Difficulty;
        public byte MaxPlayers;
        public string LevelType;

        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketJoinGame(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            EntityId = buff.ReadInt();
            Gamemode = buff.ReadByte();
            Dimension = buff.ReadByte();
            Difficulty = buff.ReadByte();
            MaxPlayers = buff.ReadByte();
            LevelType = buff.ReadString();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}
