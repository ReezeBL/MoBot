using System;
using MoBot.Protocol.Handlers;
using MoBot.Core.Game.World;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketChunkData : Packet
    {
        public Chunk Chunk;
        public byte[] ChunkData;
        public int Length;
        public bool RemoveChunk;
        public int X, Z;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketChunkData(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            X = buff.ReadInt();
            Z = buff.ReadInt();
            var groundUp = buff.ReadBool();
            var pbitmap = buff.ReadShort();
            var abitmap = buff.ReadShort();
            Length = buff.ReadInt();
            ChunkData = buff.ReadBytes(Length);
            Chunk = new Chunk(X, Z, pbitmap, abitmap, groundUp, true);

            RemoveChunk = pbitmap == 0;
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}
