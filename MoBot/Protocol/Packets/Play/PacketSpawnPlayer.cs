using System;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketSpawnPlayer : Packet
    {
        public int EntityId;
        public string Name;
        public double X, Y, Z;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketSpawnPlayer(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            EntityId = buff.ReadVarInt();
            buff.ReadString();
            Name = buff.ReadString();
            int len = buff.ReadVarInt();
            for (var i = 0; i < len; i++) 
            {
                buff.ReadString();
                buff.ReadString();
                buff.ReadString();
            }

            X = buff.ReadInt() / 32.0;
            Y = buff.ReadInt() / 32.0;
            Z = buff.ReadInt() / 32.0;
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}