using System.IO;
using System.IO.Compression;

namespace MoBot.Structure
{
    class Decompressor
    {
        // ZLib Decompressor.
        readonly byte[] _thisdata;

        public Decompressor(byte[] data)
        {
            _thisdata = data;
        }

        public byte[] Decompress()
        {
            using (var compressedStream = new MemoryStream(_thisdata))
            using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}
