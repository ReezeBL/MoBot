using System;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol
{
    class PacketSpawnPlayer : Packet
    {
        public int EntityID;
        public string name;
        public double x, y, z;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketSpawnPlayer(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            EntityID = buff.ReadVarInt();
            String UUID = buff.ReadString();
            name = buff.ReadString();
            int len = buff.ReadVarInt();
            for (int i = 0; i < len; i++) 
            {
                buff.ReadString();
                buff.ReadString();
                buff.ReadString();
            }

            x = buff.ReadInt() / 32.0;
            y = buff.ReadInt() / 32.0;
            z = buff.ReadInt() / 32.0;
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}