using System.Collections.Generic;
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
                    var packet = NetworkController.MainChannel.GetPacket();
                    if (packet == null) continue;
                    if (packet.ProceedNow())
                        packet.HandlePacket(NetworkController.Handler);
                    else
                        lock (_queueLocker)
                            _processQueue.Enqueue(packet);
                }
            })
            { IsBackground = true };
            ProcessThread = new Thread(() =>
            {
                while (Process)
                {
                    lock (_queueLocker)
                    {
                        while (_processQueue.Count > 0)
                            _processQueue.Dequeue().HandlePacket(NetworkController.Handler);
                    }
                    Thread.Sleep(50);
                }
            })
            { IsBackground = true };
            ReadThread.Start();
            ProcessThread.Start();
        }
    }
}
