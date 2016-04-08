using MoBot.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoBot.Structure
{
    class ReadingThread
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
                while (true)
                {
                    Packet packet = model.mainChannel.GetPacket();
                    if (packet != null)
                    {
                        if (packet.ProceedNow())
                            packet.HandlePacket(model.handler);
                        else
                            lock (queueLocker)
                                processQueue.Enqueue(packet);
                    }
                }
            })
            { IsBackground = true };
            processThread = new Thread(() =>
            {
                while (true)
                {
                    lock (queueLocker)
                    {
                        while (processQueue.Count > 0)
                            processQueue.Dequeue().HandlePacket(model.handler);
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
