using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace MoBotServer
{
    public class Program
    {
        public static void Main()
        {
            var assembly = File.ReadAllBytes("TestingAssembly.dll");
            Console.WriteLine(assembly.Length);

            var listener = new TcpListener(IPAddress.Any, 1488);
            listener.Start();
            while (true)
            {
                var client = listener.AcceptTcpClient();
                var writer = new BinaryWriter(client.GetStream());

                writer.Write(assembly.Length);
                writer.Write(assembly);
            }
        }
    }
}
