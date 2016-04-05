using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Handshake
{
    class PacketHandshake : Packet
    {
        public int protocolVersion;
        public string hostname;
        public ushort port;
        public int nextState;

        public override void HandlePacket(IHandler handler)
        {
            throw new NotImplementedException();
        }

        public override void ReadPacketData(PacketBuffer buff)
        {
            protocolVersion = buff.ReadVarInt();
            hostname = buff.ReadString();
            port = (ushort)buff.ReadShort();
            nextState = buff.ReadVarInt();
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            buff.WriteVarInt(protocolVersion);
            buff.WriteString(hostname);
            buff.WriteShort((short)port);
            buff.WriteVarInt(nextState);
        }
    }
}
