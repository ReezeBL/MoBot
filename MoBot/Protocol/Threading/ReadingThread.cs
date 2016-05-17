using MoBot.Protocol;
using MoBot.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoBot.Protocol.Threading
{
    class ReadingThread : BaseThread
    {
        private object queueLocker = new object();
        public Thread readThread { get; private set; }
        public Thread processThread { get; private set; }

        private Model model;
        private Queue<Packet> processQueue = new Queue<Packet>();
        public ReadingThread(Model model)
        {
            this.model = model;
            readThread = new Thread(() =>
            {
                while (Process)
                {
                    Packet packet = model.MainChannel.GetPacket();
                    if (packet != null)
                    {
                        if (packet.ProceedNow())
                            packet.HandlePacket(model.Handler);
                        else
                            lock (queueLocker)
                                processQueue.Enqueue(packet);
                    }
                }
            })
            { IsBackground = true };
            processThread = new Thread(() =>
            {
                while (Process)
                {
                    lock (queueLocker)
                    {
                        while (processQueue.Count > 0)
                            processQueue.Dequeue().HandlePacket(model.Handler);
                    }
                    Thread.Sleep(50);
                }
            })
            { IsBackground = true };
            readThread.Start();
            processThread.Start();
        }
    }
}
