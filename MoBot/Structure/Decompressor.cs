using System.IO;
using System.IO.Compression;

namespace MoBot.Structure
{
    public static class Decompressor
    {
        // ZLib Decompressor.
        public static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}
