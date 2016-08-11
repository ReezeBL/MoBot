using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MoBot.Structure;

namespace MoBot.Protocol.Threading
{
    class ReadingThread : BaseThread
    {
        private readonly object _queueLocker = new object();
        private Thread ReadThread { get; }
        private Thread ProcessThread { get; }
        private readonly Queue<Packet> _processQueue = new Queue<Packet>();
        public ReadingThread()
        {           
            ReadThread = new Thread(() =>
            {
                while (Process)
                {
                    try
                    {
                        var packet = NetworkController.MainChannel.GetPacket();
                        if (packet == null) continue;
                        if (packet.ProceedNow())
                            packet.HandlePacket(NetworkController.Handler);
                        else
                            lock (_queueLocker)
                                _processQueue.Enqueue(packet);
                    }
                    catch (EndOfStreamException)
                    {
                        NetworkController.Disconnect();
                        NetworkController.NotifyViewer("Client diconnected!");
                    }
                    catch (Exception exception)
                    {
                        Program.GetLogger().Error($"Reading thread: {exception}");
                    }
                }
            })
            { IsBackground = true };
            ProcessThread = new Thread(() =>
            {
                while (Process)
                {
                    try
                    {
                        while (_processQueue.Count > 0)
                        {
                            Packet packet;
                            lock(_queueLocker)
                                packet = _processQueue.Dequeue();
                            packet.HandlePacket(NetworkController.Handler);
                        }
                    }
                    catch (Exception exception)
                    {
                        Program.GetLogger().Error($"Processing thread: {exception}");
                    }
                    Thread.Sleep(10);
                }
            })
            { IsBackground = true };
            ReadThread.Start();
            ProcessThread.Start();
        }
    }
}
