using System.IO;
using System.IO.Compression;

namespace MoBot.Structure
{
    public class Decompressor
    {
        // ZLib Decompressor.
        private readonly byte[] _thisdata;

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
