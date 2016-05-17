using MoBot.Structure;
using System.Collections.Generic;
using System.Threading;

namespace MoBot.Protocol.Threading
{
    class ReadingThread : BaseThread
    {
        private readonly object _queueLocker = new object();
        private Thread ReadThread { get; set; }
        private Thread ProcessThread { get; set; }

        private readonly Model _model;
        private readonly Queue<Packet> _processQueue = new Queue<Packet>();
        public ReadingThread()
        {
            _model = Model.GetInstance();
            ReadThread = new Thread(() =>
            {
                while (Process)
                {
                    var packet = _model.MainChannel.GetPacket();
                    if (packet == null) continue;
                    if (packet.ProceedNow())
                        packet.HandlePacket(_model.Handler);
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
                            _processQueue.Dequeue().HandlePacket(_model.Handler);
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
