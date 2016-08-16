using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fNbt;
using MoBot.Protocol;
using MoBot.Protocol.Handlers;
using MoBot.Structure;
using MoBot.Structure.Game;
using MoBot.Structure.Game.AI.Pathfinding;
using MoBot.Structure.Game.Items;
using MoBot.Structure.Game.World;
using Org.BouncyCastle.Cms;

namespace MoBot.Scripts.Handlers
{
    class IC2Handler : CustomHandler
    {
        [ImportHandler.PreInit]
        public static void Init()
        {
            ClientHandler.CustomHandlers.Add("ic2", new IC2Handler());
        }

        private MemoryStream data;

        public override void OnPacketData(byte[] payloadData)
        {
            var payload = new StreamWrapper(payloadData);
            byte id = payload.ReadByte();
            switch (id)
            {
                case 0:
                    {
                        byte descriptor = payload.ReadByte();
                        //Console.WriteLine($"{descriptor & 1} {descriptor & 2} {descriptor >> 2}");
                        if ((descriptor & 1) != 0)
                        {
                            data = new MemoryStream();
                        }
                        byte[] tmp = new byte[4096];
                        int count;
                        while ((count = payload.ReadBytes(tmp)) > 0)
                            data.Write(tmp, 0, count);

                        if ((descriptor & 2) != 0)
                        {
                            var prepared = data.ToArray().Skip(2).ToArray();
                            var deflatedData = Decompressor.Decompress(prepared);
                            if (descriptor >> 2 == 0)
                            {
                                ProcessInitPacket(new StreamWrapper(deflatedData));
                            }
                        }
                    }
                    break;
            }
        }

        private void ProcessInitPacket(StreamWrapper payload)
        {
            int dimensionId = payload.ReadInt();
            while (true)
            {
                label53:
                int x;
                try
                {
                    x = payload.ReadInt();
                }
                catch (Exception e)
                {
                    payload.GetStream().Close();
                    return;
                }
                int y = payload.ReadInt();
                int z = payload.ReadInt();
                Console.WriteLine($"{x} {y} {z}");
                byte[] buff = payload.ReadBytes(payload.ReadInt());
                Console.WriteLine(buff.Length);
                StreamWrapper stream = new StreamWrapper(buff);
                Dictionary<string, object> tags = new Dictionary<string, object>();
                while (true)
                {
                    String field;
                    try
                    {
                        field = stream.ReadStringT();
                    }
                    catch (Exception)
                    {
                        Location location = new Location(x,y,z);
                        if (GameController.World.GetBlock(location) == -1)
                            break;
                        GameController.SetTileEntity(location, null, tags);
                        goto label53;
                    }
                    Console.WriteLine(field);
                    object val = Decode(stream, stream.ReadByte());
                    tags.Add(field, val);
                }
            }
        }

        private static object Decode(StreamWrapper stream, int type)
        {
            int x, y;
            switch (type)
            {
                case 0:
                    return null;
                case 1:
                    byte arrType = stream.ReadByte();
                    y = stream.ReadVarInt();
                    bool isMulti = stream.ReadBool();
                    var res = new object[y];
                    for (int i = 0; i < y; i++)
                    {
                        if (isMulti)
                            arrType = stream.ReadByte();
                        res.SetValue(Decode(stream, arrType), i);
                    }
                    return res;
                case 2:
                    return stream.ReadByte();
                case 3:
                    return stream.ReadShort();
                case 4:
                    return stream.ReadInt();
                case 5:
                    return stream.ReadLong();
                case 6:
                    return stream.ReadSingle();
                case 7:
                    return stream.ReadDouble();
                case 8:
                    return stream.ReadBool();
                case 9:
                    return (char) stream.ReadByte();
                case 10:
                    return stream.ReadStringT();
                case 11:
                    return stream.ReadVarInt();
                case 12:
                    String item = (String) Decode(stream, 14);
                    x = stream.ReadByte();
                    short var15 = stream.ReadShort();
                    NbtCompound tag = (NbtCompound) (Decode(stream, 15));
                    return new ItemStack(-2) {Item = new Item() {Name = item}, NbtRoot = tag, ItemCount = (byte)x, ItemDamage = var15};
                case 13:
                    return stream.ReadStringT();
                case 14:
                    return stream.ReadStringT();
                case 15:
                    NbtFile reader = new NbtFile() {BigEndian = true};
                    reader.LoadFromStream(stream.GetStream(), NbtCompression.AutoDetect);
                    return reader.RootTag;
                case 16:
                    return stream.ReadInt();
                case 17:
                    return stream.ReadInt();
                case 18:
                    return stream.ReadStringT();
                case 19:
                    stream.ReadInt();
                    stream.ReadInt();
                    stream.ReadInt();
                    return null;
                case 20:
                    stream.ReadInt();
                    stream.ReadInt();
                    return null;
                case 21:
                    stream.ReadInt();
                    stream.ReadInt();
                    stream.ReadInt();
                    return null;
                case 22:
                    int world = (int) Decode(stream, 23);
                    stream.ReadInt();
                    stream.ReadInt();
                    stream.ReadInt();
                    return null;
                case 23:
                    return stream.ReadInt();
                case 24:
                    stream.ReadInt();
                    stream.ReadInt();
                    Decode(stream, 15);
                    return null;
                case 25:
                    Decode(stream, 24);
                    stream.ReadInt();
                    return null;
                case 26:
                    return new object();
                default:
                    return null;
            }
        }
    }
}
