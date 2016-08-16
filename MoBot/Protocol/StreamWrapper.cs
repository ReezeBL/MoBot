using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Protocol
{
    //Provides combined Network stream reading and writing
    public class StreamWrapper : IDisposable
    {
        private readonly Stream _mem = new MemoryStream(); //Allocated memory stream
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;

        public struct Buffer
        {
            public long ActualLength;
            public byte[] Bytes;
        }
        public StreamWrapper()
        {
            _reader = new BinaryReader(_mem);
            _writer = new BinaryWriter(_mem);
        }
        public StreamWrapper(byte[] data)
        {
            _mem = new MemoryStream(data);
            _reader = new BinaryReader(_mem);
            _writer = new BinaryWriter(_mem);
        }
        public StreamWrapper(Stream s)
        {
            _mem = s;
            _reader = new BinaryReader(s);
            _writer = new BinaryWriter(s);
        }

        public int ReadVarInt() // Reads variable length int to stream
        {
            int result = 0, length = 0;
            byte nextByte;

            do
            {
                nextByte = ReadByte();
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
                WriteByte((byte)(val & 127 | 128));
                val >>= 7;
            }
            WriteByte((byte)val);
        }
        public byte ReadByte()
        {
            return _reader.ReadByte();
        }
        public void WriteByte(byte b)
        {
            _writer.Write(b);
        }
        public void WriteInt(int val)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(val));
        }
        public int ReadInt()
        {
            return IPAddress.NetworkToHostOrder(_reader.ReadInt32());
        }
        public void WriteShort(short val)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(val));
        }
        public short ReadShort()
        {
            return IPAddress.NetworkToHostOrder(_reader.ReadInt16());
        }
        public void WriteBytes(byte[] val)
        {
            _writer.Write(val);
        }
        public void WriteBytes(Buffer val)
        {
            _writer.Write(val.Bytes, 0, (int)val.ActualLength);
        }
        public byte[] ReadBytes(int len)
        {
            return _reader.ReadBytes(len);
        }

        public int ReadBytes(byte[] buff)
        {
            return _reader.Read(buff, 0, buff.Length);
        }

        public void WriteString(string val) //Writes an UTF8 string to stream
        {
            byte[] bytes = Encoding.UTF8.GetBytes(val);
            WriteVarInt(bytes.Length);
            _writer.Write(bytes);
        }     
        public String ReadString() //Reads an UTF8 string from stream
        {
            int length = ReadVarInt();
            if (length < 0)
            {
                throw new IOException("The received encoded string Bytes length is less than zero! Weird string!");
            }
            byte[] buffer = _reader.ReadBytes(length);
            return Encoding.UTF8.GetString(buffer);
        }

        public String ReadStringT()
        {
            return _reader.ReadString();
        }

        public float ReadSingle()
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(IPAddress.NetworkToHostOrder(_reader.ReadInt32())), 0);
        }
        public void WriteSingle(float val)
        {
            _writer.Write(IPAddress.HostToNetworkOrder((BitConverter.ToInt32(BitConverter.GetBytes(val), 0))));
        }
        public double ReadDouble()
        {
            return BitConverter.Int64BitsToDouble(IPAddress.NetworkToHostOrder(_reader.ReadInt64()));
        }
        public void WriteDouble(double val)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(BitConverter.DoubleToInt64Bits(val)));
        }
        public bool ReadBool()
        {
            return _reader.ReadBoolean();
        }
        public void WriteBool(bool val)
        {
            _writer.Write(val);
        }
        public void WriteLong(long val)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(val));
        }
        public long ReadLong()
        {
            return IPAddress.NetworkToHostOrder(_reader.ReadInt64());
        }
        public Buffer GetBlob()//Return contains of memory stream
        {
            var memoryStream = _mem as MemoryStream;
            if (memoryStream != null)
                return new Buffer { ActualLength = _mem.Length, Bytes = memoryStream.GetBuffer() };
            return new Buffer();
        }

        public void Dispose()
        {
            _mem?.Dispose();
        }

        public void Clear()
        {
            Task t = _mem.FlushAsync();
            t.Wait();
        }
        public Stream GetStream()
        {
            return _mem;
        }
    }
}
