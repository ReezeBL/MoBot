using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Protocol
{
    //Provides combined Network stream reading and writing
    class StreamWrapper : IDisposable
    {
        private Stream mem = new MemoryStream(); //Allocated memory stream
        private BinaryReader reader;
        private BinaryWriter writer;

        public struct Buffer
        {
            public long ActualLength;
            public byte[] buffer;
        }
        public StreamWrapper()
        {
            reader = new BinaryReader(mem);
            writer = new BinaryWriter(mem);
        }
        public StreamWrapper(byte[] data)
        {
            mem = new MemoryStream(data);
            reader = new BinaryReader(mem);
            writer = new BinaryWriter(mem);
        }
        public StreamWrapper(Stream s)
        {
            mem = s;
            reader = new BinaryReader(s);
            writer = new BinaryWriter(s);
        }

        public int ReadVarInt() // Reads variable length int to stream
        {
            int result = 0, length = 0;
            byte nextByte;

            do
            {
                nextByte = this.ReadByte();
                result |= (nextByte & 127) << length++ * 7;

                if (length > 5)
                {
                    throw new IOException("VarInt too big");
                }
            }
            while ((nextByte & 128) == 128);

            return result;
        }
        public void WriteVarInt(int val) // Writes variable length int to stream
        {
            while ((val & -128) != 0)
            {
                this.WriteByte((byte)(val & 127 | 128));
                val >>= 7;
            }
            this.WriteByte((byte)val);
        }
        public byte ReadByte()
        {
            return reader.ReadByte();
        }
        public void WriteByte(byte b)
        {
            writer.Write(b);
        }
        public void WriteInt(int val)
        {
            writer.Write(IPAddress.HostToNetworkOrder(val));
        }
        public int ReadInt()
        {
            return IPAddress.NetworkToHostOrder(reader.ReadInt32());
        }
        public void WriteShort(short val)
        {
            writer.Write(IPAddress.HostToNetworkOrder(val));
        }
        public short ReadShort()
        {
            return IPAddress.NetworkToHostOrder(reader.ReadInt16());
        }
        public void WriteBytes(byte[] val)
        {
            writer.Write(val);
        }
        public void WriteBytes(Buffer val)
        {
            writer.Write(val.buffer, 0, (int)val.ActualLength);
        }
        public byte[] ReadBytes(int len)
        {
            return reader.ReadBytes(len);
        }
        public void WriteString(string val) //Writes an UTF8 string to stream
        {
            byte[] bytes = Encoding.UTF8.GetBytes(val);
            this.WriteVarInt(val.Length);
            writer.Write(bytes);
        }     
        public String ReadString() //Reads an UTF8 string from stream
        {
            int length = this.ReadVarInt();
            if (length < 0)
            {
                throw new IOException("The received encoded string buffer length is less than zero! Weird string!");
            }
            byte[] buffer = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(buffer);
        }
        public float ReadSingle()
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(IPAddress.NetworkToHostOrder(reader.ReadInt32())), 0);
        }
        public void WriteSingle(float val)
        {
            writer.Write(IPAddress.HostToNetworkOrder((BitConverter.ToInt32(BitConverter.GetBytes(val), 0))));
        }
        public double ReadDouble()
        {
            return BitConverter.Int64BitsToDouble(IPAddress.NetworkToHostOrder(reader.ReadInt64()));
        }
        public void WriteDouble(double val)
        {
            writer.Write(IPAddress.HostToNetworkOrder(BitConverter.DoubleToInt64Bits(val)));
        }
        public bool ReadBool()
        {
            return reader.ReadBoolean();
        }
        public void WriteBool(bool val)
        {
            writer.Write(val);
        }
        public void WriteLong(long val)
        {
            writer.Write(IPAddress.HostToNetworkOrder(val));
        }
        public long ReadLong()
        {
            return IPAddress.NetworkToHostOrder(reader.ReadInt64());
        }
        public Buffer GetBlob()//Return contains of memory stream
        {
            return new Buffer() { ActualLength = mem.Length, buffer = (mem as MemoryStream).GetBuffer() };
        }
        public void Dispose()
        {
            if (mem != null)
                mem.Dispose();
        }
        public void Clear()
        {
            Task t = mem.FlushAsync();
            t.Wait();
        }
        public Stream getStream()
        {
            return mem;
        }
    }
}
