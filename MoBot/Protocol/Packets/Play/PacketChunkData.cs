using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;
using MinecraftEmuPTS.GameData;

namespace MoBot.Protocol.Packets.Play
{
    class PacketChunkData : Packet
    {
        public Chunk chunk;
        public byte[] ChunkData;
        public int Length;
        public bool RemoveChunk;
        public int x, z;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketChunkData(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            x = buff.ReadInt();
            z = buff.ReadInt();
            bool groundUp = buff.ReadBool();
            short pbitmap = buff.ReadShort();
            short abitmap = buff.ReadShort();
            Length = buff.ReadInt();
            ChunkData = buff.ReadBytes(Length);
            chunk = new Chunk(x, z, pbitmap, abitmap, groundUp, true);
            RemoveChunk = pbitmap == 0;
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}
