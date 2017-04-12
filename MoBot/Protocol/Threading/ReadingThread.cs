using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MoBot.Core;
using NLog;

namespace MoBot.Protocol.Threading
{
    internal class ReadingThread : BaseThread
    {
        private readonly object queueLocker = new object();
        private Thread ReadThread { get; }
        private Thread ProcessThread { get; }
        private readonly Queue<Packet> processQueue = new Queue<Packet>();
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
                            lock (queueLocker)
                                processQueue.Enqueue(packet);
                    }
                    catch (EndOfStreamException)
                    {
                        NetworkController.Disconnect();
                        NetworkController.NotifyViewer("Client diconnected!");
                    }
                    catch (IOException)
                    {
                        NetworkController.Disconnect();
                        NetworkController.NotifyViewer("Client diconnected!");
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"Reading thread: {exception}");
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
                        while (processQueue.Count > 0)
                        {
                            Packet packet;
                            lock(queueLocker)
                                packet = processQueue.Dequeue();
                            packet.HandlePacket(NetworkController.Handler);
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"Processing thread: {exception}");
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
