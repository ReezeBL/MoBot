using MoBot.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoBot.Protocol
{
    class WritingThread
    {
        private Model model;
        public Object queueLocker { get; private set; } = new object();
        public Queue<Packet> SendingQueue { get; private set; } = new Queue<Packet>();
        public Thread thread { get; private set; }
        public WritingThread(Model model)
        {
            this.model = model;
            thread = new Thread(() =>
            {
                while (true)
                {
                    lock (queueLocker)
                    {
                        while (SendingQueue.Count > 0)
                        {
                            Packet pack = SendingQueue.Dequeue();
                            if (pack != null)
                                model.mainChannel.SendPacket(pack);
                        }
                    }
                    Thread.Sleep(50);
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
