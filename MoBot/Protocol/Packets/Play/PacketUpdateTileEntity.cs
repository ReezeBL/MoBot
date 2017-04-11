using System;
using fNbt;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketUpdateTileEntity : Packet
    {
        public int X, Z;
        public short Y;

        public byte Action;
        public byte[] NbtData;
        public NbtCompound Root;

        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketUpdateTileEntity(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            X = buff.ReadInt();
            Y = buff.ReadShort();
            Z = buff.ReadInt();
            Action = buff.ReadByte();
            int length = buff.ReadShort();
            if (length > 0)
            {
                NbtData = buff.ReadBytes(length);
                var reader = new NbtFile() { BigEndian = true };
                reader.LoadFromBuffer(NbtData, 0, length, NbtCompression.AutoDetect);
                Root = reader.RootTag;
            }
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}
