using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;

namespace TestClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var client = new TcpClient("localhost", 1488);
            var reader = new BinaryReader(client.GetStream());

            var length = reader.ReadInt32();
            var bytes = reader.ReadBytes(length);

            var assembly = Assembly.Load(bytes);
            foreach (var type in assembly.GetExportedTypes())
            {
                Console.WriteLine(type.FullName);
            }
            //ScriptLoader.LoadExtension(assembly);
        }
    }
}
