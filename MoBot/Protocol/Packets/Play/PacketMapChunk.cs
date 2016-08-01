using System;
using MoBot.Protocol.Handlers;
using MoBot.Structure.Game.World;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketMapChunk : Packet
    {
        public short ChunkNumber;
        public int DataLength;
        public bool Flag;
        public Chunk[] Chunks;
        public byte[] ChunkData;        
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketMapChunk(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            ChunkNumber = buff.ReadShort();
            DataLength = buff.ReadInt();
            Flag = buff.ReadBool();
            ChunkData = buff.ReadBytes(DataLength);
            Chunks = new Chunk[ChunkNumber];
            for(int i=0; i<ChunkNumber; i++)
            {
                int x = buff.ReadInt();
                int z = buff.ReadInt();
                short pbitmap = buff.ReadShort();
                short abitmap = buff.ReadShort();

                Chunks[i] = new Chunk(x, z, pbitmap, abitmap, Flag, true);
            }
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}
