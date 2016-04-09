using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace MinecraftEmuPTS.Encription
{
    class Decompressor
    {
        // ZLib Decompressor.
        byte[] thisdata;

        public Decompressor(byte[] data)
        {
            thisdata = data;
        }

        public byte[] decompress()
        {
            using (var compressedStream = new MemoryStream(thisdata))
            using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}
